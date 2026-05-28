using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    GameObject sprite;
    [HideInInspector]public SpriteRenderer s;

    public Sprite forward;
    public Sprite backward;
    public Sprite move;

    bool turn;

    PauseMenuStuff pms;
    public GameObject ui;

    public float maxSpeed = 5f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    Vector2 moveInput;
    Vector2 currentVelocity;

    KeyCode[] directionKeys = { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D};
    List<KeyCode> heldKeys = new List<KeyCode>();


    void Start()
    {
        sprite = transform.GetChild(0).gameObject;

        rb = GetComponent<Rigidbody2D>();
        pms = ui.GetComponent<PauseMenuStuff>();

        s = sprite.GetComponent<SpriteRenderer>();
        
    }


    void Update()
    {
        if(!pms.showMenu)
            TrackInput();
    }

    void TrackInput()
    {
        foreach (KeyCode key in directionKeys)
        {
            if (Input.GetKeyDown(key) && !heldKeys.Contains(key))
            {
                heldKeys.Add(key);
            }

            if(Input.GetKeyUp(key))
            {
                heldKeys.Remove(key);
            }
        }

        if(turn)
        {
            sprite.transform.localScale = new Vector3(-3.1637f, 3.1637f, 3.1637f);
        }
        else if(!turn)
        {
            sprite.transform.localScale = new Vector3(3.1637f, 3.1637f, 3.1637f);
        }

        moveInput = GetDirectionFromLastKey();
    }

    Vector2 GetDirectionFromLastKey()
    {
        for (int i = heldKeys.Count - 1; i >= 0; i--)
        {
            switch(heldKeys[i])
            {
                case KeyCode.W: 
                s.sprite = forward;
                turn = false;
                return Vector2.up;
                case KeyCode.S:
                s.sprite = backward;
                turn = false;
                return Vector2.down;
                case KeyCode.A:
                s.sprite = move;
                turn = false;
                return Vector2.left;
                case KeyCode.D:
                s.sprite = move;
                turn = true;
                return Vector2.right;
            }
        }

        return Vector2.zero;
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * maxSpeed;

        float rate = moveInput != Vector2.zero ? acceleration : deceleration;
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);

        rb.linearVelocity = currentVelocity;
    }
}
