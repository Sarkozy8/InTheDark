using Cinemachine.PostFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UpdateBright : MonoBehaviour
{
    private CinemachineVolumeSettings volume;

    private ColorAdjustments colorAdjustments;

    // Start is called before the first frame update
    private void Start()
    {
        volume = GetComponent<CinemachineVolumeSettings>();
        ColorAdjustments tmp;
        if (volume.m_Profile.TryGet<ColorAdjustments>(out tmp))
        {
            colorAdjustments = tmp;
        }
        colorAdjustments.postExposure.value = PlayerPrefs.GetFloat("Brightness");
    }


}
