namespace Avaturn.Core.Runtime.Scripts.Avatar.Data
{
  public class AvatarInfo
  {
    public readonly string Url;
    public readonly string UrlType;
    public readonly string BodyId;
    public readonly string Gender;
    public readonly string AvatarId;

    public AvatarInfo(string url, string urlType, string bodyId, string gender, string avatarId)
    {
      Url = url;
      UrlType = urlType;
      BodyId = bodyId;
      Gender = gender;
      AvatarId = avatarId;
    }
  }
}