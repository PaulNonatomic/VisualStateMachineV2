using System;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class ExitNodeView : BaseStateNodeView
	{
		private readonly StateNodeModel _nodeModel;
		private readonly StateMachineModel _model;
		private readonly Type _stateType;
		private VisualElement _titleContainer;
		private VisualElement _title;
		private GraphView _graphView;
		private VisualElement _glowBorder;

		public ExitNodeView(GraphView graphView, StateMachineModel model,  StateNodeModel nodeModel)
		{
			this.name = nodeModel.Id;
			this.userData = nodeModel;

			_graphView = graphView;
			_model = model;
			_nodeModel = nodeModel;
			_stateType = nodeModel.State.GetType();
			
			AddStyle();
			AddTitleContainer();
			ColorizeTitle(_nodeModel);
			AddTitleLabel();
			AddGlowBorder();
			AddTitleIcon();
			AddInputPorts();
			UpdatePosition(_nodeModel, _model);
			
			RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
			RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
			RegisterCallback<DetachFromPanelEvent>(HandleLeavePanel);
		}
		
		public override void Update()
		{
			UpdateGlowBorder();
		}
		
		private void AddGlowBorder()
		{
			_glowBorder = new VisualElement();
			_glowBorder.name = "state-border";
			_glowBorder.pickingMode = PickingMode.Ignore;
			this.Add(_glowBorder);
		}
		
		private void UpdateGlowBorder()
		{
			var timeElapsed = Time.time - _nodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = _nodeModel.LastActive == 0 ? 0 : timeOpacity;
			
			_glowBorder.style.opacity = opacity;
		}
		
		private void AddStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>("ExitNodeView");
			Assert.IsNotNull(style, "ExitNodeView.uss not found");
			styleSheets.Add(style);
		}
		
		private void AddInputPorts()
		{
			for (var index = 0; index < _nodeModel.InputPorts.Count; index++)
			{
				var portData = _nodeModel.InputPorts[index];
				ApplyStateColorToPortData(_nodeModel, portData);
				StateGraphFactory.MakePort(_graphView, _model, this,
					_titleContainer, Direction.Input, Port.Capacity.Multi, portData);
			}
		}

		private void HandleAttachToPanel(AttachToPanelEvent evt)
		{
			
		}

		private void HandleLeavePanel(DetachFromPanelEvent evt)
		{
			
		}

		private void AddTitleContainer()
		{
			_title = this.Query<VisualElement>("title").First();

			var titleButton = _title.Query<VisualElement>("title-button-container").First();
			_title.Remove(titleButton);
			
			_titleContainer = new VisualElement();
			_titleContainer.name = "title-container";
			_title.Add(_titleContainer);
		}

		private void AddTitleLabel()
		{
			var titleString = _stateType.Name;
			this.title = StringUtils.ProcessNodeTitle(titleString);
			
			var titleLabel = _title.Query<VisualElement>("title-label").First();
			_titleContainer.Add(titleLabel);
		}

		private void AddTitleIcon()
		{
			var icon = MakeIcon(_nodeModel);
			_titleContainer.Insert(0, icon);
		}
		
		private void HandleGeometryChanged(GeometryChangedEvent evt)
		{
			if (evt.oldRect.position != evt.newRect.position)
			{
				_nodeModel.Position = evt.newRect.position;
			}
		}
	}
}