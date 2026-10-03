using UnityEngine;
using UnityEngine.InputSystem;

public class Vacuum : MonoBehaviour
{
    [Header("Vacuum Light")]
    public SpriteRenderer nozzleLight;

    private bool isUsing = false;
    private bool isMoving = false;

    private BoxCollider2D vacuumCollider;

    void Start()
    {
        nozzleLight.enabled = false;
        vacuumCollider = GetComponent<BoxCollider2D>();
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

        // RIGHT CLICK + HOLD DIRECTLY ON VACUUM
        // = ACTIVATE + MOVE
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(
                new Vector2(worldPosition.x, worldPosition.y)
            );

            foreach (Collider2D hit in hits)
            {
                Vacuum vacuum =
                    hit.GetComponentInParent<Vacuum>();

                if (vacuum == this)
                {
                    isUsing = true;
                    isMoving = true;

                    Debug.Log("VACUUM CLICKED!");
                    break;
                }
            }
        }

        // MOVE + CLEAN
        if (isMoving && Mouse.current.rightButton.isPressed)
        {
            MoveToMouse(worldPosition);

            nozzleLight.enabled = true;

            CleanFloor();
        }

        // RELEASE = STOP + CHECK TOOLBOX
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;
            isUsing = false;

            nozzleLight.enabled = false;

            Debug.Log("Vacuum: OFF");

            CheckReturnToToolbox();
        }
    }

    void MoveToMouse(Vector3 worldPosition)
    {
        worldPosition.z = transform.position.z;

        transform.position = worldPosition;
    }

    void CleanFloor()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            vacuumCollider.bounds.center,
            vacuumCollider.bounds.size,
            transform.eulerAngles.z
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.name == "Dirt")
            {
                hit.gameObject.SetActive(false);

                Debug.Log("Dirt cleaned!");
            }
            else if (hit.gameObject.name == "Water")
            {
                hit.gameObject.SetActive(false);

                Debug.Log("Water cleaned!");
            }
        }
    }

    void CheckReturnToToolbox()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(
            transform.position
        );

        foreach (Collider2D hit in hits)
        {
            Toolbox toolbox =
                hit.GetComponentInParent<Toolbox>();

            if (toolbox != null)
            {
                gameObject.SetActive(false);

                Debug.Log(
                    "VACUUM RETURNED TO TOOLBOX!"
                );

                return;
            }
        }
    }
}