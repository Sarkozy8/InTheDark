using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Update the dead status of the player when interacted with.
/// </summary>
public class PlayerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    private GlobalReferences _globalReferences;
    private void Start()
    {
        _globalReferences = GameObject.Find("GlobalReferences").GetComponent<GlobalReferences>();
    }
    public bool Interact(Interactor interactor)
    {
        if (_globalReferences.areYouDead == false)
        {
            _globalReferences.areYouDead = true;
        }

        return true;
    }
}
