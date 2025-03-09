using Nonatomic.VSM2.Editor.StateGraph.Factories;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes.Base
{
	public class NodePortManager
	{
		private readonly GraphView _graphView;
		private readonly StateNodeModel _nodeModel;
		private readonly BaseStateNodeView _nodeView;
		private readonly StateMachineModel _stateMachineModel;

		public NodePortManager(BaseStateNodeView nodeView, GraphView graphView,
			StateMachineModel stateMachineModel, StateNodeModel nodeModel)
		{
			_nodeView = nodeView;
			_graphView = graphView;
			_stateMachineModel = stateMachineModel;
			_nodeModel = nodeModel;
		}

		public void AddInputPorts(VisualElement portContainer)
		{
			foreach (var portData in _nodeModel.InputPorts)
			{
				ApplyStateColorToPortData(_nodeModel, portData);

				StateGraphPortFactory.MakePort(_graphView,
					_stateMachineModel,
					_nodeView,
					portContainer,
					Direction.Input,
					Port.Capacity.Multi,
					portData);
			}
		}

		public void AddOutputPorts(VisualElement portContainer)
		{
			foreach (var portData in _nodeModel.OutputPorts)
			{
				var updatedPortData = portData;
				TryUpdatePortDataFromState(_nodeModel, portData.Id, out updatedPortData);

				StateGraphPortFactory.MakePort(_graphView,
					_stateMachineModel,
					_nodeView,
					portContainer,
					Direction.Output,
					Port.Capacity.Single,
					updatedPortData);
			}
		}

		public void RemovePortContainer()
		{
			var top = _nodeView.Q<VisualElement>("top");
			top?.parent.Remove(top);

			var divider = _nodeView.Q<VisualElement>("divider");
			divider?.parent.Remove(divider);
		}

		private void ApplyStateColorToPortData(StateNodeModel nodeModel, PortModel portModel)
		{
			var stateType = nodeModel.State.GetType();

			if (AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt)) portModel.PortColor = colorAtt.HexColor;
		}

		private bool TryUpdatePortDataFromState(StateNodeModel nodeModel, string portId, out PortModel portModel)
		{
			portModel = null;

			var stateType = nodeModel.State.GetType();

			var eventInfo = stateType.GetEvent(portId);
			if (eventInfo == null) return false;

			var attributes = eventInfo.GetCustomAttributes(typeof(TransitionAttribute), false);
			if (attributes.Length == 0) return false;

			var transAtt = (TransitionAttribute)attributes[0];
			portModel = transAtt.GetPortData(eventInfo, 0);

			if (portModel.PortColor == default &&
				AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt))
				portModel.PortColor = colorAtt.HexColor;

			return true;
		}
	}
}