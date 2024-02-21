using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Avaturn.Core.Runtime.Scripts.Mobile
{
  public class MenuMobile : MonoBehaviour
  {
    [SerializeField] private bool _isOpen;
    
    [SerializeField] private AvaturnIframeControllerMobile _frameController;
    [SerializeField] private UnityEvent _openEvent, _closeEvent;

    public void Open() => 
      DefinedSwitch(true);

    public void Close() => 
      DefinedSwitch(false);

    public void Switch() => 
      DefinedSwitch(!_isOpen);

    private void DefinedSwitch(bool isOpen)
    {
      _isOpen = isOpen;
      _frameController.ShowView(_isOpen);
      
      if (isOpen)
        _openEvent?.Invoke();
      else
        _closeEvent?.Invoke();
    }
  }
}