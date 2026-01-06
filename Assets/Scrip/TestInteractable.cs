using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] string promptText = "상호작용 (E)";
    [SerializeField] bool canInteract = true;

    public string PromptText => promptText;

    public bool CanInteract(PlayerController controller)
    {
        return canInteract;
    }

    public void Interact(PlayerController controller)
    {
        Debug.Log($"{name} interacted by {controller.name}");
    }
}