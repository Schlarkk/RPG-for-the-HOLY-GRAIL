using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject attackListPanel;
    public GameObject targetPanel;
    public GameObject messagePanel;

    [Header("Attack Buttons (exactly 4)")]
    public Button[] attackButtons;
    public TextMeshProUGUI[] attackButtonLabels;

    [Header("Target Buttons")]
    public Transform targetButtonContainer;
    public Button targetButtonPrefab;

    [Header("Turn Order Strip")]
    public Transform turnOrderStrip;
    public TextMeshProUGUI turnOrderEntryPrefab;

    [Header("Message Text")]
    public TextMeshProUGUI messageText;

    [Header("Back Buttons")]
    public Button backFromAttackList;   // ← wire in Inspector: sits inside AttackListPanel
    public Button backFromTargetPanel;  // ← wire in Inspector: sits inside TargetPanel

    CombatManager combatManager;

    void Awake()
    {
        combatManager = FindAnyObjectByType<CombatManager>();
        HideAll();

        // Wire back buttons here so you don't have to set onClick in the Inspector
        backFromAttackList.onClick.AddListener(OnBackFromAttackList);
        backFromTargetPanel.onClick.AddListener(OnBackFromTargetPanel);
    }

    // ── Type colours ─────────────────────────────────────────

    Color ColourForType(PlayerAttack.AttackColour colour)
    {
        switch (colour)
        {
            case PlayerAttack.AttackColour.red:    return new Color(0.85f, 0.15f, 0.15f);
            case PlayerAttack.AttackColour.blue:   return new Color(0.15f, 0.40f, 0.85f);
            case PlayerAttack.AttackColour.green:  return new Color(0.10f, 0.65f, 0.20f);
            case PlayerAttack.AttackColour.yellow: return new Color(0.90f, 0.78f, 0.05f);
            case PlayerAttack.AttackColour.white:  return new Color(0.90f, 0.90f, 0.90f);
            case PlayerAttack.AttackColour.black:  return new Color(0.15f, 0.10f, 0.20f);
            case PlayerAttack.AttackColour.purple: return new Color(0.50f, 0.00f, 0.50f);
            case PlayerAttack.AttackColour.pink:   return new Color(1.00f, 0.41f, 0.71f);
            case PlayerAttack.AttackColour.grey:   return new Color(0.50f, 0.50f, 0.50f);
            case PlayerAttack.AttackColour.brown:  return new Color(0.40f, 0.20f, 0.00f);
            case PlayerAttack.AttackColour.teal:   return new Color(0.00f, 0.50f, 0.50f);
            case PlayerAttack.AttackColour.orange: return new Color(0.90f, 0.45f, 0.00f);
            default:                               return Color.grey;
        }
    }

    Color TextColourForType(PlayerAttack.AttackColour colour)
    {
        switch (colour)
        {
            case PlayerAttack.AttackColour.white:
            case PlayerAttack.AttackColour.yellow:
                return new Color(0.10f, 0.10f, 0.10f);
            default:
                return Color.white;
        }
    }

    void ApplyButtonColour(Button btn, TextMeshProUGUI label, PlayerAttack.AttackColour colour)
    {
        Color bg = ColourForType(colour);
        ColorBlock cb = btn.colors;
        cb.normalColor      = bg;
        cb.highlightedColor = Color.Lerp(bg, Color.white, 0.15f);
        cb.pressedColor     = Color.Lerp(bg, Color.black, 0.20f);
        cb.selectedColor    = bg;
        btn.colors = cb;
        label.color = TextColourForType(colour);
    }

    // ── Panels ───────────────────────────────────────────────

    public void HideAll()
    {
        mainMenuPanel.SetActive(false);
        attackListPanel.SetActive(false);
        targetPanel.SetActive(false);
        messagePanel.SetActive(false);
    }

    public void ShowMainMenu(CharacterStats actor)
    {
        HideAll();
        mainMenuPanel.SetActive(true);
        // Optionally show actor name somewhere in the main menu panel
    }

    // ── Main menu buttons ────────────────────────────────────

    public void OnAttackPressed()
    {
        HideAll();
        attackListPanel.SetActive(true);
        PopulateAttackButtons();
    }

    public void OnDefensePressed()  => combatManager.PlayerChooseDefend();
    public void OnOtherPressed()    { }
    public void OnEscapePressed()
    {
        HideAll();
        messagePanel.SetActive(true);
        messageText.text = "You can't escape!";
        Invoke(nameof(ShowMainMenu), 2f);
    }

    // ── Back buttons ─────────────────────────────────────────

    // Attack list → Main menu
    void OnBackFromAttackList()
    {
        combatManager.PendingAttack = null;  // clear any half-selection
        ShowMainMenu(combatManager.CurrentActor);
    }

    // Target panel → Attack list
    void OnBackFromTargetPanel()
    {
        combatManager.PendingAttack = null;
        HideAll();
        attackListPanel.SetActive(true);
        PopulateAttackButtons();
    }

    // ── Attack list ──────────────────────────────────────────

    void PopulateAttackButtons()
    {
        List<PlayerAttack> attacks = combatManager.GetCurrentActorAttacks();
        CharacterStats actor = combatManager.CurrentActor; 

        for (int i = 0; i < attackButtons.Length; i++)
        {
            if (i < attacks.Count)
            {
                int captured = i;
                PlayerAttack atk = attacks[i];
                bool canAfford = actor.HasMP(atk.mpCost);

                attackButtons[i].gameObject.SetActive(true);
                attackButtons[i].interactable = canAfford;   // grey out if no MP

                // Label: "Fire Slash  (10 MP)" or "Fire Slash  [FREE]"
                attackButtonLabels[i].text = atk.mpCost > 0
                    ? $"{atk.attackName}  ({atk.mpCost} MP)"
                    : $"{atk.attackName}  [FREE]";

                ApplyButtonColour(attackButtons[i], attackButtonLabels[i], atk.attackColour);
                attackButtons[i].onClick.RemoveAllListeners();
                if (canAfford)
                    attackButtons[i].onClick.AddListener(() => OnAttackChosen(captured));
            }
            else
            {
                attackButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowTargetPanelPublic(PlayerAttack attack)
    {
        ShowTargetPanel(attack);
    }

    void OnAttackChosen(int index)
    {
        PlayerAttack chosen = combatManager.GetCurrentActorAttacks()[index];
        combatManager.PendingAttack = chosen;
        ShowTargetPanel(chosen);
    }

    // ── Target panel ─────────────────────────────────────────

    void ShowTargetPanel(PlayerAttack attack)
    {
        HideAll();
        targetPanel.SetActive(true);

        foreach (Transform child in targetButtonContainer)
            Destroy(child.gameObject);

        List<CharacterStats> targets = combatManager.GetValidTargets(attack);
        foreach (CharacterStats t in targets)
        {
            Button btn = Instantiate(targetButtonPrefab, targetButtonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{t.characterName}  HP:{t.health}/{t.maxHealth}";
            CharacterStats captured = t;
            btn.onClick.AddListener(() => combatManager.PlayerConfirmTarget(captured));
        }
    }

    // ── Turn order strip ─────────────────────────────────────

    public void RefreshTurnOrderStrip(List<CharacterStats> ordered, int currentIndex)
    {
        foreach (Transform child in turnOrderStrip)
            Destroy(child.gameObject);

        for (int i = 0; i < ordered.Count; i++)
        {
            var entry = Instantiate(turnOrderEntryPrefab, turnOrderStrip);
            entry.text      = ordered[i].characterName;
            entry.color     = Color.white;
            entry.fontStyle = (i == currentIndex)
                ? TMPro.FontStyles.Bold
                : TMPro.FontStyles.Normal;

            // Scale the active entry up slightly
            entry.transform.localScale = (i == currentIndex)
                ? new Vector3(1.3f, 1.3f, 1f)
                : Vector3.one;
        }
    }
}