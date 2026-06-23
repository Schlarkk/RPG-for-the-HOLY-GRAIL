using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenUi : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Button returnButton;
    public TextMeshProUGUI returnButtonLabel;

    [Header("Colours")]
    public Color victoryColour = new Color(1f, 0.85f, 0.1f);
    public Color defeatColour  = new Color(0.8f, 0.1f, 0.1f);

    string targetScene;

    void Awake() => panel.SetActive(false);

    public void Show(bool victory, string sceneName)
    {
        targetScene = sceneName;
        panel.SetActive(true);

        if (victory)
        {
            titleText.text    = "Victory!";
            titleText.color   = victoryColour;
            subtitleText.text = "The enemy has been defeated.";
        }
        else
        {
            titleText.text    = "Defeated...";
            titleText.color   = defeatColour;
            subtitleText.text = "Your party has fallen.";
        }

        returnButtonLabel.text = "Return to Menu";
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(ReturnToMenu);
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene(targetScene);
    }
}