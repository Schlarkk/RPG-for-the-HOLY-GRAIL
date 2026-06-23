using UnityEngine;

[CreateAssetMenu(fileName = "NewStatus", menuName = "Combat/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public enum StatusType
    {
        // Damage over time
        Burn, Bleed, Rupture,
        // Turn manipulation
        Paralysis, Crippled, Haste,
        // Damage modifiers
        DamageUp, DamageDown,
        // Defense modifiers
        DefenseUp, Fragile,
        // Healing
        Regeneration,
        // Utility
        Cleanse, Poise
    }

    public StatusType type;
    public string displayName;
    public Color displayColour;

    [Header("Caps")]
    public int maxPotency;
    public int maxCount;
}