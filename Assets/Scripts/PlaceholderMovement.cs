using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceholderMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var kb = Keyboard.current;
        Vector2 input = Vector2.zero;

        if (kb.wKey.isPressed) input.y += 1;
        if (kb.sKey.isPressed) input.y -= 1;
        if (kb.dKey.isPressed) input.x += 1;
        if (kb.aKey.isPressed) input.x -= 1;

        rb.MovePosition(rb.position + input.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}