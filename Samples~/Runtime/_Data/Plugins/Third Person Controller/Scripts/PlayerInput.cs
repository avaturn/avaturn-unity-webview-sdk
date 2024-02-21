using UnityEngine;

namespace Avaturn.Samples.Runtime._Data.Plugins.Third_Person_Controller.Scripts
{
  public class PlayerInput : MonoBehaviour
  {
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    private const string MOUSE_AXIS_X = "Mouse X";
    private const string MOUSE_AXIS_Y = "Mouse Y";
    private const string JUMP_BUTTON = "Jump";

    public float MouseSensitivityX = 3;
    public float MouseSensitivityY = 3;

    public float AxisHorizontal { get; private set; }
    public float AxisVertical { get; private set; }
    public float MouseAxisX { get; private set; }
    public float MouseAxisY { get; private set; }
    public bool Sprint { get; private set; }
    public bool Jump { get; set; }
    public Vector2 Look => new Vector2(MouseAxisX, MouseAxisY);
    public Vector2 Move => new Vector2(AxisHorizontal, AxisVertical);

    public void CheckInput()
    {
#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_ANDROID)
      AxisHorizontal = Input.GetAxis(HORIZONTAL_AXIS);
      AxisVertical = Input.GetAxis(VERTICAL_AXIS);
      MouseAxisX = Input.GetAxis(MOUSE_AXIS_X) * (MouseSensitivityX * 50);
      MouseAxisY = Input.GetAxis(MOUSE_AXIS_Y) * (MouseSensitivityY * -50);
      Sprint = Input.GetKey(KeyCode.LeftShift);
      Jump = Input.GetButtonDown(JUMP_BUTTON);
#endif
    }

    public void MoveInput(Vector2 moveDirection)
    {
      AxisHorizontal = moveDirection.x;
      AxisVertical = moveDirection.y;
    }

    public void LookInput(Vector2 lookDirection)
    {
      MouseAxisX = lookDirection.x * MouseSensitivityX;
      MouseAxisY = lookDirection.y * MouseSensitivityY;
    }

    public void JumpInput(bool value) =>
      Jump = value;

    public void SprintInput(bool value) =>
      Sprint = value;
  }
}