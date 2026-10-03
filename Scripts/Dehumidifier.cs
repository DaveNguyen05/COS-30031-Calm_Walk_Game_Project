using UnityEngine;
using UnityEngine.InputSystem;

public class Dehumidifier : MonoBehaviour
{
    [Header("Humidity")]
    public float currentHumidity = 60f;
    public float requiredHumidity = 50f;

    [Header("Settings")]
    public float dryingSpeed = 1f;

    [Header("Status Light")]
    public SpriteRenderer statusLight;

    [Header("Colours")]
    public Color offColour = Color.blue;
    public Color runningColour = Color.red;
    public Color finishedColour = Color.green;

    private bool isRunning = false;
    private bool isFinished = false;

    private bool isMoving = false;

    void Start()
    {
        statusLight.color = offColour;
        Debug.Log("Dehumidifier ready.");
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        // LEFT CLICK = START DEHUMIDIFIER
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForClick();
        }

        // RIGHT CLICK = START MOVING
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CheckForMoveStart();
        }

        // MOVE WHILE RIGHT CLICK IS HELD
        if (isMoving && Mouse.current.rightButton.isPressed)
        {
            MoveToMouse();
        }

        // RELEASE RIGHT CLICK
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;

            // Check if dropped on Toolbox
            CheckReturnToToolbox();
        }

        // REDUCE HUMIDITY WHILE RUNNING
        if (isRunning && !isFinished && !isMoving)
        {
            currentHumidity -= dryingSpeed * Time.deltaTime;

            if (currentHumidity <= requiredHumidity)
            {
                currentHumidity = requiredHumidity;
                FinishDehumidifier();
            }
        }
    }

    // -------------------------
    // START DEHUMIDIFIER
    // -------------------------

    void CheckForClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, 0f)
        );

        Collider2D[] hits = Physics2D.OverlapPointAll(
            new Vector2(worldPosition.x, worldPosition.y)
        );

        foreach (Collider2D hit in hits)
        {
            Dehumidifier machine =
                hit.GetComponentInParent<Dehumidifier>();

            if (machine == this)
            {
                Debug.Log("DEHUMIDIFIER CLICKED!");
                StartDehumidifier();
                return;
            }
        }
    }

    // -------------------------
    // START MOVING
    // -------------------------

    void CheckForMoveStart()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, 0f)
        );

        Collider2D[] hits = Physics2D.OverlapPointAll(
            new Vector2(worldPosition.x, worldPosition.y)
        );

        foreach (Collider2D hit in hits)
        {
            Dehumidifier machine =
                hit.GetComponentInParent<Dehumidifier>();

            if (machine == this)
            {
                isMoving = true;

                Debug.Log("DEHUMIDIFIER MOVING!");

                return;
            }
        }
    }

    // -------------------------
    // MOVE
    // -------------------------

    void MoveToMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                -Camera.main.transform.position.z
            )
        );

        worldPosition.z = transform.position.z;

        transform.position = worldPosition;
    }

    // -------------------------
    // RETURN TO TOOLBOX
    // -------------------------

    void CheckReturnToToolbox()
    {
        // Cannot return while the dehumidifier is running
        if (isRunning)
        {
            Debug.Log("DEHUMIDIFIER IS ON - CANNOT RETURN TO TOOLBOX!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapPointAll(
            transform.position
        );

        foreach (Collider2D hit in hits)
        {
            Toolbox toolbox =
                hit.GetComponentInParent<Toolbox>();

            if (toolbox != null)
            {
                gameObject.SetActive(false);

                Debug.Log("DEHUMIDIFIER RETURNED TO TOOLBOX!");

                return;
            }
        }
    }

    // -------------------------
    // START
    // -------------------------

    void StartDehumidifier()
    {
        if (isFinished)
            return;

        isRunning = true;
        statusLight.color = runningColour;

        Debug.Log("DEHUMIDIFIER STARTED!");
    }

    // -------------------------
    // FINISH
    // -------------------------

    void FinishDehumidifier()
    {
        isRunning = false;
        isFinished = true;

        statusLight.color = finishedColour;

        Debug.Log("DEHUMIDIFIER FINISHED!");
    }
}