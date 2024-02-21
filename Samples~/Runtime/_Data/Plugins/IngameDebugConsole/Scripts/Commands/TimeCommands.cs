using UnityEngine;

namespace Avaturn.Samples.Runtime._Data.Plugins.IngameDebugConsole.Scripts.Commands
{
	public class TimeCommands
	{
		[ConsoleMethod( "time.scale", "Sets the Time.timeScale value" ), UnityEngine.Scripting.Preserve]
		public static void SetTimeScale( float value )
		{
			Time.timeScale = Mathf.Max( value, 0f );
		}

		[ConsoleMethod( "time.scale", "Returns the current Time.timeScale value" ), UnityEngine.Scripting.Preserve]
		public static float GetTimeScale()
		{
			return Time.timeScale;
		}
	}
}