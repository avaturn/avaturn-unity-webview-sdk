using System;
using UnityEngine;
using UnityEngine.Events;

namespace Avaturn.Core.Runtime.Scripts.Avatar.Events
{
  [Serializable]
  public class OnAvatarDownloaded: UnityEvent<Transform>
  {
  }
}