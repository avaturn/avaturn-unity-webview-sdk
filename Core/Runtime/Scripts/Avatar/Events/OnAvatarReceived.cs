using System;
using Avaturn.Core.Runtime.Scripts.Avatar.Data;
using UnityEngine.Events;

namespace Avaturn.Core.Runtime.Scripts.Avatar.Events
{
  [Serializable]
  public class OnAvatarReceived : UnityEvent<AvatarInfo>
  {
  }
}