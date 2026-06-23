using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatLog : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public ScrollRect scrollRect;
    public GameObject logPanel;

    public int maxLines = 12;

    // Use a simple list instead of Queue + string.Join every call
    List<string> lines = new List<string>();
    bool scrollPending = false;

    public static CombatLog Instance;

    void Awake() => Instance = this;

    public void Log(string message)
    {
        lines.Add(message);
        if (lines.Count > maxLines)
            lines.RemoveAt(0);

        // Build text only once per log call
        logText.text = string.Join("\n", lines);

        if (!scrollPending)
            StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        scrollPending = true;
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
        scrollPending = false;
    }

    public void LogAttack(string attacker, string target, string attackName,
                          int damage, float typeMult)
    {
        string effectiveness = typeMult >= 2f   ? " <color=#AAD4F5>(super effective!)</color>"
                             : typeMult <= 0.5f ? " <color=#AAD4F5>(not very effective...)</color>"
                             : "";
        Log($"<color=#FFFFFF>{attacker}</color> hit <color=#FFFFFF>{target}</color> " +
            $"with <color=#FFD966>{attackName}</color> " +
            $"for <color=#FF6B6B>{damage}</color> dmg!{effectiveness}");
    }

    public void LogDefend(string characterName)
    {
        Log($"<color=#80C4FF>{characterName}</color> takes a defensive stance.");
    }

    public void LogKO(string characterName)
    {
        Log($"<color=#FF4444>{characterName}</color> has been knocked out!");
    }

    public void LogRoundStart(int round)
    {
        Log($"<color=#FFD966>── Round {round} ──</color>");
    }
}