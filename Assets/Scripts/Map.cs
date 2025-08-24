using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using TMPro;
using UnityEngine.Android;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.VisualScripting;

/// <summary>
/// Get the map from Google Maps Static API and street view image from Google Street View Static API.
/// Also, get the device location using GPS.
/// To ask permission, the plugin I made is used to request enabling GPS on Android devices.
/// Same way that Google Maps app does.
/// </summary>

public class Map : MonoBehaviour
{
    public string apiKey = "AIzaSyCH1gmSeNjU_VRBrxhCYIs33YB17A6xbl0";
    public float lat = 25.85707f;
    public float lon = -80.27847f;
    private int zoom = 16;
    public enum resolution { low = 1, high = 2 };
    public resolution mapResolution = resolution.low;
    public enum type { roadmap, satellite, gybrid, terrain };
    public type mapType = type.roadmap;
    private string url = "";
    private int mapWidth = 500;
    private int mapHeight = 500;
    //private bool mapIsLoading = false; //not used. Can be used to know that the map is loading 
    private Rect rect;

    private string apiKeyLast = "AIzaSyCH1gmSeNjU_VRBrxhCYIs33YB17A6xbl0";
    private float latLast = 25.85707f;
    private float lonLast = -80.27847f;
    private int zoomLast = 12;
    private resolution mapResolutionLast = resolution.low;
    private type mapTypeLast = type.roadmap;

    // Location Auto
    public TextMeshProUGUI textMeshProUGUI;
    public TextMeshProUGUI textMeshProUGUI2;
    PermissionCallbacks callback;

    public UnityEngine.UI.Text gpsOut;
    public bool isUpdating;
    LocationServiceStatus locationServiceStatus;
    Texture2D texture;

    public UnityEngine.UI.Image image;
    public float fadeDuration = 1.0f;
    public GameObject backgroundFade;
    public bool didTheButtonGetPressed;

    //Android Class
    private const string PluginClassName = "com.argosgamestudios.unity.MyPlugin";

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("PLayerMap", 0);
        PlayerPrefs.SetInt("PLayerStreetMap", 0);

    }


    IEnumerator GettingLocation()
    {
        if (!didTheButtonGetPressed)
        {
            didTheButtonGetPressed = true;
            // Check if the user has location service enabled.
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);

            // Wait for user to grant permissions
            yield return new WaitUntil(() =>
                Permission.HasUserAuthorizedPermission(Permission.FineLocation) ||
                Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
            );

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation) &&
            !Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            {
                Debug.LogError("Location permissions not granted");
                StartCoroutine(FadeIn());
                yield break;
            }

            RequestLocationSettings();
            //OpenLocationSettings();

            // Starts the location service.
            Input.location.Start();
            Input.location.Start(1f, 1f);

            // Waits until the location service initializes
            int maxWait = 5;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // If the service didn't initialize in 5 seconds this cancels location service use.
            if (maxWait < 1)
            {
                Debug.Log("Timed out");
                Debug.LogError("Unable to determine device location");
                StartCoroutine(FadeIn());
                yield break;
            }

            // If the connection failed this cancels location service use.
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.LogError("Unable to determine device location");
                StartCoroutine(FadeIn());
                yield break;
            }
            else if (Input.location.lastData.latitude == 0 && Input.location.lastData.longitude == 0)
            {
                Debug.LogError("Unable to determine device location22");
                StartCoroutine(FadeIn());
                yield break;
            }
            else
            {
                lat = Input.location.lastData.latitude;
                lon = Input.location.lastData.longitude;
                // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
                Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
            }

            // Stops the location service if there is no need to query location updates continuously.
            Input.location.Stop();

            StartCoroutine(GetGoogleMap());

        }
    }

    private void RequestLocationSettings()
    {
        if (UnityEngine.Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaClass gpsManager = new AndroidJavaClass(PluginClassName))
                {
                    gpsManager.CallStatic("requestEnableGPS", activity);
                }
            }
        }
        else
        {
            Debug.LogWarning("GPS enable request is only supported on Android.");
        }
    }

    void OpenLocationSettings()
    {
#if UNITY_EDITOR
        return;
#else
        try
            {
                // Get the current Android activity
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Prepare an Intent to open location settings
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent",
                    "android.settings.LOCATION_SOURCE_SETTINGS");

                // Start the activity with the intent
                currentActivity.Call("startActivity", intent);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to open location settings: " + e.Message);
            }
#endif
    }



    public void buttonLocation()
    {

        Debug.Log("Location Started");

        StartCoroutine(GettingLocation());
    }

    IEnumerator GetGoogleMap()
    {
        url = "https://maps.googleapis.com/maps/api/staticmap?center=" + lat + "," + lon + "&zoom=" + zoom + "&size=" + mapWidth + "x" + mapHeight + "&scale=" + mapResolution + "&maptype=" + mapType + "&key=" + apiKey + "&style=element:labels%7Cvisibility:off&style=feature:administrative%7Celement:geometry%7Cvisibility:off&style=feature:administrative.land_parcel%7Cvisibility:off&style=feature:administrative.neighborhood%7Cvisibility:off&style=feature:poi%7Cvisibility:off&style=feature:road%7Celement:labels.icon%7Cvisibility:off&style=feature:transit%7Cvisibility:off&size=480x360";

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
            PlayerPrefs.SetInt("PLayerMap", 0);
        }
        else
        {

            texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            byte[] bytes = ImageConversion.EncodeToPNG(texture);

            print(UnityEngine.Application.dataPath);
            File.WriteAllBytes($"{UnityEngine.Application.persistentDataPath}/PlayerMap.png", bytes);


            apiKeyLast = apiKey;
            latLast = lat;
            lonLast = lon;
            zoomLast = zoom;
            mapResolutionLast = mapResolution;
            mapTypeLast = mapType;
            PlayerPrefs.SetInt("PLayerMap", 1);

            StartCoroutine(GetGoogleStreetMap());

        }
    }

    IEnumerator GetGoogleStreetMap()
    {
        url = "https://maps.googleapis.com/maps/api/streetview?size=1000x1000&location=" + lat + "," + lon + "&fov=80" + "&key=" + apiKey;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
            PlayerPrefs.SetInt("PLayerStreetMap", 0);
        }
        else
        {
            string NoImageLength = "8834";

            if (www.GetResponseHeaders().TryGetValue("Content-Length", out string contentLength))
            {
                if (contentLength == NoImageLength)
                {
                    Debug.Log("No valid Street View image available for this location.");
                    PlayerPrefs.SetInt("PLayerStreetMap", 0);
                    yield break;
                }
            }

            texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            byte[] bytes = ImageConversion.EncodeToPNG(texture);

            print(UnityEngine.Application.dataPath);
            File.WriteAllBytes($"{UnityEngine.Application.persistentDataPath}/PlayerStreetMap.png", bytes);
            PlayerPrefs.SetInt("PLayerStreetMap", 1);

            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        backgroundFade.SetActive(true);
        didTheButtonGetPressed = false;
        float elapsedTime = 0f;
        Color color = image.color;
        color.a = 0f; // Start fully transparent.
        image.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration); // Incrementally increase alpha.
            image.color = color;
            yield return null;
        }

        color.a = 1f; // Ensure fully visible at the end.
        image.color = color;
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(1);
    }
}