using UnityEngine;
using UnityEngine.InputSystem;

public class Toolbox : MonoBehaviour
{
    [Header("Panel")]
    public GameObject toolSelectionPanel;

    [Header("Tools")]
    public GameObject dehumidifier;
    public GameObject vacuum;
    public GameObject laserLevel;
    public GameObject floorGrinder;
    public GameObject floorScraper;
    public GameObject rubberMallet;

    [Header("Moisture Meter UI")]
    public GameObject moistureMeterUI;

    [Header("Tool Drop Position")]
    public Transform toolDropPoint;

    [Header("Tool Spacing")]
    public float toolSpacing = 0.8f;

    private bool isMoving = false;

    void Start()
    {
        // Toolbox panel starts closed
        toolSelectionPanel.SetActive(false);

        // Moisture Meter UI starts hidden
        if (moistureMeterUI != null)
        {
            moistureMeterUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    -Camera.main.transform.position.z
                )
            );

        // LEFT CLICK = OPEN / CLOSE TOOLBOX
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForClick(worldPosition);
        }

        // RIGHT CLICK = START MOVING TOOLBOX
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Collider2D[] hits =
                Physics2D.OverlapPointAll(
                    new Vector2(
                        worldPosition.x,
                        worldPosition.y
                    )
                );

            foreach (Collider2D hit in hits)
            {
                Toolbox toolbox =
                    hit.GetComponentInParent<Toolbox>();

                if (toolbox == this)
                {
                    isMoving = true;
                    break;
                }
            }
        }

        // MOVE TOOLBOX
        if (isMoving &&
            Mouse.current.rightButton.isPressed)
        {
            MoveToMouse(worldPosition);
        }

        // STOP MOVING
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;
        }
    }

    // -------------------------
    // TOOLBOX CLICK
    // -------------------------

    void CheckForClick(Vector3 worldPosition)
    {
        Collider2D[] hits =
            Physics2D.OverlapPointAll(
                new Vector2(
                    worldPosition.x,
                    worldPosition.y
                )
            );

        foreach (Collider2D hit in hits)
        {
            Toolbox toolbox =
                hit.GetComponentInParent<Toolbox>();

            if (toolbox == this)
            {
                toolSelectionPanel.SetActive(
                    !toolSelectionPanel.activeSelf
                );

                Debug.Log(
                    "TOOLBOX OPENED/CLOSED!"
                );

                return;
            }
        }
    }

    // -------------------------
    // MOVE TOOLBOX
    // -------------------------

    void MoveToMouse(Vector3 worldPosition)
    {
        worldPosition.z =
            transform.position.z;

        transform.position =
            worldPosition;
    }

    // -------------------------
    // CLOSE PANEL BUTTON
    // -------------------------

    public void CloseToolPanel()
    {
        toolSelectionPanel.SetActive(false);

        Debug.Log(
            "TOOLBOX PANEL CLOSED!"
        );
    }

    // -------------------------
    // TOOL BUTTONS
    // -------------------------

    public void SelectDehumidifier()
    {
        PlaceTool(dehumidifier);
    }

    public void SelectVacuum()
    {
        PlaceTool(vacuum);
    }

    public void SelectLaserLevel()
    {
        PlaceTool(laserLevel);
    }

    public void SelectFloorGrinder()
    {
        PlaceTool(floorGrinder);
    }

    public void SelectFloorScraper()
    {
        PlaceTool(floorScraper);
    }

    public void SelectRubberMallet()
    {
        PlaceTool(rubberMallet);
    }

    // -------------------------
    // MOISTURE METER
    // -------------------------

    public void SelectMoistureMeter()
    {
        // Show Moisture Meter UI
        if (moistureMeterUI != null)
        {
            moistureMeterUI.SetActive(true);
        }

        // Close toolbox panel
        toolSelectionPanel.SetActive(false);

        Debug.Log(
            "MOISTURE METER SELECTED!"
        );
    }

    // -------------------------
    // PLACE TOOL
    // -------------------------

    void PlaceTool(GameObject tool)
    {
        // Selecting any physical tool
        // hides Moisture Meter UI
        if (moistureMeterUI != null)
        {
            moistureMeterUI.SetActive(false);
        }

        if (tool == null)
        {
            Debug.LogWarning(
                "Tool reference is missing."
            );

            return;
        }

        if (toolDropPoint == null)
        {
            Debug.LogWarning(
                "Tool Drop Point is missing."
            );

            return;
        }

        // If this tool is already on the floor,
        // don't create another copy
        if (tool.activeSelf)
        {
            Debug.Log(
                tool.name +
                " is already on the floor."
            );

            toolSelectionPanel.SetActive(false);

            return;
        }

        // Activate tool
        tool.SetActive(true);

        // Find free position
        Vector3 spawnPosition =
            FindFreeToolPosition();

        tool.transform.position =
            spawnPosition;

        // Close toolbox panel
        toolSelectionPanel.SetActive(false);

        Debug.Log(
            tool.name +
            " selected at position: " +
            spawnPosition
        );
    }

    // -------------------------
    // FIND FREE TOOL POSITION
    // -------------------------

    Vector3 FindFreeToolPosition()
    {
        Vector3 position =
            toolDropPoint.position;

        int positionNumber = 0;

        while (IsToolAtPosition(position))
        {
            positionNumber++;

            position =
                toolDropPoint.position;

            position.x +=
                positionNumber * toolSpacing;
        }

        return position;
    }

    // -------------------------
    // CHECK TOOL POSITION
    // -------------------------

    bool IsToolAtPosition(Vector3 position)
    {
        float checkRadius = 0.35f;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                position,
                checkRadius
            );

        foreach (Collider2D hit in hits)
        {
            if (IsOurTool(hit.gameObject))
            {
                return true;
            }
        }

        return false;
    }

    // -------------------------
    // CHECK OUR TOOLS
    // -------------------------

    bool IsOurTool(GameObject objectHit)
    {
        if (objectHit == dehumidifier ||
            objectHit == vacuum ||
            objectHit == laserLevel ||
            objectHit == floorGrinder ||
            objectHit == floorScraper ||
            objectHit == rubberMallet)
        {
            return true;
        }

        return false;
    }
}