using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This the interactable generator object that the player can activate to power up the exit.
/// </summary>
public class generator : MonoBehaviour, IInteractable
{
    private bool _activated = false;

    [SerializeField] private string _prompt;
    public Light _light;
    public string InteractionPrompt => _prompt;
    private GlobalReferences _globalReferences;
    private ParticleSystem _particleSystem;
    private AudioSource _audioSourceGenerator;
    public AudioClip _audioStartup;
    public GameObject _gameObjectAudio;


    private void Start()
    {
        _globalReferences = GameObject.Find("GlobalReferences").GetComponent<GlobalReferences>();
        _particleSystem = this.GetComponentInChildren<ParticleSystem>();
        _audioSourceGenerator = _gameObjectAudio.GetComponent<AudioSource>();
    }

    public bool Interact(Interactor interactor)
    {
        if (_activated == false)
        {
            _globalReferences.generatorActivated++;
            _globalReferences.generatorDisplay.text = $"{_globalReferences.generatorActivated}/{_globalReferences.generatorCounter}";
            _light.color = Color.green;
            _activated = true;
            var main = _particleSystem.main;
            main.startLifetime = 0f;
            _audioSourceGenerator.PlayOneShot(_audioStartup);
            Debug.Log("Generator Activated");
        }

        return true;
    }

}
