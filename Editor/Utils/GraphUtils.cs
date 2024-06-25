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
	}
}