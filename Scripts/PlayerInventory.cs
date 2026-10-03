using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public List<ToolData> heldTools = new List<ToolData>();
    private bool isOpen = false;

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            isOpen = !isOpen;
            Debug.Log("Inventory " + (isOpen ? "opened" : "closed"));
        }
    }

    public void AddTool(ToolData tool)
    {
        heldTools.Add(tool);
        Debug.Log("Picked up: " + tool.toolName);
    }

    public bool HasTool(ToolData tool)
    {
        return heldTools.Contains(tool);
    }
}