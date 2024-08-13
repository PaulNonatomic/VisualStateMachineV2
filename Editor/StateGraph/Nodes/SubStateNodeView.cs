using System;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class SubStateNodeView : BaseStateNodeView
	{
		private readonly StateMachineModel _model;
		private readonly Type _stateType;
		private VisualElement _titleContainer;
		private VisualElement _title;
		private GraphView _graphView;
		private VisualElement _glowBorder;

		public SubStateNodeView(GraphView graphView, 
								StateMachineModel stateMachineModel,  
								StateNodeModel nodeModel) 
								: base(graphView, stateMachineModel, nodeModel)
		{
			AddStyle(nameof(SubStateNodeView));
			AddTitleContainer();
			ColorizeTitle();
			AddTitleLabel();
			AddTitleIcon();
			AddProgressBar();
			AddInputPorts(inputContainer);
			AddOutputPorts(outputContainer);

			var contents = this.Query<VisualElement>("contents").First();
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