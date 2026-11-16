using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>Continuous yaw from horizontal stick input, without cardinal snapping or instant U-turns.</summary>
    [AddComponentMenu("XR/Locomotion/Smooth Joystick Turn Provider")]
    public class SmoothJoystickTurnProvider : ContinuousTurnProvider
    {
        protected override float GetTurnAmount(Vector2 input)
        {
            // The input action supplies the stick deadzone. Preserve proportional speed
            // on diagonals and stop immediately when the stick returns to center.
            return enableTurnLeftRight
                ? Mathf.Clamp(input.x, -1f, 1f) * turnSpeed * Time.deltaTime
                : 0f;
        }
    }
}
