using UnityEngine;

public class ToolPickup : MonoBehaviour, IInteractable
{
    public ToolData toolToGive;

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null && toolToGive != null)
        {
            inventory.AddTool(toolToGive);
            gameObject.SetActive(false); // picked up, remove it from the scene
        }
    }

    public string GetPrompt()
    {
        return toolToGive != null ? "Press E to pick up " + toolToGive.toolName : "Press E to pick up";
    }
}