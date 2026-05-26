using System.Collections.Generic;
using UnityEngine;

public class CheckOrder : MonoBehaviour
{
    public List<int> SpeedValues;

    void Start()
    {
        CheckForValues();
    }

    public void CheckForValues()
    {
        for (int i = 0; i <= 10; i++)
        {
            GameObject Child = transform.GetChild(i).gameObject;
            if (Child.GetComponent<SpeedValue>() != null)
            {
                Child.GetComponent<SpeedValue>().CalculateSpeed();

                SpeedValues.Add(Child.GetComponent<SpeedValue>().currentSpeed);
            }
            else
            {
                OrderValues();
            }
        }
    }

    void OrderValues()
    {
        SpeedValues.Sort();
    }


}
