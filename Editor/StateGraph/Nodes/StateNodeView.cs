using System;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class StateNodeView : BaseStateNodeView
	{
		private readonly StateMachineModel _model;
		private readonly Type _stateType;
		private VisualElement _titleContainer;
		private VisualElement _title;
		private GraphView _graphView;
		private VisualElement _glowBorder;

		public StateNodeView(GraphView graphView, 
							 StateMachineModel stateMachineModel,  
							 StateNodeModel nodeModel) 
							 : base(graphView, stateMachineModel, nodeModel)
		{
			
			var contents = this.Query<VisualElement>("contents").First();
			
			AddStyle("StateNodeView");
			AddTitleContainer();
			ColorizeTitle();
			AddTitleLabel();
			AddTitleIcon();
			AddProgressBar();
			AddInputPorts(inputContainer);
			AddOutputPorts(outputContainer);
			
			var propertyContainer = CreatePropertyContainer();
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);
			
			AddProperties(propertyContainer);
			AddGlowBorder();
			CheckCustomWidth();
			UpdatePosition();
		}

		public override void Update()
		{
			UpdateProgressBar();
			UpdateGlowBorder();
		}
	}
}