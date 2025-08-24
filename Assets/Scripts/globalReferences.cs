using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This object holds all the global references needed for the game. This is a singleton.
/// </summary>

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { get; set; }

    public TextMeshProUGUI generatorDisplay;
    public TextMeshProUGUI youAreDeadDisplay;
    public int generatorCounter;
    public int generatorActivated;
    public bool goHomeReminder;
    public TextMeshProUGUI reminderText;
    public bool areYouDead;
    public bool areYouDead2;
    public GameObject PlayerCamera;
    public GameObject SirenHeadJumpscare;
    public Animator SirenHeadAnimator;
    public GameObject PlayerControls;
    public GameObject PlayerControlsUi;
    public GameObject GeneratorCanvasGameObject;
    public Animator GeneratorCanvasAnimator;
    public Animator FadeinOutAnimator;
    public bool playerWon;
    public bool playerMoving;
    public AudioSource playerAS;
    public AudioClip playerFootSteps;
    public float playerFootstepsDelay;
    private bool playerFootWait;
    public bool sirenMoving;
    public AudioSource SirenAS;
    public AudioClip SirenFootSteps;
    public float SirenFootstepsDelay;
    private bool SirenFootWait;
    public GameObject UITouchInput;
    public GameObject InputSystem;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        //Set All Values I need

        generatorCounter = 0;
        generatorActivated = 0;
        areYouDead = false;
        //InputSystem.SetActive(false);
        //InputSystem.SetActive(true);

    }

    void Update()
    {
        if (areYouDead && !areYouDead2)
        {
            areYouDead2 = true;
            StartCoroutine(waitForReset());
        }

        if (playerMoving && !playerFootWait)
        {
            StartCoroutine(PlayerFootSound());
            //Debug.Log("Moving");
            playerFootWait = true;
        }

        if (sirenMoving && !SirenFootWait)
        {
            StartCoroutine(SirenFootSound());
            SirenFootWait = true;
        }

        if (generatorCounter <= generatorActivated && !goHomeReminder)
        {
            goHomeReminder = true;
            StartCoroutine(reminderFadein());

        }
    }

    IEnumerator reminderFadein()
    {
        reminderText.alpha = 0; // Ensure the text starts fully transparent
        float elapsedTime = 0;

        while (elapsedTime < 5)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / 5);
            reminderText.alpha = alpha; // Set the text's alpha value
            yield return null;
        }

        reminderText.alpha = 1; // Ensure the text is fully visible at the end
        goHomeReminder = true;
    }


    IEnumerator waitForReset()
    {
        //UITouchInput.SetActive(false);
        PlayerCamera.SetActive(false);
        //PlayerControls.SetActive(false);
        //PlayerControlsUi.SetActive(false);
        SirenHeadJumpscare.SetActive(true);
        GeneratorCanvasGameObject.SetActive(false);
        SirenHeadAnimator.SetBool("JumpScare", true);
        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene(0);
    }

    IEnumerator PlayerFootSound()
    {
        playerAS.pitch = UnityEngine.Random.Range(0.80f, 1.20f);
        playerAS.PlayOneShot(playerFootSteps);
        yield return new WaitForSeconds(playerFootstepsDelay);
        playerFootWait = false;
    }
    IEnumerator SirenFootSound()
    {
        SirenAS.pitch = UnityEngine.Random.Range(0.80f, 1.20f);
        SirenAS.PlayOneShot(SirenFootSteps);
        yield return new WaitForSeconds(SirenFootstepsDelay);
        SirenFootWait = false;
    }
}
