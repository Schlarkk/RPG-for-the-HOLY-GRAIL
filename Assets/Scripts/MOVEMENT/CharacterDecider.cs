using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterDecider : MonoBehaviour
{
    public enum Characters
    {
        ghost,
        cerberus,
        skeleton
    }

    [SerializeField] public Characters currentCharacter;

    public Sprite ghostF;
    public Sprite ghostB;
    public Sprite ghostM;

    public Sprite CerbF;
    public Sprite CerbB;
    public Sprite CerbM;

    public Sprite skelF;
    public Sprite skelB;
    public Sprite skelM;


    PlayerMovement pm;

    void Start()
    {
        pm = gameObject.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        switch(currentCharacter)
        {
            case Characters.ghost:
            AssaignSprites(ghostF, ghostB, ghostM);
            pm.s.sprite = ghostB;
            break;
            case Characters.cerberus:
            AssaignSprites(CerbF, CerbB, CerbM);
            pm.s.sprite = CerbB;
            break;
            case Characters.skeleton:
            AssaignSprites(skelF, skelB, skelM);
            pm.s.sprite = skelB;
            break;
        }
    }

    void AssaignSprites(Sprite forward, Sprite backward, Sprite move)
    {
        pm.forward = forward;
        pm.backward = backward;
        pm.move = move;
    }
}
