using UnityEngine;
using UnityEngine.UI;

public class SpeedValue : MonoBehaviour
{
    public int currentSpeed;

    Text txt;

    [SerializeField]int lowestSpeed;
    [SerializeField]int HighestSpeed;

    void Start()
    {
        txt = GetComponent<Text>();
    }

    void Update()
    {
        txt.text = currentSpeed.ToString();
    }


    public void CalculateSpeed()
    {
        currentSpeed = Random.Range(lowestSpeed, HighestSpeed);
    }
}
