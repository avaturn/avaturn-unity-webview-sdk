using UnityEngine;

namespace Avaturn.Samples.Runtime._Data.Plugins.Third_Person_Controller.Scripts
{
  public class VirtualControllerInput : MonoBehaviour
  {
    public PlayerInput PlayerInput;

    public void VirtualMoveInput(Vector2 virtualMoveDirection) =>
      PlayerInput.MoveInput(virtualMoveDirection);

    public void VirtualLookInput(Vector2 virtualLookDirection) =>
      PlayerInput.LookInput(virtualLookDirection);

    public void VirtualJumpInput(bool virtualJumpState) =>
      PlayerInput.JumpInput(virtualJumpState);

    public void VirtualSprintInput(bool virtualSprintState) =>
      PlayerInput.SprintInput(virtualSprintState);
  }
}