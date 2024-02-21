using System;
using UnityEngine;

namespace Avaturn.Core.Runtime.Scripts.WebGL
{
  public class MenuWebGL : MonoBehaviour
  {
    [SerializeField] private KeyCode _key;
    
    [SerializeField] private AvaturnIframeControllerWebGL _frameController;
    [SerializeField] private GameObject _openButton, _hideButton;
    
    [SerializeField] private bool _isOpen;
    [SerializeField] private bool _isLockCursor;

    private void Awake()
    {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
      if (Input.GetKeyDown(_key))
        Switch();
    }

    public void Open() =>
      DefinedSwitch(true);

    public void Close() =>
      DefinedSwitch(false);

    public void Switch() =>
      DefinedSwitch(!_isOpen);

    private void DefinedSwitch(bool isOpen)
    {
      _isOpen = isOpen;
      _frameController.ChangeVisibility();
      _hideButton.SetActive(_isOpen);
      _openButton.SetActive(!_isOpen);

      if (_isLockCursor)
      {
        Cursor.visible = _isOpen;
        Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
      }
    }
  }
}