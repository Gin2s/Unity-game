using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public GameObject dialoguePanel;
    public Text speakerNameText;
    public Text dialogueText;
    public KeyCode advanceKey = KeyCode.Z;

    private string[] lines;
    private int currentIndex;
    private bool isActive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        HideDialogue();
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(advanceKey))
        {
            AdvanceDialogue();
        }
    }

    public void ShowDialogue(string[] newLines)
    {
        if (newLines == null || newLines.Length == 0) return;

        lines = newLines;
        currentIndex = 0;
        isActive = true;
        dialoguePanel.SetActive(true);
        UpdateDialogueText();
    }

    void AdvanceDialogue()
    {
        currentIndex++;
        if (currentIndex >= lines.Length)
        {
            HideDialogue();
        }
        else
        {
            UpdateDialogueText();
        }
    }

    void UpdateDialogueText()
    {
        dialogueText.text = lines[currentIndex];
    }

    public void HideDialogue()
    {
        isActive = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
}
