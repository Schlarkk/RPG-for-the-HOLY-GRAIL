using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    public float maxSpeed = 5f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    Vector2 moveInput;
    Vector2 currentVelocity;

    KeyCode[] directionKeys = { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D};
    List<KeyCode> heldKeys = new List<KeyCode>();


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
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

        moveInput = GetDirectionFromLastKey();
    }

    Vector2 GetDirectionFromLastKey()
    {
        for (int i = heldKeys.Count - 1; i >= 0; i--)
        {
            switch(heldKeys[i])
            {
                case KeyCode.W: return Vector2.up;
                case KeyCode.S: return Vector2.down;
                case KeyCode.A: return Vector2.left;
                case KeyCode.D: return Vector2.right;
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
