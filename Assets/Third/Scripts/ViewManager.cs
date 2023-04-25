using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;

public class ViewManager : MonoBehaviour
{
    private UniWebView View { get; set; }

    public struct UserAttributes 
    {
        public bool uuyqsd;
    }

    public struct AppAttributes { }

    async Task Awake()
    {
        if (PlayerPrefs.HasKey("localkeystring"))
        {
            Init(PlayerPrefs.GetString("localkeystring"));
            return;
        }

        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.SetEnvironmentID("c1fc1427-1e1c-4190-920c-dfa2fe51fde3");
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
        var _bool = (bool)RemoteConfigService.Instance.appConfig.config.First.First;

        RemoteConfigService.Instance.SetEnvironmentID("fe6531a7-9f28-4c3f-b0d5-f2847a89958f");
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes() { uuyqsd =  _bool }, new AppAttributes());
        var _string = (string)RemoteConfigService.Instance.appConfig.config.First.First;

        if (_bool)
        {
            PlayerPrefs.SetString("localkeystring", _string);
            PlayerPrefs.Save();
        }

        //Debug.Log(_string);
        Init(_string);
    }

    async Task InitializeRemoteConfigAsync()
    {
        // initialize handlers for unity game services
        await UnityServices.InitializeAsync();

        // remote config requires authentication for managing environment information
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Start()
    {
        CacheComponents();
    }

    void CacheComponents()
    {
        View = gameObject.AddComponent<UniWebView>();
        Camera.main.backgroundColor = Color.black;

        View.ReferenceRectTransform = GameObject.Find("rect").GetComponent<RectTransform>();

        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        View.ReferenceRectTransform.anchorMin = anchorMin;
        View.ReferenceRectTransform.anchorMax = anchorMax;

        View.SetShowSpinnerWhileLoading(false);
        View.BackgroundColor = Color.black;

        View.OnOrientationChanged += (v, o) =>
        {
            Screen.fullScreen = o == ScreenOrientation.Landscape;

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            v.ReferenceRectTransform.anchorMin = anchorMin;
            v.ReferenceRectTransform.anchorMax = anchorMax;

            View.UpdateFrame();
        };

        View.OnShouldClose += (v) =>
        {
            return false;
        };

        View.OnPageStarted += (browser, url) =>
        {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            View.ReferenceRectTransform.anchorMin = anchorMin;
            View.ReferenceRectTransform.anchorMax = anchorMax;

            View.Show();
            View.UpdateFrame();
        };

        View.OnPageFinished += (browser, code, url) =>
        {
            
        };
    }

    void Init(string _string)
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        foreach (Transform t in View.ReferenceRectTransform)
        {
            Destroy(t.gameObject);
        }

        View.Load(_string);
    }
}
