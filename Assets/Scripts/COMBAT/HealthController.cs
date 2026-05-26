using Unity.VisualScripting;
using UnityEditor;


using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
   public float healthNumb;
   public int maxHealthNumb;
   public Image HealthImage;

   public int armorNumb;
    
    void Awake()
    {
        
    }
   
    void Start()
    {
        healthNumb = maxHealthNumb ; // Health starts as max health
    }

    
    void Update()
    {
        MinHealth();
    }

    public void MinHealth()
    {
        if(healthNumb <= 0f)
        {
            healthNumb = 0f;
        }
    }
}
