using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStreetHouse : MonoBehaviour
{

    private Texture2D StreetViewMap;
    public RawImage RawImageCanvas;
    private bool errorCath;

    // Start is called before the first frame update
    void Start()
    {
        string filePath = $"{UnityEngine.Application.persistentDataPath}/PlayerStreetMap.png";
        Debug.Log($"File path: {filePath}");
        StreetViewMap = new Texture2D(2, 2);

        if (PlayerPrefs.GetInt("PLayerStreetMap") == 1)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    // Get pixels from map
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                    Debug.Log($"File size: {fileData.Length} bytes");

                    if (StreetViewMap != null)
                    {
                        bool success = StreetViewMap.LoadImage(fileData);
                        Debug.Log(success ? "Loaded image successfully" : "Failed to load image");
                    }
                    else
                    {
                        Debug.LogError("StreetViewMap is null!");
                        errorCath = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading file or loading image: {ex.Message}");
                    errorCath = true;
                }
            }
            else
            {
                Debug.LogError("File does not exist at the specified path.");
                errorCath = true;
            }
        }
        else if (PlayerPrefs.GetInt("PLayerStreetMap") == 0 || errorCath == true)
        {
            StreetViewMap = Resources.Load<Texture2D>("ScaryHouse");
            Debug.Log("Using Default SV one");
        }

        RawImageCanvas.texture = StreetViewMap;
    }


}
