using TMPro;
using UnityEngine;

public class TMPTextCopy : MonoBehaviour
{
    [SerializeField] private TMP_Text sourceText;
    [SerializeField] private TMP_Text targetText;

    private string lastText;

    private void Update()
    {
        if (sourceText == null || targetText == null)
            return;

        if (sourceText.text != lastText)
        {
            lastText = sourceText.text;
            targetText.text = lastText;
        }
    }
}