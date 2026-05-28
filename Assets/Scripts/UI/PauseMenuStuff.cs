using UnityEngine;

public class PauseMenuStuff : MonoBehaviour
{

    [HideInInspector] public bool showMenu;

    public GameObject UI;

    public GameObject player;
    CharacterDecider cd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cd = player.GetComponent<CharacterDecider>();
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



    public void switchCharacter()
    {
        switch(cd.currentCharacter)
        {
            case CharacterDecider.Characters.ghost:
            cd.currentCharacter = CharacterDecider.Characters.cerberus;
            break;
            case CharacterDecider.Characters.cerberus:
            cd.currentCharacter = CharacterDecider.Characters.skeleton;
            break;
            case CharacterDecider.Characters.skeleton:
            cd.currentCharacter = CharacterDecider.Characters.ghost;
            break;
        }
    }






}
