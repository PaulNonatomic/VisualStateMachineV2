using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class SubStateNodeView : BaseStateNodeView
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
			AddStyle("SubStateNodeView");
			AddTitleContainer();
			ColorizeTitle();
			AddTitleLabel();
			AddTitleIcon();
			AddStateMachineOpenButton();
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

		private void AddStateMachineOpenButton()
		{
			var openButton = new Button(() =>
			{
				var substate = (BaseSubStateMachineState) NodeModel.State;
				ModelSelection.ActiveModel = Application.isPlaying 
					? substate.SubStateMachine.Model 
					: substate.Model;
			});
			openButton.text = "Open";
			openButton.name = "open-button";

			TitleContainer.Add(openButton);
		}

		public override void Update()
		{
			UpdateProgressBar();
			UpdateGlowBorder();
		}
	}
}