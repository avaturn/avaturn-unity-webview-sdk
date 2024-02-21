using System.Collections;
using UnityEngine;
using UnityEngine.Android;

namespace Avaturn.Core.Runtime.Scripts.Mobile
{
  public class RequestCameraPermission : MonoBehaviour
  {
    private IEnumerator Start()
    {
      if (Application.platform == RuntimePlatform.Android)
      {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
          Permission.RequestUserPermission(Permission.Camera);

          yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
        }
      }
      else
      {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
          yield break;
      }

      var devices = WebCamTexture.devices;
    }
  }
}