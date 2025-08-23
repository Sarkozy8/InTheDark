using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdateVolume : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");
    }

}
