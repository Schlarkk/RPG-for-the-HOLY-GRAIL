using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [Header("Info")]
    public string characterName;
    public bool isEnemy;

    [Header("Stats")]
    public int maxHealth;
    [HideInInspector] public int health;
    public int minSpeed;
    public int maxSpeed;
    public int critChance;
    public int baseDamage;
    public int armor;
    public int physicalResistance;
    public int specialResistance;

    [Header("MP")]
    public int maxMP;
    [HideInInspector] public int mp;
    public Slider mpSlider;
    public int mpRegenAmount;

    [Header("Type")]
    public PlayerAttack.AttackColour characterType;

    [Header("UI")]
    public Slider healthSlider;

    [HideInInspector] public int rolledSpeed;
    [HideInInspector] public bool isDefending;
    [HideInInspector] public bool isCharging;
    [HideInInspector] public PlayerAttack chargedAttack;
    [HideInInspector] public CharacterStats chargedTarget;
    [HideInInspector] public List<ActiveStatus> activeStatuses = new List<ActiveStatus>();

    void Awake()
    {
        health = maxHealth;
        mp     = maxMP;
    }

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value    = maxHealth;
        }
        if (mpSlider != null)
        {
            mpSlider.maxValue = maxMP;
            mpSlider.value    = maxMP;
        }
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value    = Mathf.Max(0, health);
        }
        if (mpSlider != null)
        {
            mpSlider.maxValue = maxMP;
            mpSlider.value    = Mathf.Max(0, mp);
        }
    }

    public bool IsAlive() => health > 0;

    // ── Stat modifiers ───────────────────────────────────────

    public float GetDamageMultiplier()
    {
        float mult = 1f;
        foreach (var s in activeStatuses)
        {
            if (s.definition.type == StatusEffect.StatusType.DamageUp)
                mult += s.potency / 100f;
            else if (s.definition.type == StatusEffect.StatusType.DamageDown)
                mult -= s.potency / 100f;
        }
        return Mathf.Max(0f, mult);
    }

    public float GetDefenseMultiplier()
    {
        float mult = 1f;
        foreach (var s in activeStatuses)
        {
            if (s.definition.type == StatusEffect.StatusType.Fragile)
                mult += s.potency / 100f;
            else if (s.definition.type == StatusEffect.StatusType.DefenseUp)
                mult -= s.potency / 100f;
        }
        return Mathf.Max(0f, mult);
    }

    public int GetSpeedBonus()
    {
        int bonus = 0;
        foreach (var s in activeStatuses)
            if (s.definition.type == StatusEffect.StatusType.Haste)
                bonus += s.potency;
        return bonus;
    }

    public int GetRuptureDamage()
    {
        int total = 0;
        foreach (var s in activeStatuses)
            if (s.definition.type == StatusEffect.StatusType.Rupture)
                total += s.potency;
        return total;
    }

    // Poise adds potency directly to effective crit chance
    public int GetEffectiveCritChance()
    {
        int bonus = 0;
        foreach (var s in activeStatuses)
            if (s.definition.type == StatusEffect.StatusType.Poise)
                bonus += s.potency * s.count;
        return Mathf.Min(100, critChance + bonus);
    }

    // ── Speed ────────────────────────────────────────────────

    public void RollSpeed()
    {
        int speedPenalty = 0;
        foreach (var s in activeStatuses)
            if (s.definition.type == StatusEffect.StatusType.Paralysis)
                speedPenalty += s.potency;

        rolledSpeed = Mathf.Max(0, Random.Range(minSpeed, maxSpeed + 1)
                                   - speedPenalty + GetSpeedBonus());
    }

    // ── MP ───────────────────────────────────────────────────

    public bool HasMP(int cost) => mp >= cost;

    public void SpendMP(int cost)
    {
        mp = Mathf.Max(0, mp - cost);
        RefreshUI();
    }

    // ── Status application ───────────────────────────────────

    // additive = true means count stacks on re-application (used for per-hit statuses)
    public void ApplyStatus(StatusEffect def, int potency, int count, bool additive = true)
    {
        if (def.type == StatusEffect.StatusType.Cleanse)
        {
            activeStatuses.Clear();
            CombatLog.Instance?.Log(
                $"<color=#FFFFFF>{characterName}'s status effects were cleansed!</color>");
            StatusDisplayManager.Instance?.Refresh(this);
            return;
        }

        foreach (var existing in activeStatuses)
        {
            if (existing.definition.type == def.type)
            {
                existing.potency = Mathf.Clamp(existing.potency + potency, 1, def.maxPotency);
                existing.count   = additive
                    ? Mathf.Clamp(existing.count + count, 1, def.maxCount)   // add count per hit
                    : Mathf.Clamp(Mathf.Max(existing.count, count), 1, def.maxCount); // keep highest
                CombatLog.Instance?.Log(
                    $"<color=#{ColorUtility.ToHtmlStringRGB(def.displayColour)}>" +
                    $"{characterName}'s {def.displayName} was refreshed! " +
                    $"(potency:{existing.potency}, count:{existing.count})</color>");
                StatusDisplayManager.Instance?.Refresh(this);
                return;
            }
        }

        activeStatuses.Add(new ActiveStatus(def, potency, count));
        CombatLog.Instance?.Log(
            $"<color=#{ColorUtility.ToHtmlStringRGB(def.displayColour)}>" +
            $"{characterName} now has {def.displayName}! " +
            $"(potency:{potency}, count:{count})</color>");
        StatusDisplayManager.Instance?.Refresh(this);
    }

    public void ApplyStatusArray(StatusInfliction[] inflictions, bool additive = true)
    {
        if (inflictions == null) return;
        foreach (var inf in inflictions)
            if (inf.statusToInflict != null)
                ApplyStatus(inf.statusToInflict, inf.potency, inf.count, additive);
    }

    // ── Status processing ────────────────────────────────────

    // Called at the START of each round — Burn deals damage here
    public void ProcessRoundStartStatuses()
    {
        List<ActiveStatus> toRemove = new List<ActiveStatus>();

        foreach (var s in activeStatuses)
        {
            if (s.definition.type == StatusEffect.StatusType.Burn)
            {
                health -= s.potency;
                health  = Mathf.Max(0, health);
                CombatLog.Instance?.Log(
                    $"<color=#FF6600>{characterName} burns for {s.potency}!</color>");
            }
        }

        RefreshUI();
        StatusDisplayManager.Instance?.Refresh(this);
    }

    // Called at END of each combatant's turn — counts tick down, regen/haste/etc resolve
    public void ProcessEndOfTurnStatuses()
    {
        List<ActiveStatus> toRemove = new List<ActiveStatus>();

        mp = Mathf.Min(maxMP, mp + mpRegenAmount);

        foreach (var s in activeStatuses)
        {
            switch (s.definition.type)
            {
                // Burn only damages at round start — just tick count here
                case StatusEffect.StatusType.Burn:
                    s.count--;
                    break;

                // Rupture only damages on hit — just tick count here
                case StatusEffect.StatusType.Rupture:
                    s.count--;
                    break;

                case StatusEffect.StatusType.Regeneration:
                    health = Mathf.Min(maxHealth, health + s.potency);
                    CombatLog.Instance?.Log(
                        $"<color=#44FF88>{characterName} regenerates {s.potency} HP!</color>");
                    s.count--;
                    break;

                case StatusEffect.StatusType.Paralysis:
                    CombatLog.Instance?.Log(
                        $"<color=#FFD700>{characterName} is paralysed! (speed -{s.potency})</color>");
                    s.count--;
                    break;

                case StatusEffect.StatusType.DamageUp:
                    s.count--;
                    break;
                case StatusEffect.StatusType.DamageDown:
                    s.count--;
                    break;
                case StatusEffect.StatusType.DefenseUp:
                    s.count--;
                    break;
                case StatusEffect.StatusType.Fragile:
                    s.count--;
                    break;
                case StatusEffect.StatusType.Haste:
                    s.count--;
                    break;
                case StatusEffect.StatusType.Poise:
                    s.count--;
                    break;
            }

            if (s.IsExpired()) toRemove.Add(s);
        }

        foreach (var s in toRemove)
        {
            CombatLog.Instance?.Log(
                $"<color=#AAAAAA>{characterName}'s {s.definition.displayName} wore off.</color>");
            activeStatuses.Remove(s);
        }

        RefreshUI();
        StatusDisplayManager.Instance?.Refresh(this);
    }

    // Called once per attack action — Bleed deals damage here, once regardless of hit count
    public void ProcessOnAttackStatuses()
    {
        List<ActiveStatus> toRemove = new List<ActiveStatus>();

        foreach (var s in activeStatuses)
        {
            if (s.definition.type == StatusEffect.StatusType.Bleed)
            {
                health -= s.potency;
                health  = Mathf.Max(0, health);
                CombatLog.Instance?.Log(
                    $"<color=#CC0000>{characterName} bleeds for {s.potency}!</color>");
                s.count--;
                if (s.IsExpired()) toRemove.Add(s);
            }
        }

        foreach (var s in toRemove)
        {
            CombatLog.Instance?.Log(
                $"<color=#AAAAAA>{characterName}'s {s.definition.displayName} wore off.</color>");
            activeStatuses.Remove(s);
        }

        RefreshUI();
        StatusDisplayManager.Instance?.Refresh(this);
    }

    public bool CheckCrippled()
    {
        if (isDefending) return false;
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            if (activeStatuses[i].definition.type == StatusEffect.StatusType.Crippled)
            {
                CombatLog.Instance?.Log(
                    $"<color=#9B59B6>{characterName} is crippled and loses their turn!</color>");
                activeStatuses.RemoveAt(i);
                StatusDisplayManager.Instance?.Refresh(this);
                return true;
            }
        }
        return false;
    }
}