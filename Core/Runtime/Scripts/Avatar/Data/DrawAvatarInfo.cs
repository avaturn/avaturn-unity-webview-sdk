using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts.Avatar.Data
{
  public class DrawAvatarInfo : MonoBehaviour
  {
    private AvatarInfo _avatarInfo;
    private readonly GUIStyle _guiStyle = new GUIStyle();

    private void Awake()
    {
      _guiStyle.fontSize = 22;
      _guiStyle.normal.textColor = new Color32(4, 0, 10, 255);
    }

    public void Draw(AvatarInfo avatarInfo) => 
      _avatarInfo = avatarInfo;

    private void OnGUI()
    {
      if(_avatarInfo == null)
        return;
      
      DrawInfo(0, $"Gender: {_avatarInfo.Gender}");
      DrawInfo(1, $"AvatarId: {_avatarInfo.AvatarId}");
      DrawInfo(2, $"BodyId: {_avatarInfo.BodyId}");
      DrawInfo(3, $"URL: {_avatarInfo.Url}");
    }

    private void DrawInfo(float step, string info) => 
      GUI.Label(new Rect(10, 60 + 30 * step, 200, 50), info, _guiStyle);
  }
}