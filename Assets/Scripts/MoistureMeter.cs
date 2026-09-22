using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class MoistureMeter : MonoBehaviour
{
    [Header("Moisture Settings")]
    public float currentMoisture = 47f;
    public float requiredMinimum = 40f;
    public float requiredMaximum = 50f;

    [Header("Meter UI")]
    public GameObject meterPopup;
    public Image meterScreen;
    public TMP_Text resultText;

    [Header("Colours")]
    public Color scanningColour = new Color(0.0f, 0.6f, 1.0f);
    public Color goodColour = Color.green;
    public Color cautionColour = Color.yellow;
    public Color dryColour = Color.red;

    [Header("Floor")]
    public Collider2D floorCollider;

    private bool isScanning = false;

    void Start()
    {
        meterPopup.SetActive(true);

        meterScreen.color = scanningColour;
        resultText.text = "READY";

        Debug.Log("Moisture Meter ready.");
    }

    void Update()
    {
        if (isScanning)
            return;

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckFloorClick();
        }
    }

    void CheckFloorClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z)
        );

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null && hit.collider == floorCollider)
        {
            StartCoroutine(ScanMoisture());
        }
    }

    IEnumerator ScanMoisture()
    {
        isScanning = true;

        // Blue scanning screen
        meterScreen.color = scanningColour;
        resultText.text = "SCANNING";

        yield return new WaitForSeconds(1f);

        // Check moisture level
        if (currentMoisture >= requiredMinimum &&
            currentMoisture <= requiredMaximum)
        {
            meterScreen.color = goodColour;
            resultText.text = "GOOD";
        }
        else if (currentMoisture >= requiredMinimum - 5 &&
                 currentMoisture <= requiredMaximum + 5)
        {
            meterScreen.color = cautionColour;
            resultText.text = "CAUTION";
        }
        else
        {
            meterScreen.color = dryColour;
            resultText.text = "DRY";
        }

        isScanning = false;
    }
}