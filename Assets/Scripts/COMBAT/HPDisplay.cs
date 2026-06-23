using UnityEngine;
using TMPro;

public class HPDisplay : MonoBehaviour
{
    public CharacterStats cs;
    public TextMeshProUGUI hpLabel;   // e.g. "HP  84 / 100"
    public TextMeshProUGUI mpLabel;   // e.g. "MP  40 / 60"  — leave empty for enemies

    void Update()
    {
        if (cs == null) return;

        if (hpLabel != null)
            hpLabel.text = $"HP  {Mathf.Max(0, cs.health)} / {cs.maxHealth}";

        if (mpLabel != null)
            mpLabel.text = $"MP  {Mathf.Max(0, cs.mp)} / {cs.maxMP}";
    }
}