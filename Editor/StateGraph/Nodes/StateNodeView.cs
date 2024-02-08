using System;
using System.Reflection;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class StateNodeView : BaseStateNodeView
	{
		public StateNodeModel NodeModel => _nodeModel;
		
		private readonly StateNodeModel _nodeModel;
		private readonly StateMachineModel _model;
		private readonly Type _stateType;
		private VisualElement _titleContainer;
		private VisualElement _title;
		private GraphView _graphView;
		private VisualElement _glowBorder;

		public StateNodeView(GraphView graphView, StateMachineModel model,  StateNodeModel nodeModel)
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
			AddTitleIcon();
			AddProgressBar();
			AddInputPorts();
			AddOutputPorts();
			AddProperties();
			AddGlowBorder();
			CheckCustomWidth();
			UpdatePosition(_nodeModel, _model);
			
			RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
			RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
			RegisterCallback<DetachFromPanelEvent>(HandleLeavePanel);
		}

		private void AddGlowBorder()
		{
			_glowBorder = new VisualElement();
			_glowBorder.name = "state-border";
			_glowBorder.pickingMode = PickingMode.Ignore;
			this.Add(_glowBorder);
		}

		private void CheckCustomWidth()
		{
			var stateType = _nodeModel.State.GetType();
			var nodeWidth = stateType.GetCustomAttribute<NodeWidthAttribute>();
			if (nodeWidth != null)
			{
				this.style.maxWidth = nodeWidth.Width;
				this.style.minWidth = nodeWidth.Width;
				this.style.width = nodeWidth.Width;
			}
		}

		public override void Update()
		{
			UpdateProgressBar();
			UpdateGlowBorder();
		}

		private void UpdateGlowBorder()
		{
			var timeElapsed = Time.time - _nodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = _nodeModel.LastActive == 0 ? 0 : timeOpacity;
			
			_glowBorder.style.opacity = opacity;
		}

		private void UpdateProgressBar()
		{
			var progressBar = this.Query<ProgressBar>().First();
			var progressFill = progressBar.Q(className:"unity-progress-bar__progress");
			progressFill.visible = _nodeModel.Active;
			
			if(progressBar != null) progressBar.value = (Time.time % 1f) * 100f;
		}

		private void AddStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>("StateNodeView");
			Assert.IsNotNull(style, "StateNodeView.uss not found");
			styleSheets.Add(style);
		}

		private void AddProgressBar()
		{
			var progressBar = new ProgressBar();
			progressBar.name = "progress-bar";
			
			var title = this.Query<VisualElement>("title").First();
			title.Add(progressBar);
		}

		private void AddInputPorts()
		{
			for (var index = 0; index < _nodeModel.InputPorts.Count; index++)
			{
				var portData = _nodeModel.InputPorts[index];
				ApplyStateColorToPortData(_nodeModel, portData);
				StateGraphPortFactory.MakePort(_graphView, _model, this,
					inputContainer, Direction.Input, Port.Capacity.Multi, portData);
			}
		}

		private void AddOutputPorts()
		{
			for (var index = 0; index < _nodeModel.OutputPorts.Count; index++)
			{
				var portData = _nodeModel.OutputPorts[index];
				TryUpdatePortDataFromState(_nodeModel, portData.Id, out portData);

				StateGraphPortFactory.MakePort(_graphView, _model, this,
					outputContainer, Direction.Output, Port.Capacity.Single, portData);
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

		private void AddProperties()
		{
			var contents = this.Query<VisualElement>("contents").First();
			var propertyContainer = new VisualElement()
			{
				name = "property-container"
			};
			
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);
			
			var scrollView = new ScrollView();
			propertyContainer.Add(scrollView);
				
			var stateInspector = MakePropertyInspector(_nodeModel.State);
			stateInspector.name = "state-inspector";
			scrollView.contentContainer.Add(stateInspector);
				
			if (stateInspector.childCount > 0)
			{
				propertyContainer.AddToClassList("has-properties");
			}
		}
	}
}