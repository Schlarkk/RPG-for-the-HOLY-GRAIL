using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack")]
public class PlayerAttack : ScriptableObject
{
    public string attackName;
    public int attackPower;
    public bool canTargetAllies;
    public bool canTargetEnemies;

    public enum AttackType   { Physical, Special }
    public enum AttackColour { red, blue, green, yellow, white, black, purple, pink, grey, brown, teal, orange }

    public AttackType   attackType;
    public AttackColour attackColour;

    [Header("MP")]
    public int mpCost;

    [Header("Multi-Hit")]
    [Min(1)] public int hitCount = 1;   // how many times this attack hits

    [Header("Charge / Setup")]
    public bool requiresCharge;         // if true, skip turn then act next round

    [Header("Status Infliction — On Target (multiple allowed)")]
    public StatusInfliction[] statusesOnTarget;

    [Header("Status Infliction — On Self After Use (multiple allowed)")]
    public StatusInfliction[] statusesOnSelf;

    [TextArea] public string description;
}

[System.Serializable]
public class StatusInfliction
{
    public StatusEffect statusToInflict;
    public int potency;
    public int count;
}