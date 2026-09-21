using UnityEngine;
using UnityEngine.InputSystem;

public class FloorScraper : MonoBehaviour
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

        // RIGHT CLICK + HOLD DIRECTLY ON SCRAPER
        // = ACTIVATE + MOVE
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(
                new Vector2(worldPosition.x, worldPosition.y)
            );

            foreach (Collider2D hit in hits)
            {
                FloorScraper scraper =
                    hit.GetComponentInParent<FloorScraper>();

                if (scraper == this)
                {
                    isUsing = true;
                    isMoving = true;

                    Debug.Log("FLOOR SCRAPER CLICKED!");
                    break;
                }
            }
        }

        // MOVE + SCRAPE
        if (isMoving && Mouse.current.rightButton.isPressed)
        {
            MoveToMouse(worldPosition);

            if (isUsing)
            {
                ScrapeOldFloor();
            }
        }

        // RELEASE = STOP
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;
            isUsing = false;

            Debug.Log("Floor Scraper: OFF");

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
    // SCRAPE OLD FLOOR
    // -------------------------

    void ScrapeOldFloor()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            GetComponent<BoxCollider2D>().bounds.size,
            transform.eulerAngles.z
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.name.StartsWith("OldFloor"))
            {
                hit.gameObject.SetActive(false);

                Debug.Log("Old floor removed!");
            }
        }
    }

    // -------------------------
    // RETURN TO TOOLBOX
    // -------------------------

    void CheckReturnToToolbox()
    {
        // Scraper is OFF after releasing right-click,
        // so it can be returned to the Toolbox.
        if (isUsing)
        {
            Debug.Log(
                "FLOOR SCRAPER IS ON - CANNOT RETURN!"
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
                    "FLOOR SCRAPER RETURNED TO TOOLBOX!"
                );

                return;
            }
        }
    }
}