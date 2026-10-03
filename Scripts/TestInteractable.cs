using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    // Called when the player interacts with this object
    public void Interact(GameObject interactor)
    {
        Debug.Log(gameObject.name + " was interacted with by " + interactor.name);
    }

    // Text shown in the UI prompt
    public string GetPrompt()
    {
        return "Press E to inspect " + gameObject.name;
    }
}