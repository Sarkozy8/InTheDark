using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates Volume with Slider
/// </summary>

public class SliderUpdateVolume : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");
    }

}
