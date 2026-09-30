using UnityEngine;

public class FloorEffects : MonoBehaviour
{
    [Header("Dirt Slow")]
    [Range(0f, 1f)]
    public float dirtSpeedMultiplier = 0.5f;

    [Header("Water / Moisture Speed Boost")]
    public float waterSpeedMultiplier = 1.5f;

    private PlaceholderMovement movement;

    void Awake()
    {
        movement = GetComponent<PlaceholderMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string name = other.gameObject.name;

        if (name.Contains("Dirt"))
        {
            movement.speedMultiplier = dirtSpeedMultiplier;
        }
        else if (name.Contains("Water") || name.Contains("Moisture"))
        {
            movement.speedMultiplier = waterSpeedMultiplier;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        string name = other.gameObject.name;

        if (name.Contains("Dirt") || name.Contains("Water") || name.Contains("Moisture"))
        {
            movement.speedMultiplier = 1f;
        }
    }
}