using UnityEngine;
using UnityEngine.InputSystem;

public class LaserLevel : MonoBehaviour
{
    public GameObject laserLight;

    public GameObject unevenHighlight1;
    public GameObject unevenHighlight2;
    public GameObject unevenHighlight3;

    private bool uneven1Fixed = false;
    private bool uneven2Fixed = false;
    private bool uneven3Fixed = false;

    private bool isOn = false;
    private bool isMoving = false;

    void Start()
    {
        laserLight.SetActive(false);
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

        // LEFT CLICK = ON / OFF
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForClick(worldPosition);
        }

        // RIGHT CLICK = MOVE ONLY IF CLICKED ON LASER LEVEL
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(
                new Vector2(worldPosition.x, worldPosition.y)
            );

            foreach (Collider2D hit in hits)
            {
                if (hit.GetComponent<LaserLevel>() == this)
                {
                    isMoving = true;

                    // Moving automatically turns Laser Level OFF
                    isOn = false;
                    laserLight.SetActive(false);

                    HideHighlights();

                    break;
                }
            }
        }

        // MOVE WHILE RIGHT CLICK IS HELD
        if (isMoving && Mouse.current.rightButton.isPressed)
        {
            MoveToMouse(worldPosition);
        }

        // STOP MOVING WHEN RIGHT CLICK IS RELEASED
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;

            CheckReturnToToolbox();
        }
    }

    // -------------------------
    // LEFT CLICK
    // -------------------------

    void CheckForClick(Vector3 worldPosition)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(
            new Vector2(worldPosition.x, worldPosition.y)
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<LaserLevel>() == this)
            {
                isOn = !isOn;

                laserLight.SetActive(isOn);

                if (isOn)
                {
                    if (!GameObject.Find("UnevenFloor1"))
                        uneven1Fixed = true;

                    if (!GameObject.Find("UnevenFloor2"))
                        uneven2Fixed = true;

                    if (!GameObject.Find("UnevenFloor3"))
                        uneven3Fixed = true;
                }

                if (!uneven1Fixed)
                    unevenHighlight1.SetActive(isOn);

                if (!uneven2Fixed)
                    unevenHighlight2.SetActive(isOn);

                if (!uneven3Fixed)
                    unevenHighlight3.SetActive(isOn);

                Debug.Log(
                    "Laser Level: " +
                    (isOn ? "ON" : "OFF")
                );

                return;
            }
        }
    }

    // -------------------------
    // HIDE HIGHLIGHTS
    // -------------------------

    void HideHighlights()
    {
        if (unevenHighlight1 != null)
            unevenHighlight1.SetActive(false);

        if (unevenHighlight2 != null)
            unevenHighlight2.SetActive(false);

        if (unevenHighlight3 != null)
            unevenHighlight3.SetActive(false);
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
    // RETURN TO TOOLBOX
    // -------------------------

    void CheckReturnToToolbox()
    {
        // Cannot return while Laser Level is ON
        if (isOn)
        {
            Debug.Log(
                "LASER LEVEL IS ON - CANNOT RETURN TO TOOLBOX!"
            );

            return;
        }

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
                    "LASER LEVEL RETURNED TO TOOLBOX!"
                );

                return;
            }
        }
    }
}