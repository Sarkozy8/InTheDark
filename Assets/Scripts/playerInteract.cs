using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    private globalReferences _globalReferences;
    private void Start()
    {
        _globalReferences = GameObject.Find("GlobalReferences").GetComponent<globalReferences>();
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
