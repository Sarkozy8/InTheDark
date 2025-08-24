using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Updates Brightness with Slider
/// </summary>

public class SliderUpdateBright : MonoBehaviour
{
    private Volume volume;

    private ColorAdjustments colorAdjustments;

    void Start()
    {
        this.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Brightness");
    }

}
