using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Combatants")]
    public List<CharacterStats> playerCharacters;
    public List<CharacterStats> enemyCharacters;

    [Header("Player Attack Lists")]
    public List<AttackList> playerAttackLists;

    [Header("References")]
    public BattleUI battleUI;

    [Header("Settings")]
    public float hitDelay = 0.5f;   // seconds between hits in a multi-hit attack

    List<CharacterStats> turnQueue = new List<CharacterStats>();
    int currentIndex = 0;
    bool isPlayerTurn = false;
    int roundNumber = 0;
    bool roundStarting = false;

    [HideInInspector] public PlayerAttack PendingAttack;
    public CharacterStats CurrentActor => turnQueue[currentIndex];

    void Start() => StartCoroutine(DelayedStart());

    IEnumerator DelayedStart()
    {
        yield return null;
        StartRound();
    }

    // ── Round Setup ──────────────────────────────────────────

    void StartRound()
    {
        if (roundStarting) return;
        roundStarting = true;

        turnQueue.Clear();
        foreach (var cs in playerCharacters)
        {
            if (cs == null) continue;
            cs.RollSpeed();
            cs.isDefending = false;
            if (cs.IsAlive()) turnQueue.Add(cs);
        }
        foreach (var cs in enemyCharacters)
        {
            if (cs == null) continue;
            cs.RollSpeed();
            cs.isDefending = false;
            if (cs.IsAlive()) turnQueue.Add(cs);
        }

        if (turnQueue.Count == 0)
        {
            roundStarting = false;
            CheckBattleOver();
            return;
        }

        turnQueue.Sort((a, b) => b.rolledSpeed.CompareTo(a.rolledSpeed));
        currentIndex = 0;
        roundNumber++;

        CombatLog.Instance?.LogRoundStart(roundNumber);

        foreach (var cs in turnQueue)
            cs.ProcessRoundStartStatuses();

        battleUI.RefreshTurnOrderStrip(turnQueue, currentIndex);

        roundStarting = false;
        ProcessNextActor();
    }

    // ── Turn Loop ────────────────────────────────────────────

    void ProcessNextActor()
    {
        while (currentIndex < turnQueue.Count && !turnQueue[currentIndex].IsAlive())
            currentIndex++;

        if (currentIndex >= turnQueue.Count) { StartRound(); return; }
        if (CheckBattleOver()) return;

        CharacterStats actor = turnQueue[currentIndex];
        battleUI.RefreshTurnOrderStrip(turnQueue, currentIndex);

        if (actor.CheckCrippled()) { AdvanceTurn(); return; }

        // Fire charged attack if ready
        if (actor.isCharging && actor.chargedAttack != null)
        {
            actor.isCharging = false;
            CombatLog.Instance?.Log(
                $"<color=#FFD966>{actor.characterName} unleashes " +
                $"{actor.chargedAttack.attackName}!</color>");

            if (actor.chargedTarget != null)
            {
                StartCoroutine(ExecuteAttackCoroutine(
                    actor, actor.chargedTarget, actor.chargedAttack,
                    () => { actor.chargedAttack = null; actor.chargedTarget = null; }));
            }
            else
            {
                PendingAttack = actor.chargedAttack;
                actor.chargedAttack = null;
                isPlayerTurn = true;
                battleUI.ShowTargetPanelPublic(PendingAttack);
            }
            return;
        }

        if (actor.isEnemy)
        {
            isPlayerTurn = false;
            battleUI.HideAll();
            StartCoroutine(EnemyActCoroutine(actor));
        }
        else
        {
            isPlayerTurn = true;
            battleUI.ShowMainMenu(actor);
        }
    }

    // ── Attack execution — coroutine with per-hit delay ──────

    IEnumerator ExecuteAttackCoroutine(CharacterStats attacker, CharacterStats target,
                                   PlayerAttack attack, System.Action onComplete = null)
    {
        int hits = Mathf.Max(1, attack.hitCount);

        if (hits > 1)
            CombatLog.Instance?.Log(
                $"<color=#FFD966>{attack.attackName} hits {hits} times!</color>");

        attacker.ProcessOnAttackStatuses();

        // Determine if this is a heal — ally-only moves always heal,
        // mixed moves heal if target is a player character
        bool isHeal = attack.canTargetAllies && !attack.canTargetEnemies;
        if (!isHeal && attack.canTargetAllies)
            isHeal = playerCharacters.Contains(target);

        for (int i = 0; i < hits; i++)
        {
            if (!target.IsAlive()) break;

            int amount = DamageController.CalculateSingleHit(attacker, target, attack);

            if (isHeal)
            {
                target.health = Mathf.Clamp(target.health + amount, 0, target.maxHealth);
                CombatLog.Instance?.Log(
                    $"<color=#44FF88>{attacker.characterName} heals {target.characterName} " +
                    $"for {amount} HP!</color>");
            }
            else
            {
                target.health -= amount;
                target.health  = Mathf.Max(0, target.health);
            }

            target.RefreshUI();
            target.ApplyStatusArray(attack.statusesOnTarget, additive: true);

            if (!isHeal && !target.IsAlive())
            {
                CombatLog.Instance?.LogKO(target.characterName);
                break;
            }

            if (i < hits - 1)
                yield return new WaitForSeconds(hitDelay);
        }

        attacker.ApplyStatusArray(attack.statusesOnSelf);
        onComplete?.Invoke();
        EndActorTurn(attacker);
    }

    // ── Player Actions ───────────────────────────────────────

    public List<PlayerAttack> GetCurrentActorAttacks()
    {
        if (!isPlayerTurn) return new List<PlayerAttack>();
        int idx = playerCharacters.IndexOf(turnQueue[currentIndex]);
        if (idx >= 0 && idx < playerAttackLists.Count)
            return playerAttackLists[idx].attacks;
        return new List<PlayerAttack>();
    }

    public List<CharacterStats> GetValidTargets(PlayerAttack attack)
    {
        var valid = new List<CharacterStats>();
        if (attack.canTargetEnemies)
            foreach (var e in enemyCharacters) if (e != null && e.IsAlive()) valid.Add(e);
        if (attack.canTargetAllies)
            foreach (var p in playerCharacters) if (p != null && p.IsAlive()) valid.Add(p);
        return valid;
    }

    public void PlayerConfirmTarget(CharacterStats target)
    {
        if (!isPlayerTurn || PendingAttack == null) return;

        CharacterStats attacker = turnQueue[currentIndex];

        if (!attacker.HasMP(PendingAttack.mpCost))
        {
            CombatLog.Instance?.Log(
                $"<color=#FF4444>{attacker.characterName} doesn't have enough MP!</color>");
            return;
        }

        attacker.SpendMP(PendingAttack.mpCost);
        isPlayerTurn = false;           // block input during hit sequence
        battleUI.HideAll();

        if (PendingAttack.requiresCharge)
        {
            attacker.isCharging    = true;
            attacker.chargedAttack = PendingAttack;
            attacker.chargedTarget = target;
            CombatLog.Instance?.Log(
                $"<color=#AAD4F5>{attacker.characterName} is charging " +
                $"{PendingAttack.attackName}...</color>");
            PendingAttack = null;
            EndActorTurn(attacker);
            return;
        }

        PlayerAttack attackToUse = PendingAttack;
        PendingAttack = null;
        StartCoroutine(ExecuteAttackCoroutine(attacker, target, attackToUse));
    }

    public void PlayerChooseDefend()
    {
        if (!isPlayerTurn) return;
        CharacterStats actor = turnQueue[currentIndex];
        actor.isDefending = true;
        CombatLog.Instance?.LogDefend(actor.characterName);
        EndActorTurn(actor);
    }

    // ── Enemy Actions ────────────────────────────────────────

    IEnumerator EnemyActCoroutine(CharacterStats actor)
    {
        yield return new WaitForSeconds(1.2f);
        EnemyController ec = actor.GetComponent<EnemyController>();
        if (ec != null)
        {
            // Get the attack choice first, then run it as a coroutine
            (PlayerAttack chosenAttack, CharacterStats chosenTarget) =
                ec.PickTurn(playerCharacters, enemyCharacters, actor.rolledSpeed);

            if (chosenAttack != null && chosenTarget != null)
            {
                // Yield until the full hit sequence finishes before EndActorTurn
                bool done = false;
                yield return StartCoroutine(ExecuteAttackCoroutine(
                    actor, chosenTarget, chosenAttack, () => done = true));

                // Apply self statuses
                actor.ApplyStatusArray(chosenAttack.statusesOnSelf);
                yield break;    // ExecuteAttackCoroutine already calls EndActorTurn
            }
            else if (chosenAttack == null)
            {
                // Enemy chose to defend
                actor.isDefending = true;
                CombatLog.Instance?.LogDefend(actor.characterName);
            }
        }
        EndActorTurn(actor);
    }

    // ── End of turn ──────────────────────────────────────────

    void EndActorTurn(CharacterStats actor)
    {
        actor.ProcessEndOfTurnStatuses();
        if (CheckBattleOver()) return;
        AdvanceTurn();
    }

    void AdvanceTurn()
    {
        isPlayerTurn = false;
        currentIndex++;
        ProcessNextActor();
    }

    // ── Battle over ──────────────────────────────────────────

    bool CheckBattleOver()
    {
        if (playerCharacters.Count == 0 || enemyCharacters.Count == 0) return false;
        if (!enemyCharacters.Exists(e => e != null)) return false;
        if (!playerCharacters.Exists(p => p != null)) return false;

        bool allEnemiesDead = enemyCharacters.TrueForAll(e => e == null || !e.IsAlive());
        bool allPlayersDead = playerCharacters.TrueForAll(p => p == null || !p.IsAlive());

        if (allEnemiesDead)
        {
            battleUI.HideAll();
            CombatLog.Instance?.Log("<color=#FFD966>All enemies defeated! Victory!</color>");
            enabled = false;
            return true;
        }
        if (allPlayersDead)
        {
            battleUI.HideAll();
            CombatLog.Instance?.Log("<color=#FF4444>Your party has fallen...</color>");
            enabled = false;
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class AttackList { public List<PlayerAttack> attacks; }