using System;
using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.StateGraph.States;
using Nonatomic.VSM2.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class JumpNodeView : BaseStateNodeView
	{
		private PopupField<string> _idDropdown;
		private JumpState _jumpState;
		private Image _beacon;
		private Image _halo;

		public JumpNodeView(GraphView graphView, 
							StateMachineModel stateMachineModel,  
							StateNodeModel nodeModel) 
							: base(graphView, stateMachineModel, nodeModel)
		{
			_jumpState = NodeModel.State as JumpState;

			AddStyle("JumpNodeView");
			AddToClassList(NodeModel.State is JumpOutState ? "jump-out" : "jump-in");
			AddBeacon(_jumpState);
			AddTitleContainer();
			ColorizeTitle();
			RemoveTitleLabel();
			AddDropdown();
			AddGlowBorder();
			
			if(NodeModel.State is JumpOutState) AddInputPorts(TitleContainer);
			if(NodeModel.State is JumpInState) AddOutputPorts(TitleContainer);
			
			UpdatePosition();
			
			HandleIdValueChanged(_idDropdown.value);
		}
	
		public override void Update()
		{
			UpdateBeaconGlow();
			UpdateGlowBorder();
		}

		protected override void HandleFocus(FocusEvent evt)
		{
			RepopulateDropdown();
		}

		private void UpdateBeaconGlow()
		{
			var timeElapsed = Time.time - NodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = NodeModel.LastActive == 0 ? 0 : timeOpacity;

			_halo.tintColor = opacity > 0 
				? new Color(0, 1, 0, opacity) 
				: new Color(1, 1, 1, 0f);
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
			_idDropdown.RegisterCallback<PointerDownEvent>(evt => RepopulateDropdown());
			_idDropdown.RegisterCallback<PointerOverEvent>(evt => RepopulateDropdown());
			_idDropdown.RegisterValueChangedCallback(evt =>
			{
				HandleIdValueChanged(evt.newValue);
			});

			TitleContainer.Add(_idDropdown);
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

			_idDropdown.choices = ids;
			_idDropdown.value = currentId;

			HandleIdValueChanged(_idDropdown.value);
		}

		private void HandleIdValueChanged(string value)
		{
			_jumpState.JumpId = (JumpId) Enum.Parse(typeof(JumpId), value);
			
			var jumpIndex = (int) _jumpState.JumpId;
			var maxIndex = Enum.GetValues(typeof(JumpId)).Length;
			var color = ColorUtils.GetColorFromValue(jumpIndex, maxIndex, 0.5f, 200);
			var port = TitleContainer.Q<Port>();
			port.portColor = color;
			
			_idDropdown.Children().First().style.backgroundColor = Color.clear;
			Title.style.backgroundColor = color;
			_beacon.tintColor = color;
		}

		private List<string> GetIds()
		{
			var ids = Enum.GetNames(typeof(JumpId)).ToList();
			var isOutput = NodeModel.State is JumpOutState;
			if (isOutput) return ids;
			
			//filter out used ids;
			var jumpNodes = GraphView.Query<JumpNodeView>().ToList();
			foreach(var node in jumpNodes)
			{
				if (node.name == this.name) continue;
				if (node.NodeModel.State is not JumpInState) continue;
				
				var jumpIn = node.NodeModel.State as JumpInState;
				ids.Remove(jumpIn.JumpId.ToString());
			}
			
			return ids;
		}
	}
}