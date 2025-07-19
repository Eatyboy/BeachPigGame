using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool isInteractable;

    public abstract void Interact(GameObject interactor);
}
