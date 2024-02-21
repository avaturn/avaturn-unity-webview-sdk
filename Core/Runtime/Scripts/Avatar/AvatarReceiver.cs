using System;
using Avaturn.Core.Runtime.Scripts.Avatar.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Avaturn.Core.Runtime.Scripts.Avatar
{
  public class AvatarReceiver : MonoBehaviour
  {
    private UnityAction<AvatarInfo> _onReceived;
    private UniWebView _webView;

    public void SetOnReceived(UnityAction<AvatarInfo> action) => 
      _onReceived = action;

    public void SetWebView(UniWebView webView)
    {
      _webView = webView;
      webView.OnMessageReceived += ReceiveLinkAsUniwebViewMessage;
    }

    /// <summary>
    /// This method is invoked for Mobile Iframe controller. Only works for httpURL
    /// </summary>
    private void ReceiveLinkAsUniwebViewMessage(UniWebView webview, UniWebViewMessage message)
    {
      string url = Uri.UnescapeDataString(message.Args["url"]);
      string urlType = Uri.UnescapeDataString(message.Args["urlType"]);
      string bodyId = Uri.UnescapeDataString(message.Args["bodyId"]);
      string gender = Uri.UnescapeDataString(message.Args["gender"]);
      string avatarId = Uri.UnescapeDataString(message.Args["avatarId"]);

      _onReceived?.Invoke(new AvatarInfo(url, urlType, bodyId, gender, avatarId));
    }

    /// <summary>
    /// This method is invoked for WebGL controller. URL can be either dataURL or httpURL
    /// </summary>
    public void ReceiveAvatarLink(string json)
    {
      AvatarInfo avatarInfo = JsonConvert.DeserializeObject<AvatarInfo>(json);
      _onReceived?.Invoke(avatarInfo);
    }
  }
}