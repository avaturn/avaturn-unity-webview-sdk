using System;
using Avaturn.Core.Runtime.Scripts.Avatar;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

#if UNITY_IOS
using UnityEngine.iOS;
using UnityEngine.Serialization;
#endif

namespace Avaturn.Core.Runtime.Scripts.Mobile
{
  public class AvaturnIframeControllerMobile : AvaturnIframeController
  {
    [Space(10)] 
    [SerializeField] private GameObject _webViewGameObject;
    [SerializeField] private RectTransform _webViewFrame;

    private UniWebView _webView;

    public override void Start()
    {
      base.Start();
      
      string domain, link;
      if (_linkFromAPI == "")
      {
        domain = $"{_subdomain}.avaturn.dev";
        link = $"https://{domain}?sdk=true";
      }
      else
      {
        domain = new Uri(_linkFromAPI).Host;
        link = _linkFromAPI + "&sdk=true";
      }

#if UNITY_ANDROID
      Permission.RequestUserPermission(Permission.Camera);
      Permission.RequestUserPermission(Permission.Microphone);
#elif UNITY_IOS
      Application.RequestUserAuthorization(UserAuthorization.WebCam);
      Application.RequestUserAuthorization(UserAuthorization.Microphone);
#endif

#if UNITY_ANDROID || UNITY_IOS
      UniWebView.SetAllowAutoPlay(true);
      UniWebView.SetAllowInlinePlay(true);
      _webView = _webViewGameObject.AddComponent<UniWebView>();

      _webView.Load(link);
      _webView.ReferenceRectTransform = _webViewFrame;

      _avatarReceiver.SetWebView(_webView);
      _webView.SetAcceptThirdPartyCookies(true);
      _webView.SetShowToolbar(true);
      _webView.AddPermissionTrustDomain(domain);
      _webView.SetUserAgent("Mozilla/5.0 (Linux; Android 12) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.5359.128 Mobile Safari/537.36");
      _webView.OnPageFinished += WebViewOnOnPageFinished;
#endif
    }

    public void ShowView(bool show)
    {
      if (show)
        _webView.Show();
      else
        _webView.Hide();
    }

    private void WebViewOnOnPageFinished(UniWebView webview, int statuscode, string url)
    {
      string jsCode = @"function loadAvaturn() {                
                          // Required overrides for mobile
                          window.avaturnForceExportHttpUrl = true;
                          window.avaturnFirebaseUseSignInWithRedirect = true;

                          // Init SDK and callback
                          window.avaturnSDKEnvironment = JSON.stringify({ engine: 'Unity', version: '__VERSION__', platform: '__PLATFORM__' });
                          window.avaturnSDK.init(null, {})
                            .then(() => window.avaturnSDK.on('export',
                              (data) => {
                                const params = new URLSearchParams();

                                ['avatarId', 'avatarSupportsFaceAnimations', 'bodyId', 'gender', 'sessionId', 'url', 'urlType'].forEach( (p) => {
                                  params.append(p, data[p] || '');
                                })
                                  
                                location.href = 'uniwebview://action?' + params.toString();
                              })
                            );
                        }

                        // Start Avaturn on page load 
                        if (document.readyState === 'loading') {
                          document.addEventListener('DOMContentLoaded', loadAvaturn);
                        } else {
                          loadAvaturn();
                        }";

      jsCode = jsCode.Replace("__VERSION__", Application.unityVersion);
      jsCode = jsCode.Replace("__PLATFORM__", GetPlatformString());
      _webView.AddJavaScript(jsCode, (payload) =>
      {
        if (payload.resultCode.Equals("0")) 
          print("Adding JavaScript Finished without error.");
      });
    }
  }
}