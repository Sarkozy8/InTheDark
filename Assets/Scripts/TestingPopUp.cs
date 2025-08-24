using UnityEngine;

/// <summary>
/// Testing the new plugin to enable GPS on Android devices.
/// </summary>

public class TestingPopUp : MonoBehaviour
{

    private const string PluginClassName = "com.argosgamestudios.unity.MyPlugin";

    /*AndroidJavaObject javaObject;
    AndroidJavaClass unityClass;
    AndroidJavaObject unityActivity;

    private void Start()
    {
        javaObject = new AndroidJavaObject("com.argosgamestudios.unity.MyPlugin");
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
    }*/

    public void AskToEnableGPS()
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

}
