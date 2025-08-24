using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script manages the behavior of an enemy character (Sirenhead) using Unity's NavMeshAgent for navigation.
/// </summary>

public class EnemyNavAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public Transform playerPosition;

    private bool haloBool;

    [Header("Surface Object")]
    public NavMeshSurface navSurface;

    [Header("States and Other Important Values")]
    [SerializeField] private bool screamState;
    [SerializeField] private bool runningState;
    [SerializeField] private bool waitingState;

    private GlobalReferences _globalReferences;

    public AudioSource _audioSourceSiren;
    public AudioClip _audioSirenScream;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        _globalReferences = GameObject.Find("GlobalReferences").GetComponent<GlobalReferences>();
    }

    void Update()
    {
        // We got to wait until Mision Light Halo gets created so we can spawn Sirenhead
        if (GameObject.Find("Mission Light Halo") && !haloBool)
        {
            haloBool = true;
            waitingState = true;
        }

        // If halo exists, we will spawn SirenHead in 20 seconds
        if (haloBool && waitingState)
        {
            StartCoroutine(Waiting());
            _globalReferences.sirenMoving = false;
        }

        //After those 20 are up, we will do a scream animation
        if (haloBool && screamState)
        {
            StartCoroutine(Screaming());
            _globalReferences.sirenMoving = false;
        }

        // After Scream, we will start running towards the player
        if (haloBool && runningState)
        {
            StartCoroutine(Running());
            _globalReferences.sirenMoving = true;
        }

        // Make SirenHead follow player when running
        if (animator.GetBool("Running"))
        {
            agent.SetDestination(playerPosition.position);
        }

        if (_globalReferences.playerWon)
        {
            animator.SetBool("Running", false);
            agent.SetDestination(this.transform.position);
        }

    }

    IEnumerator Waiting()
    {
        screamState = false;
        runningState = false;
        waitingState = false;

        //Debug.Log("Waiting");

        yield return new WaitForSeconds(20);

        this.enabled = true;
        agent.Warp(GameObject.Find("Mission Light Halo").GetComponent<Transform>().position);

        screamState = true;
    }

    IEnumerator Screaming()
    {
        agent.SetDestination(this.transform.position);
        screamState = false;
        runningState = false;
        waitingState = false;

        animator.SetBool("Running", false);
        animator.SetTrigger("Yell");
        _audioSourceSiren.PlayOneShot(_audioSirenScream);
        //Debug.Log("Screaming");

        yield return new WaitForSeconds(7);

        animator.SetBool("Running", true);
        runningState = true;
    }
    IEnumerator Running()
    {

        screamState = false;
        runningState = false;
        waitingState = false;

        //Debug.Log("Running");

        yield return new WaitForSeconds(UnityEngine.Random.Range(20, 50));

        screamState = true;
    }
}
