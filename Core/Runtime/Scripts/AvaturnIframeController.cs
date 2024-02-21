using Avaturn.Core.Runtime.Scripts.Avatar;
using Avaturn.Core.Runtime.Scripts.Avatar.Data;
using Avaturn.Core.Runtime.Scripts.Avatar.Events;
using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts
{
  public abstract class AvaturnIframeController : MonoBehaviour
  {
    public DownloadAvatar DownloadAvatar => _downloadAvatar;

    [Space(10)] [SerializeField] protected AvatarReceiver _avatarReceiver;
    [Space(10)] [SerializeField] private OnAvatarReceived _onAvatarReceived;

    [Space(10)] [SerializeField] protected DownloadAvatar _downloadAvatar;
    [Space(10)] [SerializeField] private OnAvatarDownloaded _onAvatarDownloaded;

    [Space(10)] [SerializeField] protected string _subdomain = "demo";
    [SerializeField] protected string _linkFromAPI = "";

    public virtual void Awake()
    {
      QualitySettings.skinWeights = SkinWeights.FourBones;

      _avatarReceiver.SetOnReceived(OnAvatarReceived);
      _downloadAvatar.SetOnDownloaded(OnAvatarDownloaded);
    }

    public virtual void Start()
    {
    }

    protected virtual void OnAvatarReceived(AvatarInfo avatarInfo) =>
      _onAvatarReceived?.Invoke(avatarInfo);

    protected virtual void OnAvatarDownloaded(Transform transform) =>
      _onAvatarDownloaded?.Invoke(transform);

    protected string GetPlatformString()
    {
#if UNITY_EDITOR
      return "editor";
#elif UNITY_IOS
      return "ios";
#elif UNITY_ANDROID
      return "android";
#elif UNITY_WEBGL
      return "webgl";
#elif UNITY_STANDALONE_WIN
      return "windows";
#elif UNITY_STANDALONE_OSX
      return "mac";
#elif UNITY_STANDALONE_LINUX
      return "linux";
#else
      return "unknown";
#endif
    }
  }
}