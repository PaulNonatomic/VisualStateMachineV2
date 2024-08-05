using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Utils
{
	public static class GraphUtils
	{
		public static Vector3 ScreenPointToGraphPoint(Vector2 screenPoint, GraphView graphView)
		{
			return (Vector3)screenPoint - graphView.contentViewContainer.transform.position;
		}
		
		public static Vector2 ScreenPointToGraphPointWithZoom(Vector2 screenPoint, GraphView graphView) 
		{
			var scale = graphView.contentViewContainer.transform.scale;
			var offset = screenPoint - (Vector2)graphView.contentViewContainer.transform.position;
			var scaledOffset = new Vector2(offset.x / scale.x, offset.y / scale.y);

			return scaledOffset;
		}
	}
}