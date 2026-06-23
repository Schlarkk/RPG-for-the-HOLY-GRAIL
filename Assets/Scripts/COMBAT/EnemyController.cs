using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<PlayerAttack> availableAttacks;

    CharacterStats cs;

    int turnsSinceLastRotation = 0;
    HashSet<int> usedMoveIndices = new HashSet<int>();

    void Awake()
    {
        cs = GetComponent<CharacterStats>();
    }

    // Returns (attack, target) — null attack means defend
    public (PlayerAttack, CharacterStats) PickTurn(List<CharacterStats> players,
                                                    List<CharacterStats> enemies,
                                                    int currentActorSpeed)
    {
        cs.isDefending = false;

        if (Random.Range(0, 100) < 12)
            return (null, null);    // defend

        PlayerAttack attack = PickAttack();
        if (attack == null) return (null, null);

        bool useOnAlly = attack.canTargetAllies && !attack.canTargetEnemies;
        bool canUseOnAlly = attack.canTargetAllies &&
                            attack.canTargetEnemies &&
                            Random.Range(0, 100) < 40;

        CharacterStats target = (useOnAlly || canUseOnAlly)
            ? PickAllyTarget(enemies)
            : PickEnemyTarget(players);

        if (target == null) return (null, null);

        cs.ProcessOnAttackStatuses();
        return (attack, target);
    }

    PlayerAttack PickAttack()
    {
        turnsSinceLastRotation++;

        if (turnsSinceLastRotation >= 8)
        {
            turnsSinceLastRotation = 0;
            usedMoveIndices.Clear();
        }

        List<int> rotationCandidates = new List<int>();
        for (int i = 0; i < availableAttacks.Count; i++)
            if (!usedMoveIndices.Contains(i) && availableAttacks[i].mpCost < 100)
                rotationCandidates.Add(i);

        if (rotationCandidates.Count > 0 && turnsSinceLastRotation == 0)
        {
            int forced = rotationCandidates[Random.Range(0, rotationCandidates.Count)];
            usedMoveIndices.Add(forced);
            return availableAttacks[forced];
        }

        List<float> weights = new List<float>();
        float totalWeight = 0f;
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            float w = Mathf.Max(1f, 100f - availableAttacks[i].mpCost);
            weights.Add(w);
            totalWeight += w;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
            {
                usedMoveIndices.Add(i);
                return availableAttacks[i];
            }
        }

        return availableAttacks[0];
    }

    CharacterStats PickEnemyTarget(List<CharacterStats> players)
    {
        foreach (PlayerAttack atk in availableAttacks)
            foreach (CharacterStats p in players)
                if (p.IsAlive() && DamageController.GetTypeMultiplier(
                    atk.attackColour, p.characterType) >= 2f)
                    return p;

        CharacterStats best = null;
        foreach (CharacterStats p in players)
        {
            if (!p.IsAlive()) continue;
            if (best == null || p.health < best.health) best = p;
        }
        return best;
    }

    CharacterStats PickAllyTarget(List<CharacterStats> enemies)
    {
        CharacterStats best = null;
        foreach (CharacterStats e in enemies)
        {
            if (!e.IsAlive() || e == cs) continue;
            if (best == null || e.health < best.health) best = e;
        }
        return best ?? cs;
    }
}