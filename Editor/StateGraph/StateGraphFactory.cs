using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.StateGraph.Nodes;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public static class StateGraphFactory
	{
		public static StateTransitionModel MakeTransitionData(StateMachineModel model, string originNodeId, 
			PortModel originPort, string destinationNodeId, PortModel destinationPort)
		{
			var transition = new StateTransitionModel(originNodeId, originPort, destinationNodeId, destinationPort);
			model.AddTransition(transition);
			return transition;
		}
		
		public static void MakeTransition(GraphView graphView, StateMachineModel stateMachineModel, string originNodeId, 
			PortModel originPortModel, string destinationNodeId, 
			PortModel destinationPortModel)
		{
			var transitionData = StateGraphFactory.MakeTransitionData(stateMachineModel, 
																	  originNodeId, 
																	  originPortModel, 
																	  destinationNodeId, 
																	  destinationPortModel);
			
			var transitionView = StateGraphFactory.MakeTransitionView(graphView, transitionData);
		}
		
		public static StateTransitionModel MakeTransitionData(Edge edge)
		{
			var originNodeId = edge.output.node.name;
			var originPort = edge.output.userData as PortModel;
			
			var destinationNodeId = edge.input.node.name;
			var destinationPort = edge.input.userData as PortModel;
			
			return new StateTransitionModel(originNodeId, originPort, destinationNodeId, destinationPort);
		}
		
		public static StateNodeEdge MakeTransitionView(GraphView graphView, StateTransitionModel transitionModel)
		{
			var originNode = graphView.contentViewContainer.Q<NodeView>(transitionModel.OriginNodeId);
			if(originNode == null) throw new Exception("Failed to create edge because of missing origin node");
				
			var destinationNode = graphView.contentViewContainer.Q<NodeView>(transitionModel.DestinationNodeId);
			if(destinationNode == null) throw new Exception("Failed to create edge because of missing destination node");
				
			var inputPort = destinationNode.Q<Port>(transitionModel.DestinationPort.Id, "port", "input");
			if(inputPort == null) throw new Exception("Failed to create edge because of missing input port");
				
			var outputPort = originNode.Q<Port>(transitionModel.OriginPort.Id, "port", "output");
			if(outputPort == null) throw new Exception("Failed to create edge because of missing output port");

			var edge = new StateNodeEdge()
			{
				input = inputPort,
				output = outputPort,
				userData = transitionModel
			};
		
			var position = edge.GetPosition();
			position.position -= (Vector2) graphView.contentViewContainer.transform.position;
			edge.SetPosition(position);
			
			inputPort.Connect(edge);
			outputPort.Connect(edge); 
			graphView.AddElement(edge);

			return edge;
		}
		
		public static BaseStateNodeView MakeNode(GraphView graphView, StateNodeModel node, StateMachineModel model)
		{
			switch (node.State)
			{
				case EntryState:
					return StateGraphFactory.MakeEntryNodeView(graphView, model, node);
				case ExitState:
					return StateGraphFactory.MakeExitNodeView(graphView, model, node);
				case JumpInState:
				case JumpOutState:
					return StateGraphFactory.MakeJumpNodeView(graphView, model, node);
				default:
					return StateGraphFactory.MakeStateNodeView(graphView, model, node);
			}
		}

		public static StateNodeView MakeStateNodeView(GraphView graphView, StateMachineModel model, StateNodeModel nodeModel)
		{
			var nodeView = new StateNodeView(graphView, model, nodeModel);
			graphView.AddElement(nodeView);

			return nodeView;
		}
		
		public static EntryNodeView MakeEntryNodeView(GraphView graphView, StateMachineModel model, StateNodeModel nodeModel)
		{
			var nodeView = new EntryNodeView(graphView, model, nodeModel);
			graphView.AddElement(nodeView);

			return nodeView;
		}
		
		public static ExitNodeView MakeExitNodeView(GraphView graphView, StateMachineModel model, StateNodeModel nodeModel)
		{
			var nodeView = new ExitNodeView(graphView, model, nodeModel);
			graphView.AddElement(nodeView);

			return nodeView;
		}
		
		public static JumpNodeView MakeJumpNodeView(GraphView graphView, StateMachineModel model, StateNodeModel nodeModel)
		{
			var nodeView = new JumpNodeView(graphView, model, nodeModel);
			graphView.AddElement(nodeView);

			return nodeView;
		}

		public static StateNodeModel MakeStateNodeData(StateMachineModel model, Type stateType, Vector2 position)
		{
			var state = ScriptableObject.CreateInstance(stateType) as State;
			state.name = $"{stateType.Name}-{GUID.Generate()}";

			var stateNode = new StateNodeModel(state, position);
			model.AddState(stateNode);

			return stateNode;
		}

		public static Port MakePort(GraphView graphView, StateMachineModel model, NodeView nodeView, 
			VisualElement parent, Direction direction, Port.Capacity capacity, 
			PortModel portModel)
		{
			var stateNodeEdgeListener = new StateNodeEdgeListener(graphView, model);
			var edgeConnector = new EdgeConnector<StateNodeEdge>(stateNodeEdgeListener);

			var port = nodeView.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(Node));
			port.name = portModel.Id;
			port.userData = portModel;
			port.AddManipulator(edgeConnector);
			port.portName = portModel.PortLabel == default
				? StringUtils.ProcessPortName(portModel.Id)
				: StringUtils.ProcessPortName(portModel.PortLabel);
			
			if (ColorUtility.TryParseHtmlString(portModel.PortColor, out var color))
			{
				port.portColor = color;
			}
			
			parent.Add(port);
			
			return port;
		}
		
		public static IEnumerable<FieldInfo> GetInheritedSerializedFields(Type type)
		{
			var infoFields = new List<FieldInfo>();

			while (type != null && type != typeof(UnityEngine.Object))
			{
				var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
				infoFields.AddRange(publicFields.Where(field => field.GetCustomAttribute<HideInInspector>() == null));
				
				var privateFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				infoFields.AddRange(privateFields.Where(field => field.GetCustomAttribute<SerializeField>() != null));

				type = type.BaseType;
			}

			return infoFields.ToArray();
		}
		
		public static VisualElement MakePropertyInspector(UnityEngine.Object target, 
			List<string> propertiesToExclude = null)
		{
			var container = new VisualElement();
			var serializedObject = new SerializedObject(target);
			var fields = GetInheritedSerializedFields(target.GetType());
 
			foreach (var field in fields)
			{
				if ( propertiesToExclude != null && propertiesToExclude.Contains(field.Name)) continue;

				var serializedProperty = serializedObject.FindProperty(field.Name);
				if (serializedProperty != null)
				{
					var propertyField = new PropertyField(serializedProperty);
					container.Add(propertyField);
				}
				else
				{
					Debug.LogWarning($"Property {field.Name} not found in serialized object.");
				}
			}
			
			container.Bind(serializedObject);
			
			return container;
		}
	}
}