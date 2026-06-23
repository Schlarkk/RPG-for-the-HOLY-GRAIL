using UnityEngine;

public class DamageController : MonoBehaviour
{
 


     
    
    
    void Start(){}

     static float GetTypeMultiplier(PlayerAttack.AttackColour attackColour,
                                          PlayerAttack.AttackColour defenderType)

    {
        switch (defenderType)
        {
            case PlayerAttack.AttackColour.red:
                if (attackColour == PlayerAttack.AttackColour.yellow ||
                    attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.pink   ||
                    attackColour == PlayerAttack.AttackColour.orange)  return 2f;
                if (attackColour == PlayerAttack.AttackColour.blue   ||
                    attackColour == PlayerAttack.AttackColour.green  ||
                    attackColour == PlayerAttack.AttackColour.brown  ||
                    attackColour == PlayerAttack.AttackColour.purple) return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.blue:
                if (attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.purple ||
                    attackColour == PlayerAttack.AttackColour.teal)   return 2f;
                if (attackColour == PlayerAttack.AttackColour.yellow ||
                    attackColour == PlayerAttack.AttackColour.brown  ||
                    attackColour == PlayerAttack.AttackColour.orange) return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.green:
                if (attackColour == PlayerAttack.AttackColour.yellow ||
                    attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.teal)   return 2f;
                if (attackColour == PlayerAttack.AttackColour.blue   ||
                    attackColour == PlayerAttack.AttackColour.green  ||
                    attackColour == PlayerAttack.AttackColour.brown)  return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.yellow:
                if (attackColour == PlayerAttack.AttackColour.white  ||
                    attackColour == PlayerAttack.AttackColour.black   ||
                    attackColour == PlayerAttack.AttackColour.blue    ||
                    attackColour == PlayerAttack.AttackColour.brown   ||
                    attackColour == PlayerAttack.AttackColour.orange)  return 2f;
                if (attackColour == PlayerAttack.AttackColour.yellow ||
                    attackColour == PlayerAttack.AttackColour.purple) return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.white:
                if (attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.grey)   return 2f;
                if (attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.blue    ||
                    attackColour == PlayerAttack.AttackColour.green   ||
                    attackColour == PlayerAttack.AttackColour.yellow  ||
                    attackColour == PlayerAttack.AttackColour.purple  ||
                    attackColour == PlayerAttack.AttackColour.pink    ||
                    attackColour == PlayerAttack.AttackColour.brown   ||
                    attackColour == PlayerAttack.AttackColour.teal    ||
                    attackColour == PlayerAttack.AttackColour.orange)  return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.black:
                if (attackColour == PlayerAttack.AttackColour.white  ||
                    attackColour == PlayerAttack.AttackColour.grey)   return 2f;
                if (attackColour == PlayerAttack.AttackColour.black)  return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.purple:
                if (attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.blue    ||
                    attackColour == PlayerAttack.AttackColour.black   ||
                    attackColour == PlayerAttack.AttackColour.pink)   return 2f;
                if (attackColour == PlayerAttack.AttackColour.green  ||
                    attackColour == PlayerAttack.AttackColour.yellow  ||
                    attackColour == PlayerAttack.AttackColour.purple  ||
                    attackColour == PlayerAttack.AttackColour.grey    ||
                    attackColour == PlayerAttack.AttackColour.teal)   return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.pink:
                if (attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.black   ||
                    attackColour == PlayerAttack.AttackColour.purple) return 2f;
                if (attackColour == PlayerAttack.AttackColour.pink   ||
                    attackColour == PlayerAttack.AttackColour.grey)   return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.grey:
                if (attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.white   ||
                    attackColour == PlayerAttack.AttackColour.grey)   return 2f;
                if (attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.blue    ||
                    attackColour == PlayerAttack.AttackColour.green   ||
                    attackColour == PlayerAttack.AttackColour.yellow  ||
                    attackColour == PlayerAttack.AttackColour.purple  ||
                    attackColour == PlayerAttack.AttackColour.pink    ||
                    attackColour == PlayerAttack.AttackColour.brown   ||
                    attackColour == PlayerAttack.AttackColour.teal    ||
                    attackColour == PlayerAttack.AttackColour.orange)  return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.brown:
                if (attackColour == PlayerAttack.AttackColour.yellow ||
                    attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.orange) return 2f;
                if (attackColour == PlayerAttack.AttackColour.green  ||
                    attackColour == PlayerAttack.AttackColour.grey    ||
                    attackColour == PlayerAttack.AttackColour.teal)   return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.teal:
                if (attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.brown)  return 2f;
                if (attackColour == PlayerAttack.AttackColour.green  ||
                    attackColour == PlayerAttack.AttackColour.blue    ||
                    attackColour == PlayerAttack.AttackColour.grey)   return 0.5f;
                return 1f;
            case PlayerAttack.AttackColour.orange:
                if (attackColour == PlayerAttack.AttackColour.blue   ||
                    attackColour == PlayerAttack.AttackColour.black  ||
                    attackColour == PlayerAttack.AttackColour.teal)   return 2f;
                if (attackColour == PlayerAttack.AttackColour.red    ||
                    attackColour == PlayerAttack.AttackColour.yellow  ||
                    attackColour == PlayerAttack.AttackColour.purple  ||
                    attackColour == PlayerAttack.AttackColour.pink    ||
                    attackColour == PlayerAttack.AttackColour.grey    ||
                    attackColour == PlayerAttack.AttackColour.orange)  return 0.5f;
                return 1f;
        }
        return 1f;
    }

    // Single hit — no loop, no status application, just damage
    public static int CalculateSingleHit(CharacterStats attacker, CharacterStats defender,
                                         PlayerAttack attack)
    {
        int raw = Mathf.RoundToInt((attacker.baseDamage + attack.attackPower)
                                   * attacker.GetDamageMultiplier());

        float resMult  = attack.attackType == PlayerAttack.AttackType.Physical
                       ? 1f - (defender.physicalResistance / 100f)
                       : 1f - (defender.specialResistance  / 100f);

        float typeMult = GetTypeMultiplier(attack.attackColour, defender.characterType);

        float critMult = 1f;
        if (Random.Range(1, 101) <= attacker.GetEffectiveCritChance())
        {
            critMult = 2f;
            CombatLog.Instance?.Log("<color=#FFD966>Critical hit!</color>");
        }

        float defMult       = defender.isDefending ? 0.5f : 1f;
        float defStatusMult = defender.GetDefenseMultiplier();

        int damage = Mathf.RoundToInt(raw * resMult * typeMult * critMult
                                          * defMult * defStatusMult);

        // Rupture adds flat damage then ticks down
        damage += defender.GetRuptureDamage();
        TickRupture(defender);

        damage = Mathf.Max(0, damage - defender.armor);

        CombatLog.Instance?.LogAttack(
            attacker.characterName, defender.characterName,
            attack.attackName, damage, typeMult);

        return damage;
    }

    static void TickRupture(CharacterStats target)
    {
        for (int i = target.activeStatuses.Count - 1; i >= 0; i--)
        {
            var s = target.activeStatuses[i];
            if (s.definition.type == StatusEffect.StatusType.Rupture)
            {
                s.count--;
                if (s.IsExpired())
                {
                    CombatLog.Instance?.Log(
                        $"<color=#AAAAAA>{target.characterName}'s Rupture wore off.</color>");
                    target.activeStatuses.RemoveAt(i);
                }
            }
        }
        StatusDisplayManager.Instance?.Refresh(target);
    }
}