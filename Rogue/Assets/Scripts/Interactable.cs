using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [TextArea]
    public string promptText = "Press Z to interact";

    public virtual void Interact()
    {
        Debug.Log($"Interacted with {name}");
    }
}
