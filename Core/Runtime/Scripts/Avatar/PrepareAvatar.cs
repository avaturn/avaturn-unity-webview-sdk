using System.Threading.Tasks;
using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts.Avatar
{
  public class PrepareAvatar : MonoBehaviour
  {
    public Animator Animator;

    public void Start()
    {
      if (Animator.avatar == null)
        Animator.avatar = GetAvatar();
    }

    public async void PrepareModel(Transform downloadedModel)
    {
      if (Animator.applyRootMotion)
      {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
      }

      downloadedModel.gameObject.SetActive(false);

      DestroyPreviewSkeleton();

      await Task.Yield();

      if (MoveModelFromDownloadedToBase(downloadedModel, out Transform root))
        return;

      Animator.avatar = GetAvatar();

      await Task.Yield();

      Destroy(root.gameObject);
    }

    private void DestroyPreviewSkeleton()
    {
      for (int i = 0; i < transform.childCount; i++)
      {
        GameObject targetObject = transform.GetChild(i).gameObject;
        Destroy(targetObject);
      }
    }

    private bool MoveModelFromDownloadedToBase(Transform downloadedModel, out Transform root)
    {
      downloadedModel.transform.position = transform.position;
      downloadedModel.transform.rotation = transform.rotation;
      
      root = downloadedModel.transform.GetChild(0);

      if (!root)
      {
        Debug.LogWarning("Prepare failed. Can't find root object");
        return true;
      }

      int childCount = root.childCount;
      for (int i = 0; i < childCount; i++)
      {
        Transform child = root.GetChild(0);
        child.SetParent(transform, true);
      }

      return false;
    }

    private UnityEngine.Avatar GetAvatar() =>
      HumanoidAvatarBuilder.Build(gameObject);
  }
}