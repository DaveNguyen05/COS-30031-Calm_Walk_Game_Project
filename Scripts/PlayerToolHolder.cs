using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerToolHolder : MonoBehaviour
{
    [Header("Which tool each hotbar slot (1-9) holds")]
    public ToolType[] slotTools = new ToolType[9];

    public ToolType CurrentTool { get; private set; } = ToolType.None;

    void Update()
    {
        var kb = Keyboard.current;

        if (kb == null)
            return;

        if (kb.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (kb.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (kb.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (kb.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (kb.digit5Key.wasPressedThisFrame) SelectSlot(4);
        if (kb.digit6Key.wasPressedThisFrame) SelectSlot(5);
        if (kb.digit7Key.wasPressedThisFrame) SelectSlot(6);
        if (kb.digit8Key.wasPressedThisFrame) SelectSlot(7);
        if (kb.digit9Key.wasPressedThisFrame) SelectSlot(8);
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= slotTools.Length)
            return;

        CurrentTool = slotTools[index];
        Debug.Log("Equipped: " + CurrentTool);
    }
}