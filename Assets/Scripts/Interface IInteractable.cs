using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Template for interactable objects.
/// </summary>
public interface IInteractable
{
    public string InteractionPrompt { get; }

    public bool Interact(Interactor interactor);
}
