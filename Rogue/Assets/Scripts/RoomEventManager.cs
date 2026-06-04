using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomEventManager : MonoBehaviour
{
    private Canvas eventCanvas;
    private Image fadeImage;
    private GameObject eventPanel;
    private Text messageText;
    private Button actionButton;
    private Text actionButtonText;
    private Button secondaryButton;
    private Text secondaryButtonText;

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            AddAppropriateInputModule(eventSystem);
        }

        GameObject canvasObject = new GameObject("RoomEventCanvas");
        eventCanvas = canvasObject.AddComponent<Canvas>();
        eventCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        fadeImage = CreateUIElement<Image>("FadeImage", canvasObject.transform);
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        RectTransform fadeRect = fadeImage.GetComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.offsetMin = Vector2.zero;
        fadeRect.offsetMax = Vector2.zero;

        eventPanel = new GameObject("EventPanel");
        eventPanel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = eventPanel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 180);
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = eventPanel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.85f);

        messageText = CreateUIElement<Text>("MessageText", eventPanel.transform);
        messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        messageText.fontSize = 22;
        messageText.color = Color.white;
        messageText.alignment = TextAnchor.MiddleCenter;
        messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
        messageText.verticalOverflow = VerticalWrapMode.Truncate;
        RectTransform messageRect = messageText.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0.1f, 0.4f);
        messageRect.anchorMax = new Vector2(0.9f, 0.85f);
        messageRect.offsetMin = Vector2.zero;
        messageRect.offsetMax = Vector2.zero;

        GameObject buttonObject = new GameObject("ContinueButton");
        buttonObject.transform.SetParent(eventPanel.transform, false);
        actionButton = buttonObject.AddComponent<Button>();
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.9f, 1f);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.12f, 0.1f);
        buttonRect.anchorMax = new Vector2(0.45f, 0.3f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        GameObject buttonTextObject = new GameObject("ButtonText");
        buttonTextObject.transform.SetParent(buttonObject.transform, false);
        actionButtonText = buttonTextObject.AddComponent<Text>();
        actionButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        actionButtonText.fontSize = 20;
        actionButtonText.color = Color.white;
        actionButtonText.alignment = TextAnchor.MiddleCenter;
        RectTransform buttonTextRect = actionButtonText.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        GameObject secondaryButtonObject = new GameObject("SecondaryButton");
        secondaryButtonObject.transform.SetParent(eventPanel.transform, false);
        secondaryButton = secondaryButtonObject.AddComponent<Button>();
        Image secondaryButtonImage = secondaryButtonObject.AddComponent<Image>();
        secondaryButtonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        RectTransform secondaryButtonRect = secondaryButtonObject.GetComponent<RectTransform>();
        secondaryButtonRect.anchorMin = new Vector2(0.55f, 0.1f);
        secondaryButtonRect.anchorMax = new Vector2(0.88f, 0.3f);
        secondaryButtonRect.offsetMin = Vector2.zero;
        secondaryButtonRect.offsetMax = Vector2.zero;

        GameObject secondaryButtonTextObject = new GameObject("SecondaryButtonText");
        secondaryButtonTextObject.transform.SetParent(secondaryButtonObject.transform, false);
        secondaryButtonText = secondaryButtonTextObject.AddComponent<Text>();
        secondaryButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        secondaryButtonText.fontSize = 20;
        secondaryButtonText.color = Color.white;
        secondaryButtonText.alignment = TextAnchor.MiddleCenter;
        RectTransform secondaryButtonTextRect = secondaryButtonText.GetComponent<RectTransform>();
        secondaryButtonTextRect.anchorMin = Vector2.zero;
        secondaryButtonTextRect.anchorMax = Vector2.one;
        secondaryButtonTextRect.offsetMin = Vector2.zero;
        secondaryButtonTextRect.offsetMax = Vector2.zero;

        eventPanel.SetActive(false);
    }

    private void AddAppropriateInputModule(GameObject eventSystem)
    {
        System.Type inputSystemType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputSystemType != null)
        {
            eventSystem.AddComponent(inputSystemType);
            return;
        }

        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private T CreateUIElement<T>(string name, Transform parent) where T : Graphic
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        T component = go.AddComponent<T>();
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return component;
    }

    public IEnumerator PlayPowerGainEvent(int amount, System.Action onComplete)
    {
        if (eventCanvas == null) InitializeUI();

        yield return Fade(0f, 0.8f, 0.3f);

        eventPanel.SetActive(true);
        messageText.text = $"あなたは電力を{amount}得た。";
        actionButtonText.text = "進む";
        secondaryButton.gameObject.SetActive(false);
        actionButton.gameObject.SetActive(true);

        bool clicked = false;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => clicked = true);

        while (!clicked)
        {
            yield return null;
        }

        onComplete?.Invoke();
        eventPanel.SetActive(false);
        yield return Fade(0.8f, 0f, 0.3f);
    }

    public IEnumerator PlayFloorTransitionEvent(int currentFloor, int nextFloor, System.Action onDescend, System.Action onStay)
    {
        if (eventCanvas == null) InitializeUI();

        yield return Fade(0f, 0.8f, 0.3f);

        eventPanel.SetActive(true);
        messageText.text = $"階段を見つけた。層{currentFloor}から層{nextFloor}へ進みますか？";
        actionButtonText.text = $"層{nextFloor}へ進む";
        secondaryButtonText.text = "留まる";
        actionButton.gameObject.SetActive(true);
        secondaryButton.gameObject.SetActive(true);

        bool chosen = false;
        bool descend = false;
        actionButton.onClick.RemoveAllListeners();
        secondaryButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => { descend = true; chosen = true; });
        secondaryButton.onClick.AddListener(() => { descend = false; chosen = true; });

        while (!chosen)
        {
            yield return null;
        }

        eventPanel.SetActive(false);
        if (descend)
        {
            onDescend?.Invoke();
        }
        else
        {
            onStay?.Invoke();
        }

        yield return Fade(0.8f, 0f, 0.3f);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(color.r, color.g, color.b, to);
    }
}
