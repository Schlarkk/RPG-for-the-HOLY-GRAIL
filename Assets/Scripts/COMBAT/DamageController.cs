using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public float damageNumb;
   

    public int critMultiplier;

    public GameObject HealthHolder;

    public HealthController healthController;

     
    
    
    void Start()
    {

        healthController = HealthHolder.GetComponent<HealthController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        healthController.healthNumb -= damageNumb;
    }
}
