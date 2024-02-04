using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.StateGraph.States;
using Nonatomic.VSM2.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class JumpNodeView : BaseStateNodeView
	{
		public StateNodeModel NodeModel => _nodeModel;
		
		private readonly StateNodeModel _nodeModel;
		private readonly StateMachineModel _model;
		private readonly Type _stateType;
		private VisualElement _titleContainer;
		private VisualElement _title;
		private GraphView _graphView;
		private PopupField<string> _idDropdown;
		private JumpState _jumpState;
		private Image _beacon;
		private Image _halo;
		private VisualElement _glowBorder;

		public JumpNodeView(GraphView graphView, StateMachineModel model,  StateNodeModel nodeModel)
		{
			this.name = nodeModel.Id;
			this.userData = nodeModel;

			_graphView = graphView;
			_model = model;
			_nodeModel = nodeModel;
			_stateType = nodeModel.State.GetType();
			_jumpState = _nodeModel.State as JumpState;

			AddBeacon(_jumpState);
			AddStyle();
			AddTitleContainer();
			ColorizeTitle(_nodeModel);
			RemoveTitleLabel();
			AddDropdown();
			AddGlowBorder();
			AddInputPorts();
			AddOutputPorts();
			UpdatePosition(_nodeModel, _model);
			
			HandleIdValueChanged(_idDropdown.value);
			
			RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
			RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
			RegisterCallback<DetachFromPanelEvent>(HandleLeavePanel);
			RegisterCallback<FocusEvent>(e => HandleFocus());
		}
		
		private void AddGlowBorder()
		{
			_glowBorder = new VisualElement();
			_glowBorder.name = "state-border";
			_glowBorder.pickingMode = PickingMode.Ignore;
			this.Add(_glowBorder);
		}
		
		public override void Update()
		{
			UpdateBeaconGlow();
			UpdateGlowBorder();
		}
		
		private void UpdateGlowBorder()
		{
			var timeElapsed = Time.time - _nodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = _nodeModel.LastActive == 0 ? 0 : timeOpacity;
			
			_glowBorder.style.opacity = opacity;
		}

		private void UpdateBeaconGlow()
		{
			var timeElapsed = Time.time - _nodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = _nodeModel.LastActive == 0 ? 0 : timeOpacity;

			if (opacity > 0)
			{
				_halo.tintColor = new Color(0, 1, 0, opacity);
			}
			else
			{
				_halo.tintColor = new Color(1, 1, 1, 0f);
			}
			
		}

		private void AddBeacon(JumpState state)
		{
			var pegIcon = state is JumpOutState 
				? NodeIcon.V2_BeaconRight 
				: NodeIcon.V2_BeaconLeft;
			
			var haloIcon = state is JumpOutState 
				? NodeIcon.V2_BeaconHaloRight 
				: NodeIcon.V2_BeaconHaloLeft;
			
			_beacon = new Image();
			_beacon.name = "beacon";
			_beacon.image = ImageService.FetchTexture(pegIcon, ResourceSource.Resources);
			Add(_beacon);

			_halo = new Image();
			_halo.name = "halo";
			_halo.image = ImageService.FetchTexture(haloIcon, ResourceSource.Resources);
			Add(_halo);

			UpdateBeaconGlow();
		}

		private void AddDropdown()
		{
			var ids = GetIds();
			var currentId = _jumpState.JumpId.ToString();

			if (!ids.Contains(currentId))
			{
				currentId = ids[0];
				_jumpState.JumpId = Enum.Parse<JumpId>(currentId);
			}
			
			_idDropdown = new PopupField<string>("", ids, currentId);
			_idDropdown.AddToClassList("center-aligned-text");
			_idDropdown.RegisterValueChangedCallback(evt =>
			{
				HandleIdValueChanged(evt.newValue);
			});
			_idDropdown.RegisterCallback<PointerDownEvent>(evt => RepopulateDropdown());
			_idDropdown.RegisterCallback<PointerOverEvent>(evt => RepopulateDropdown());
				
			_titleContainer.Add(_idDropdown);
		}

		private void HandleFocus()
		{
			RepopulateDropdown();
		}

		private void RepopulateDropdown()
		{
			var ids = GetIds();
			var currentId = _jumpState.JumpId.ToString();
			
			if (!ids.Contains(currentId) && ids.Count > 0)
			{
				_jumpState.JumpId = (JumpId)Enum.Parse(typeof(JumpId), ids[0]);
				currentId = ids[0];
			}

			_idDropdown.choices = ids; // Update the choices
			_idDropdown.value = currentId; // Ensure the selected value is valid

			HandleIdValueChanged(_idDropdown.value);
		}
		
		private void HandleIdValueChanged(string value)
		{
			_jumpState.JumpId = (JumpId) Enum.Parse(typeof(JumpId), value);
			
			var jumpIndex = (int)_jumpState.JumpId;
			var maxIndex = Enum.GetValues(typeof(JumpId)).Length;
			var color = ColorUtils.GetColorFromValue(jumpIndex, maxIndex, 0.5f);
			var port = _titleContainer.Q<Port>();
			port.portColor = color;
			
			color.a = 0.8f;
			_idDropdown.Children().First().style.backgroundColor = Color.clear;
			_title.style.backgroundColor = color;
			_beacon.tintColor = color;
		}

		private List<string> GetIds()
		{
			var ids = Enum.GetNames(typeof(JumpId)).ToList();
			var isOutput = _nodeModel.State is JumpOutState;
			if (isOutput) return ids;
			
			//filter out used ids;
			var jumpNodes = _graphView.Query<JumpNodeView>().ToList();
			foreach(var node in jumpNodes)
			{
				if (node == this) continue;
				if (node.NodeModel.State is not JumpInState) continue;
				
				var jumpIn = node.NodeModel.State as JumpInState;
				ids.Remove(jumpIn.JumpId.ToString());
			}
			
			return ids;
		}

		private void AddStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>("JumpNodeView");
			Assert.IsNotNull(style, "JumpNodeView.uss not found");
			styleSheets.Add(style);

			AddToClassList(_nodeModel.State is JumpOutState ? "jump-out" : "jump-in");
		}
		
		private void AddInputPorts()
		{
			if(_nodeModel.State is not JumpOutState) return;
			
			for (var index = 0; index < _nodeModel.InputPorts.Count; index++)
			{
				var portData = _nodeModel.InputPorts[index];
				ApplyStateColorToPortData(_nodeModel, portData);
				StateGraphFactory.MakePort(_graphView, _model, this,
					_titleContainer, Direction.Input, Port.Capacity.Multi, portData);
			}
		}

		private void AddOutputPorts()
		{
			if(_nodeModel.State is not JumpInState) return;
			
			var stateType = _nodeModel.State.GetType();
			var events = stateType.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			for (var index = 0; index < _nodeModel.OutputPorts.Count; index++)
			{
				var portData = _nodeModel.OutputPorts[index];
				TryUpdatePortDataFromState(_nodeModel, portData.Id, out portData);

				StateGraphFactory.MakePort(_graphView, _model, this,
					_titleContainer, Direction.Output, Port.Capacity.Single, portData);
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

		private void RemoveTitleLabel()
		{
			var titleLabel = _title.Query<VisualElement>("title-label").First();
			if (titleLabel == null) return;
			
			titleLabel.parent.Remove(titleLabel);
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