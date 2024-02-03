using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateNodeEdge : Edge
	{
		protected override EdgeControl CreateEdgeControl() => new EdgeControl()
		{
			capRadius = 4f,
			interceptWidth = 6f
		};
		
	}
}