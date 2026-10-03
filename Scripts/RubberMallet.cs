using UnityEngine;
using UnityEngine.InputSystem;

public class RubberMallet : MonoBehaviour
{
    private bool isUsing = false;
    private bool isMoving = false;

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

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(
                new Vector2(worldPosition.x, worldPosition.y)
            );

            foreach (Collider2D hit in hits)
            {
                RubberMallet mallet =
                    hit.GetComponentInParent<RubberMallet>();

                if (mallet == this)
                {
                    isUsing = true;
                    isMoving = true;

                    Debug.Log("RUBBER MALLET CLICKED!");
                    break;
                }
            }
        }

        if (isMoving && Mouse.current.rightButton.isPressed)
        {
            MoveToMouse(worldPosition);

            if (isUsing)
            {
                InstallFlooring();
            }
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;
            isUsing = false;

            Debug.Log("Rubber Mallet: OFF");

            CheckReturnToToolbox();
        }
    }

    void MoveToMouse(Vector3 worldPosition)
    {
        worldPosition.z = transform.position.z;
        transform.position = worldPosition;
    }

    // -------------------------
    // INSTALL NEW FLOORING
    // -------------------------

    void InstallFlooring()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            GetComponent<BoxCollider2D>().bounds.size,
            transform.eulerAngles.z
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.name.StartsWith("NewFloor"))
            {
                SpriteRenderer sr = hit.gameObject.GetComponent<SpriteRenderer>();

                if (sr != null && !sr.enabled)
                {
                    sr.enabled = true;
                    Debug.Log("New floor installed!");
                }
            }
        }
    }

    void CheckReturnToToolbox()
    {
        if (isUsing)
        {
            Debug.Log("RUBBER MALLET IS ON - CANNOT RETURN!");
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

                Debug.Log("RUBBER MALLET RETURNED TO TOOLBOX!");

                return;
            }
        }
    }
}