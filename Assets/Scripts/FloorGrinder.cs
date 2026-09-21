using UnityEngine;
using UnityEngine.InputSystem;

public class FloorGrinder : MonoBehaviour
{
    [Header("Grinder Light")]
    public SpriteRenderer GrinderLight;

    private bool isUsing = false;
    private bool isMoving = false;

    private BoxCollider2D grinderCollider;

    void Start()
    {
        GrinderLight.enabled = false;
        grinderCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                -Camera.main.transform.position.z
            )
        );

        // LEFT CLICK ON GRINDER = ON / OFF
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D[] hits =
                Physics2D.OverlapPointAll(worldPosition);

            foreach (Collider2D hit in hits)
            {
                if (hit.GetComponent<FloorGrinder>() == this)
                {
                    isUsing = !isUsing;

                    GrinderLight.enabled = isUsing;

                    Debug.Log(
                        "Floor Grinder: " +
                        (isUsing ? "ON" : "OFF")
                    );

                    return;
                }
            }
        }

        // RIGHT CLICK + HOLD ON GRINDER = MOVE
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Collider2D[] hits =
                Physics2D.OverlapPointAll(worldPosition);

            foreach (Collider2D hit in hits)
            {
                if (hit.GetComponent<FloorGrinder>() == this)
                {
                    isMoving = true;

                    break;
                }
            }
        }

        // MOVE WHILE RIGHT CLICK IS HELD
        if (isMoving &&
            Mouse.current.rightButton.isPressed)
        {
            MoveToMouse(worldPosition);

            // Only grind when grinder is ON
            if (isUsing)
            {
                LevelFloor();
            }
        }

        // STOP MOVING WHEN RIGHT CLICK IS RELEASED
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;

            CheckReturnToToolbox();
        }
    }

    // -------------------------
    // MOVE
    // -------------------------

    void MoveToMouse(Vector3 worldPosition)
    {
        worldPosition.z = transform.position.z;

        transform.position = worldPosition;
    }

    // -------------------------
    // GRIND FLOOR
    // -------------------------

    void LevelFloor()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            grinderCollider.bounds.center,
            grinderCollider.bounds.size,
            transform.eulerAngles.z
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.name.StartsWith("UnevenFloor"))
            {
                GameObject highlight = GameObject.Find(
                    hit.gameObject.name.Replace(
                        "UnevenFloor",
                        "UnevenHighlight"
                    )
                );

                if (highlight != null)
                {
                    highlight.SetActive(false);
                }

                hit.gameObject.SetActive(false);

                Debug.Log("Floor levelled!");
            }
        }
    }

    // -------------------------
    // RETURN TO TOOLBOX
    // -------------------------

    void CheckReturnToToolbox()
    {
        // Cannot return while grinder is ON
        if (isUsing)
        {
            Debug.Log(
                "FLOOR GRINDER IS ON - " +
                "CANNOT RETURN TO TOOLBOX!"
            );

            return;
        }

        Collider2D[] hits =
            Physics2D.OverlapPointAll(transform.position);

        foreach (Collider2D hit in hits)
        {
            Toolbox toolbox =
                hit.GetComponentInParent<Toolbox>();

            if (toolbox != null)
            {
                gameObject.SetActive(false);

                Debug.Log(
                    "FLOOR GRINDER RETURNED TO TOOLBOX!"
                );

                return;
            }
        }
    }
}