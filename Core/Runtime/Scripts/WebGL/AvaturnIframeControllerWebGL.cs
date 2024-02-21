using System.Runtime.InteropServices;
using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts.WebGL
{
  public class AvaturnIframeControllerWebGL : AvaturnIframeController
  {
    // This class is used to control Avaturn iframe for WebGL platform.
    // IframeControllerMobile can also be used for these purposes
    // but this class supports dataURLs which are generated much faster than httpURLs
    // If you are building cross-platform application and use httpURLs, you may not need this class.

    private static bool _isSetup = false;
    private static bool _isOpen = false;

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetupAvaturnIframeJS(string subdomain, string unityVersion, string platform);

    [DllImport("__Internal")]
    private static extern void ShowAvaturnIframeJS();

    [DllImport("__Internal")]
    private static extern void HideAvaturnIFrameJS();
#endif

    public void ChangeVisibility()
    {
      if (_isOpen)
        Hide();
      else
        Show();
    }

    public void Show()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
      SetupIframe();
      ShowAvaturnIframeJS();
      _isOpen = true;
#else
      Debug.Log("Iframe can't be open in editor. Build the project for WebGL to open iframe.");
#endif
    }

    public void Hide()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
      HideAvaturnIFrameJS();
      _isOpen = false;
#else
      Debug.Log("Iframe does not work in editor. Build the project for WebGL to open iframe.");
#endif
    }

    public void SetupIframe()
    {
      if (_isSetup)
        return;

      string link = _linkFromAPI == "" ? $"https://{_subdomain}.avaturn.dev" : _linkFromAPI;
      
#if !UNITY_EDITOR && UNITY_WEBGL
      SetupAvaturnIframeJS(link, Application.unityVersion, GetPlatformString());
#endif

      _isSetup = true;
    }
  }
}