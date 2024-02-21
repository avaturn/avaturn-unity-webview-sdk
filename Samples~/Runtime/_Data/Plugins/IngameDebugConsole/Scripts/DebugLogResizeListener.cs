using UnityEngine;
using UnityEngine.EventSystems;

// Listens to drag event on the DebugLogManager's resize button
namespace Avaturn.Samples.Runtime._Data.Plugins.IngameDebugConsole.Scripts
{
	public class DebugLogResizeListener : MonoBehaviour, IBeginDragHandler, IDragHandler
	{
#pragma warning disable 0649
		[SerializeField]
		private DebugLogManager debugManager;
#pragma warning restore 0649

		// This interface must be implemented in order to receive drag events
		void IBeginDragHandler.OnBeginDrag( PointerEventData eventData )
		{
		}

		void IDragHandler.OnDrag( PointerEventData eventData )
		{
			debugManager.Resize( eventData );
		}
	}
}