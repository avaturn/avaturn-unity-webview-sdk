using Avaturn.Core.Runtime.Scripts.Avatar.Data;
using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts.Avatar
{
  [RequireComponent(typeof(AvaturnIframeController))]
  public class DownloadAvatarTest : MonoBehaviour
  {
    public bool InvokeOnStart = true;
    public string URL = "https://assets.hub.in3d.io/model_2022_12_22_T182855_939_9f0cea445f.glb";

    public void Start()
    {
      if (InvokeOnStart) 
        GetComponentInChildren<AvaturnIframeController>().DownloadAvatar
          .Download(new AvatarInfo(URL, "", "DownloadAvatarTest", "DownloadAvatarTest", "DownloadAvatarTest"));
    }
  }
}