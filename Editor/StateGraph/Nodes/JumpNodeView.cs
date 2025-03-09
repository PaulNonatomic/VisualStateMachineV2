using System;
using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.StateGraph.States;
using Nonatomic.VSM2.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class JumpNodeView : BaseStateNodeView
	{
		private Image _beacon;
		private Image _halo;
		private PopupField<string> _idDropdown;
		private JumpState _jumpState;

		public JumpNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
			
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			// Initialize _jumpState before anything else
			_jumpState = NodeModel.State as JumpState;
			if (_jumpState == null)
			{
				Debug.LogError($"JumpNodeView created with non-JumpState: {NodeModel.State?.GetType().Name}");
				return;
			}

			StyleManager.AddStyleSheet(nameof(JumpNodeView));
			AddToClassList(NodeModel.State is JumpOutState ? "jump-out" : "jump-in");
			AddBeacon(_jumpState);
			StyleManager.RemoveTitleLabel();
			AddDropdown();
			AnimationController.AddGlowBorder();

			if (NodeModel.State is JumpOutState) PortManager.AddInputPorts(StyleManager.TitleContainer);
			if (NodeModel.State is JumpInState) PortManager.AddOutputPorts(StyleManager.TitleContainer);

			UpdatePosition();

			// Make sure _idDropdown is initialized before using its value
			if (_idDropdown != null) HandleIdValueChanged(_idDropdown.value);
		}

		public override void Update()
		{
			UpdateBeaconGlow();
			AnimationController.UpdateAnimations();
		}

		protected override void HandleFocus(FocusEvent evt)
		{
			RepopulateDropdown();
		}

		private void UpdateBeaconGlow()
		{
			// Check if _halo is initialized
			if (_halo == null) return;

			var timeElapsed = Time.time - NodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = NodeModel.LastActive == 0 ? 0 : timeOpacity;

			_halo.tintColor = opacity > 0
				? new Color(0, 1, 0, opacity)
				: new Color(1, 1, 1, 0f);
		}

		private void AddBeacon(JumpState state)
		{
			// Validate state parameter
			if (state == null) return;

			var pegIcon = state is JumpOutState
				? NodeIcon.BeaconRight
				: NodeIcon.BeaconLeft;

			var haloIcon = state is JumpOutState
				? NodeIcon.BeaconHaloRight
				: NodeIcon.BeaconHaloLeft;

			_beacon = new Image
			{
				name = "beacon",
				image = ImageService.FetchTexture(pegIcon)
			};
			Add(_beacon);

			_halo = new Image
			{
				name = "halo",
				image = ImageService.FetchTexture(haloIcon)
			};
			Add(_halo);

			UpdateBeaconGlow();
		}

		private void AddDropdown()
		{
			// Check if _jumpState is properly initialized
			if (_jumpState == null) return;

			var ids = GetIds();
			if (ids == null || ids.Count == 0)
			{
				Debug.LogError("No JumpIds available for dropdown");
				return;
			}

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
			_idDropdown.RegisterValueChangedCallback(evt => { HandleIdValueChanged(evt.newValue); });

			StyleManager.TitleContainer.Add(_idDropdown);
		}

		private void RepopulateDropdown()
		{
			// Make sure _idDropdown and _jumpState are initialized
			if (_idDropdown == null || _jumpState == null) return;

			var ids = GetIds();
			if (ids == null || ids.Count == 0) return;

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
			// Add null checks
			if (_jumpState == null || string.IsNullOrEmpty(value)) return;

			_jumpState.JumpId = (JumpId)Enum.Parse(typeof(JumpId), value);

			var jumpIndex = (int)_jumpState.JumpId;
			var maxIndex = Enum.GetValues(typeof(JumpId)).Length;
			var color = ColorUtils.GetColorFromValue(jumpIndex, maxIndex, 0.5f, 200);

			var port = StyleManager.TitleContainer.Q<Port>();
			if (port != null) port.portColor = color;

			if (_idDropdown != null && _idDropdown.Children().FirstOrDefault() != null) _idDropdown.Children().First().style.backgroundColor = Color.clear;

			if (StyleManager.Title != null) StyleManager.Title.style.backgroundColor = color;

			if (_beacon != null) _beacon.tintColor = color;
		}

		private List<string> GetIds()
		{
			var ids = Enum.GetNames(typeof(JumpId)).ToList();
			var isOutput = NodeModel.State is JumpOutState;
			if (isOutput) return ids;

			//filter out used ids;
			var jumpNodes = GraphView.Query<JumpNodeView>().ToList();
			foreach (var node in jumpNodes)
			{
				if (node.name == name) continue;
				if (node.NodeModel.State is not JumpInState) continue;

				var jumpIn = node.NodeModel.State as JumpInState;
				if (jumpIn != null) ids.Remove(jumpIn.JumpId.ToString());
			}

			return ids;
		}
	}
}