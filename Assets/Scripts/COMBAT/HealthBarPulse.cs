using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPulse : MonoBehaviour
{
    public CharacterStats cs;
    public Image fillImage;         // drag the Fill image of the Slider here

    [Header("Settings")]
    [Range(0f, 1f)]
    public float lowHPThreshold = 0.25f;  // pulse when HP drops below 25%

    public Color normalColor   = new Color(0.18f, 0.80f, 0.44f); // green
    public Color lowColor      = new Color(0.90f, 0.20f, 0.20f); // red
    public float pulseSpeed    = 2.5f;
    public float pulseMinAlpha = 0.4f;

    bool isPulsing = false;

    void Update()
    {
        if (cs == null || fillImage == null) return;

        float ratio = (float)cs.health / cs.maxHealth;
        bool shouldPulse = ratio <= lowHPThreshold && cs.IsAlive();

        if (shouldPulse)
        {
            isPulsing = true;
            float alpha = Mathf.Lerp(pulseMinAlpha, 1f,
                          (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            fillImage.color = new Color(lowColor.r, lowColor.g, lowColor.b, alpha);
        }
        else
        {
            isPulsing = false;
            fillImage.color = normalColor;
        }
    }
}