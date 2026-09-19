using UnityEngine;

public class FloorProblem : MonoBehaviour, IInteractable
{
    public string problemName = "Dirt Patch";
    public ToolData requiredTool;

    private bool isFixed = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Interact(GameObject interactor)
    {
        if (isFixed) return;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null && requiredTool != null && inventory.HasTool(requiredTool))
        {
            isFixed = true;
            sr.color = Color.green;
            Debug.Log(problemName + " fixed using " + requiredTool.toolName);
        }
        else
        {
            string toolNeeded = requiredTool != null ? requiredTool.toolName : "the correct tool";
            Debug.Log("You need " + toolNeeded + " to fix this " + problemName);
        }
    }

    public string GetPrompt()
    {
        if (isFixed)
        {
            return problemName + " (fixed)";
        }

        string toolNeeded = requiredTool != null ? requiredTool.toolName : "a tool";
        return "Press E to fix " + problemName + " (needs " + toolNeeded + ")";
    }
}
