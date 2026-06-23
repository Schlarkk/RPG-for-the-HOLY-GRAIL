using UnityEngine;

[System.Serializable]
public class ActiveStatus
{
    public StatusEffect definition;
    public int potency;
    public int count;

    public ActiveStatus(StatusEffect def, int potency, int count)
    {
        definition = def;
        this.potency = Mathf.Clamp(potency, 1, def.maxPotency);
        this.count   = Mathf.Clamp(count,   1, def.maxCount);
    }

    public bool IsExpired() => count <= 0;
}