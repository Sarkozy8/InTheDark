using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is used for the main menu scene to create a flickering light effect
/// </summary>


public class FlickerLight : MonoBehaviour
{
    public bool isFlickering = false;
    public float timeDelay;
    public float minDelayOff = 0.01f;
    public float MaxDelayOff = 0.2f;
    public float minDelayOn = 0.01f;
    public float MaxDelayOn = 0.2f;

    // Update is called once per frame
    void Update()
    {
        if (isFlickering == false)
        {
            StartCoroutine(FlickeringLight());
        }
    }

    IEnumerator FlickeringLight()
    {
        isFlickering = true;
        this.gameObject.GetComponent<Light>().enabled = false;
        timeDelay = Random.Range(minDelayOff, MaxDelayOff);
        yield return new WaitForSeconds(timeDelay);
        this.gameObject.GetComponent<Light>().enabled = true;
        timeDelay = Random.Range(minDelayOn, MaxDelayOn);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;

    }

}
