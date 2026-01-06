public interface IInteractable
{
    string PromptText { get; }
    bool CanInteract(PlayerController controller);
    void Interact(PlayerController controller);
}