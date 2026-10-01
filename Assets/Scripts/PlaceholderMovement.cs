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

    private bool isKnockedBack = false;
    private Vector2 knockbackVelocity = Vector2.zero;
    private float knockbackTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isKnockedBack)
            return; // ignore WASD while being knocked back

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
        if (isKnockedBack)
        {
            rb.MovePosition(rb.position + knockbackVelocity * Time.fixedDeltaTime);

            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }

            return;
        }

        rb.MovePosition(rb.position + input.normalized * moveSpeed * speedMultiplier * Time.fixedDeltaTime);
    }

    void PlayState(string stateName)
    {
        if (stateName == currentState)
            return;

        animator.Play(stateName);
        currentState = stateName;
    }

    // Called by FloorEffects when the player touches an uneven floor patch.
    // Pushes the player away from 'source' for a short time, overriding normal movement.
    public void ApplyKnockback(Vector2 source, float force, float duration)
    {
        Vector2 direction = ((Vector2)transform.position - source).normalized;

        if (direction == Vector2.zero)
            direction = Vector2.down; // fallback if exactly overlapping

        knockbackVelocity = direction * force;
        knockbackTimer = duration;
        isKnockedBack = true;
    }
}