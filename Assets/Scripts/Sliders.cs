using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Sliders : MonoBehaviour
{
    private Volume volume;

    private ColorAdjustments colorAdjustments;
    private void Start()
    {
        volume = GetComponent<Volume>();
        ColorAdjustments tmp;
        if (volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            colorAdjustments = tmp;
        }


        if (!PlayerPrefs.HasKey("Brightness"))
        {
            PlayerPrefs.SetFloat("Brightness", 2f);
        }
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 100f);
        }
        PlayerPrefs.Save();

        colorAdjustments.postExposure.value = PlayerPrefs.GetFloat("Brightness");

    }

    public void UpdateBrightness(float brightness)
    {
        if (PlayerPrefs.HasKey("Brightness"))
        {
            PlayerPrefs.SetFloat("Brightness", brightness);
        }
        PlayerPrefs.Save();
        colorAdjustments.postExposure.value = PlayerPrefs.GetFloat("Brightness");
        Debug.Log(brightness);
    }

    public void UpdateVolume(float volume)
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", volume);
        }
        PlayerPrefs.Save();
        Debug.Log(volume);
    }
}
