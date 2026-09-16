using UnityEngine;

[CreateAssetMenu(fileName = "NewTool", menuName = "Tool Data")]
public class ToolData : ScriptableObject
{
    public string toolName;
    public Sprite icon;
    public string description;
}