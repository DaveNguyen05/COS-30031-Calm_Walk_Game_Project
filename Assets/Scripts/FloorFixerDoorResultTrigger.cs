using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Finish-door interaction for Floor Fixer.
///
/// - Player enters the trigger.
/// - "Press E to end game" appears.
/// - Press E to show the existing FloorFixer result/stats UI.
/// - Optional: hide the door sprite while keeping its collider active.
/// - Prompt X/Y are offsets from the screen center.
/// </summary>
[DisallowMultipleComponent]
public class FloorFixerDoorResultTrigger : MonoBehaviour
{
    public enum ResultMode
    {
        Auto,
        Good,
        Bad
    }

    [Header("Door visual")]
    [Tooltip("If enabled, SpriteRenderer components on this door are hidden, but the collider still works.")]
    [SerializeField] private bool hideDoorSprite = true;

    [Header("Door interaction")]
    [SerializeField] private KeyCode legacyInteractKey = KeyCode.E;
    [SerializeField] private string promptText = "Press E to end game";
    [SerializeField] private bool oneUseOnly = true;

    [Header("Prompt position")]
    [Tooltip("Horizontal offset from the screen center. Negative = left, positive = right.")]
    [SerializeField] private float promptX = 0f;

    [Tooltip("Vertical offset from the screen center. Negative = up, positive = down.")]
    [SerializeField] private float promptY = 300f;

    [SerializeField] private float promptWidth = 360f;
    [SerializeField] private float promptHeight = 58f;

    [Header("Result")]
    [SerializeField] private ResultMode resultMode = ResultMode.Auto;

    [Range(0f, 1f)]
    [SerializeField] private float goodProgressThreshold = 0.80f;

    [Range(0f, 1f)]
    [SerializeField] private float goodQualityThreshold = 0.60f;

    [Header("Optional")]
    [Tooltip("Normally leave empty. The script finds FloorFixerUI automatically.")]
    [SerializeField] private MonoBehaviour floorFixerUI;

    private bool playerInside;
    private bool used;

    private GUIStyle promptStyle;
    private Texture2D promptBackground;

    private void Awake()
    {
        EnsureTriggerCollider();

        if (hideDoorSprite)
            HideDoorVisuals();

        FindFloorFixerUI();
    }

    private void Update()
    {
        if (!playerInside || (oneUseOnly && used))
            return;

        if (InteractPressedThisFrame())
        {
            used = true;
            StartCoroutine(ShowResult());
        }
    }

    private IEnumerator ShowResult()
    {
        if (floorFixerUI == null)
            FindFloorFixerUI();

        if (floorFixerUI == null)
        {
            Debug.LogError(
                "[Floor Fixer Door] Could not find FloorFixer.UI.FloorFixerUI. Deploy the UI first.",
                this);

            used = false;
            yield break;
        }

        TryInvoke(floorFixerUI, "CompleteTask");
        yield return null;

        ResultMode finalMode = resultMode;

        if (finalMode == ResultMode.Auto)
            finalMode = ChooseAutomaticResult();

        string method = finalMode == ResultMode.Bad ? "Bad" : "Good";

        if (!TryInvoke(floorFixerUI, method))
        {
            Debug.LogError(
                $"[Floor Fixer Door] Could not call {method}() on FloorFixerUI.",
                this);

            used = false;
            yield break;
        }

        playerInside = false;
    }

    private ResultMode ChooseAutomaticResult()
    {
        float progress = ReadUi01Value("progressBar", 1f);
        float quality = ReadUi01Value("qualityBar", 1f);

        return progress >= goodProgressThreshold && quality >= goodQualityThreshold
            ? ResultMode.Good
            : ResultMode.Bad;
    }

    private float ReadUi01Value(string fieldName, float fallback)
    {
        if (floorFixerUI == null)
            return fallback;

        Type type = floorFixerUI.GetType();
        FieldInfo field = type.GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field == null)
            return fallback;

        object target = field.GetValue(floorFixerUI);
        if (target == null)
            return fallback;

        Type targetType = target.GetType();

        PropertyInfo valueProp = targetType.GetProperty("value");
        if (valueProp != null)
        {
            try
            {
                float value = Convert.ToSingle(valueProp.GetValue(target));

                PropertyInfo minProp = targetType.GetProperty("minValue");
                PropertyInfo maxProp = targetType.GetProperty("maxValue");

                if (minProp != null && maxProp != null)
                {
                    float min = Convert.ToSingle(minProp.GetValue(target));
                    float max = Convert.ToSingle(maxProp.GetValue(target));

                    if (!Mathf.Approximately(min, max))
                        return Mathf.Clamp01(Mathf.InverseLerp(min, max, value));
                }

                return Mathf.Clamp01(value);
            }
            catch { }
        }

        PropertyInfo fillProp = targetType.GetProperty("fillAmount");
        if (fillProp != null)
        {
            try
            {
                return Mathf.Clamp01(Convert.ToSingle(fillProp.GetValue(target)));
            }
            catch { }
        }

        return fallback;
    }

    private void HideDoorVisuals()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in spriteRenderers)
            sr.enabled = false;
    }

    private void FindFloorFixerUI()
    {
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null)
                continue;

            Type t = behaviour.GetType();

            if (t.FullName == "FloorFixer.UI.FloorFixerUI" ||
                t.Name == "FloorFixerUI")
            {
                floorFixerUI = behaviour;
                return;
            }
        }
    }

    private static bool TryInvoke(MonoBehaviour target, string methodName)
    {
        if (target == null)
            return false;

        MethodInfo method = target.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            Type.EmptyTypes,
            null);

        if (method == null)
            return false;

        method.Invoke(target, null);
        return true;
    }

    private bool InteractPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            return true;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(legacyInteractKey);
#else
        return false;
#endif
    }

    private void EnsureTriggerCollider()
    {
        Collider2D c2d = GetComponent<Collider2D>();
        if (c2d != null)
        {
            c2d.isTrigger = true;
            return;
        }

        Collider c3d = GetComponent<Collider>();
        if (c3d != null)
        {
            c3d.isTrigger = true;
            return;
        }

        Debug.LogWarning(
            "[Floor Fixer Door] No Collider2D/Collider found on this object.",
            this);
    }

    private bool IsPlayer(GameObject obj)
    {
        if (obj == null)
            return false;

        Transform root = obj.transform.root;
        GameObject rootObject = root != null ? root.gameObject : obj;

        if (obj.CompareTag("Player") || rootObject.CompareTag("Player"))
            return true;

        return rootObject.name.IndexOf("Player", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayer(other.gameObject))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other.gameObject))
            playerInside = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other.gameObject))
            playerInside = false;
    }

    private void OnGUI()
    {
        if (!playerInside || (oneUseOnly && used))
            return;

        if (promptStyle == null)
            BuildPromptStyle();

        // X/Y are now offsets from screen center.
        float x = (Screen.width * 0.5f) - (promptWidth * 0.5f) + promptX;
        float y = (Screen.height * 0.5f) - (promptHeight * 0.5f) + promptY;

        Rect rect = new Rect(x, y, promptWidth, promptHeight);
        GUI.Label(rect, promptText, promptStyle);
    }

    private void BuildPromptStyle()
    {
        promptBackground = new Texture2D(1, 1);
        promptBackground.SetPixel(0, 0, new Color(0.03f, 0.08f, 0.12f, 0.92f));
        promptBackground.Apply();

        promptStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 22,
            fontStyle = FontStyle.Bold,
            normal =
            {
                textColor = Color.white,
                background = promptBackground
            },
            padding = new RectOffset(18, 18, 10, 10)
        };
    }

    private void OnDestroy()
    {
        if (promptBackground != null)
            Destroy(promptBackground);
    }
}
