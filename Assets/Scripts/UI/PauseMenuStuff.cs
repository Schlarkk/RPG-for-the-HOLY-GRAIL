using UnityEngine;

public class PauseMenuStuff : MonoBehaviour
{

    [HideInInspector] public bool showMenu;

    public GameObject UI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        KeyBoardInput();
        Mechanics();
    }

    void KeyBoardInput()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            showMenu = !showMenu;
        }
    }

    void Mechanics()
    {
        switch(showMenu)
        {
            case true:
                UI.SetActive(true);
                break;
            case false:
                UI.SetActive(false);
                break;
        }
    }
}
