using UnityEngine;

public class Toolbox : MonoBehaviour, IInteractable
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

    // -------------------------
    // INTERACTION (E KEY)
    // -------------------------

    public void Interact(GameObject interactor)
    {
        toolSelectionPanel.SetActive(
            !toolSelectionPanel.activeSelf
        );

        Debug.Log("TOOLBOX OPENED/CLOSED!");
    }

    public string GetPrompt()
    {
        return "Press E to open Toolbox";
    }

    // -------------------------
    // CLOSE PANEL BUTTON
    // -------------------------

    public void CloseToolPanel()
    {
        toolSelectionPanel.SetActive(false);

        Debug.Log("TOOLBOX PANEL CLOSED!");
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
        if (moistureMeterUI != null)
        {
            moistureMeterUI.SetActive(true);
        }

        toolSelectionPanel.SetActive(false);

        Debug.Log("MOISTURE METER SELECTED!");
    }

    // -------------------------
    // PLACE TOOL
    // -------------------------

    void PlaceTool(GameObject tool)
    {
        if (moistureMeterUI != null)
        {
            moistureMeterUI.SetActive(false);
        }

        if (tool == null)
        {
            Debug.LogWarning("Tool reference is missing.");
            return;
        }

        if (toolDropPoint == null)
        {
            Debug.LogWarning("Tool Drop Point is missing.");
            return;
        }

        if (tool.activeSelf)
        {
            Debug.Log(tool.name + " is already on the floor.");
            toolSelectionPanel.SetActive(false);
            return;
        }

        tool.SetActive(true);

        Vector3 spawnPosition = FindFreeToolPosition();
        tool.transform.position = spawnPosition;

        toolSelectionPanel.SetActive(false);

        Debug.Log(tool.name + " selected at position: " + spawnPosition);
    }

    // -------------------------
    // FIND FREE TOOL POSITION
    // -------------------------

    Vector3 FindFreeToolPosition()
    {
        Vector3 position = toolDropPoint.position;
        int positionNumber = 0;

        while (IsToolAtPosition(position))
        {
            positionNumber++;
            position = toolDropPoint.position;
            position.x += positionNumber * toolSpacing;
        }

        return position;
    }

    // -------------------------
    // CHECK TOOL POSITION
    // -------------------------

    bool IsToolAtPosition(Vector3 position)
    {
        float checkRadius = 0.35f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, checkRadius);

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