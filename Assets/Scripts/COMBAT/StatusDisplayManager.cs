using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusDisplayManager : MonoBehaviour
{
    public static StatusDisplayManager Instance;

    [System.Serializable]
    public class CharacterStatusUI
    {
        public CharacterStats character;
        public Transform iconContainer;     // Horizontal Layout Group next to their health bar
    }

    public List<CharacterStatusUI> characterUIs;
    public GameObject statusIconPrefab;     // small panel with Image + TMP label

    void Awake() => Instance = this;

    public void Refresh(CharacterStats cs)
    {
        CharacterStatusUI ui = characterUIs.Find(c => c.character == cs);
        if (ui == null) return;

        // Clear existing icons
        foreach (Transform child in ui.iconContainer)
            Destroy(child.gameObject);

        // Spawn one icon per active status
        foreach (var status in cs.activeStatuses)
        {
            GameObject icon = Instantiate(statusIconPrefab, ui.iconContainer);

            // Background colour = status colour
            Image bg = icon.GetComponentInChildren<Image>();
            if (bg != null) bg.color = status.definition.displayColour;

            // Label: abbreviated name + count, e.g. "BRN x3"
            TextMeshProUGUI label = icon.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = $"{status.potency}.{Abbrev(status.definition.type)} x{status.count}";
        }
    }

    string Abbrev(StatusEffect.StatusType type)
    {
        switch (type)
        {
            case StatusEffect.StatusType.Burn:      return "BRN";
            case StatusEffect.StatusType.Bleed:     return "BLD";
            case StatusEffect.StatusType.Paralysis: return "PAR";
            case StatusEffect.StatusType.Crippled:  return "CRP";
            case StatusEffect.StatusType.Rupture: return "RPT";
            case StatusEffect.StatusType.Haste: return "HST";
            case StatusEffect.StatusType.DamageUp: return "DMGUP";
            case StatusEffect.StatusType.DamageDown: return "DMGDWN";
            case StatusEffect.StatusType.DefenseUp: return "DFNSUP";
            case StatusEffect.StatusType.Fragile: return "FRG";
            case StatusEffect.StatusType.Regeneration: return "RGN";
            case StatusEffect.StatusType.Cleanse: return "CLN";
            case StatusEffect.StatusType.Poise: return "POIS";
            default:                                return "???";
        }
    }
}