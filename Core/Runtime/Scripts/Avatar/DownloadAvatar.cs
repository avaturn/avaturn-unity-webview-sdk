using System;
using System.Threading.Tasks;
using Avaturn.Core.Runtime.Scripts.Avatar.Data;
using GLTFast;
using GLTFast.Loading;
using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts.Avatar
{
  /// <summary>
  /// This example of loading avatar model.
  /// </summary>
  [RequireComponent(typeof(GltfAsset))]
  public class DownloadAvatar : MonoBehaviour
  {
    private Action<Transform> _onDownloaded;
    
    public void SetOnDownloaded(Action<Transform> avatarDownloaded) => 
      _onDownloaded = avatarDownloaded;

    public async void Download(AvatarInfo avatarInfo)
    {
      string url = avatarInfo.Url;
      
      if (string.IsNullOrEmpty(url))
      {
        Debug.LogError("Fail to download: url is empty");
        return;
      }

      Debug.Log($"Start download...\nUrl = {url}");

      // Loading via GltFast loader
      var asset = GetComponent<GltfAsset>();
      asset.ClearScenes();
      var success = await asset.Load(url, new AvaturnDownloadProvider());
      
      if (success)
        _onDownloaded?.Invoke(transform);
      else
        Debug.LogError($"Fail to download");
    }

    public async Task<IDownload> Request(Uri url)
    {
      var req = new AvaturnAwaitableDownload(url);
      await req.WaitAsync();
      
      return req;
    }
  }
}