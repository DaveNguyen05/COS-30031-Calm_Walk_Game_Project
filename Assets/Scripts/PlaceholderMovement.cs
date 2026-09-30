using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceholderMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [HideInInspector] public float speedMultiplier = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 input = Vector2.zero;
    private string facing = "Down";
    private string currentState = "";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        var kb = Keyboard.current;

        if (kb == null)
            return;

        input = Vector2.zero;

        if (kb.wKey.isPressed) input.y += 1;
        if (kb.sKey.isPressed) input.y -= 1;
        if (kb.dKey.isPressed) input.x += 1;
        if (kb.aKey.isPressed) input.x -= 1;

        if (input.x < 0) facing = "Left";
        else if (input.x > 0) facing = "Right";
        else if (input.y > 0) facing = "Up";
        else if (input.y < 0) facing = "Down";

        string action = (input != Vector2.zero) ? "Walk_" : "Idle_";

        PlayState(action + facing);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input.normalized * moveSpeed * speedMultiplier * Time.fixedDeltaTime);
    }

    void PlayState(string stateName)
    {
        if (stateName == currentState)
            return;

        animator.Play(stateName);
        currentState = stateName;
    }
}