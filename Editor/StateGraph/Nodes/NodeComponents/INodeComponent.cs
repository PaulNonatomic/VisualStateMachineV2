using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes.NodeComponents
{
	public interface INodeComponent
	{
		void Initialize(BaseStateNodeView node);
	}
}