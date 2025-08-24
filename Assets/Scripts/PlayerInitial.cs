using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This moves the player to the Mission Light Halo position at the start of the game
/// </summary>


public class PlayerInitial : MonoBehaviour
{
    private bool haloBool;
    private Vector3 playerPositionNew;
    public GameObject capsuleGO;

    void Update()
    {
        if (GameObject.Find("Mission Light Halo") && !haloBool)
        {
            haloBool = true;
            playerPositionNew = GameObject.Find("Mission Light Halo").transform.position;
            capsuleGO.transform.position = playerPositionNew;
            Debug.Log(playerPositionNew);
        }
    }
}
