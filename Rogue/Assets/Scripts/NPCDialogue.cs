using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCDialogue : Interactable
{
    [TextArea(3, 10)]
    public string[] dialogueLines;

    public override void Interact()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning($"{name} has no dialogue lines.");
            return;
        }

        DialogueManager.Instance.ShowDialogue(dialogueLines);
    }
}
