using UnityEngine;

public interface IInteractable
{
    // Called when the player interacts with this object
    void Interact(GameObject interactor);

    // Text shown in the UI prompt, e.g. "Press E to pick up"
    string GetPrompt();
}