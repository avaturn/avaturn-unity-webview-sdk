using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts
{
  public class PlatformInfo : MonoBehaviour
  {
    private void Start()
    {
#if !UNITY_EDITOR || !UNITY_STANDALONE_WIN
      gameObject.SetActive(false);
#endif
    }
  }
}