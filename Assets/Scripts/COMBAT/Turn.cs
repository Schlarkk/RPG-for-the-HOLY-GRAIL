using System.Collections.Concurrent;
using UnityEngine;

public class Turn : MonoBehaviour
{
    
    public enum gameState
    {
     PlayerTurn,
     EnemyTurn   
    }

    public gameState Currentstate;
    public CheckOrder checkOrder;
        
    
    void Start()
    {
        checkOrder=GetComponent<CheckOrder>();
    }

    
    void Update()
    {
        
    }

    private void ActiveTurn()
    {
        //if()
        {
            
        }
    }
}
