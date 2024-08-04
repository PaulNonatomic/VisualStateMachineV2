using System;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateNodeEdge : Edge
	{
		public int FrameDelay
		{
			get => _frameDelay;
			set
			{
				_frameDelay = value;
				_durationLabel.text = $"{value}";
			}
		}
		
		private Color _originalInputColor;
		private Color _originalOutputColor;
		private Color _targetColor;
		private bool _isFlashing;
		private float _flashDuration;
		private float _flashStartTime;
		private Label _durationLabel;
		private int _frameDelay;

		public StateNodeEdge() : base()
		{
			RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(HandleAttach));
			RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(HandleDetach));

			ApplyStyle();
			CreateDurationLabel();
		}

		private void ApplyStyle()
		{
			var styleSheet = Resources.Load<StyleSheet>(nameof(StateNodeEdge));
			styleSheets.Add(styleSheet);
		}

		private void CreateDurationLabel()
		{
			_durationLabel = new Label();
			_durationLabel.name = "durationLabel";
			this.Add(_durationLabel);
		}

		private void HandleDetach(DetachFromPanelEvent evt)
		{
			if (userData is not StateTransitionModel transitionModel) return;
			transitionModel.OnTransitionTriggered -= HandleFlash;
		}

		private void HandleAttach(AttachToPanelEvent evt)
		{
			if (userData is not StateTransitionModel transitionModel) return;
			transitionModel.OnTransitionTriggered += HandleFlash;
		}

		private void HandleFlash()
		{
			if (!ColorUtility.TryParseHtmlString("#4CAF50", out var color)) return;
			FlashEdge(color, 0.75f);
		}

		protected override EdgeControl CreateEdgeControl() 
		{
			var edgeController = new EdgeControl() { capRadius = 4f, interceptWidth = 6f };
			edgeController.RegisterCallback<GeometryChangedEvent>(UpdateLabelPosition);
			return edgeController;
		}
		
		public virtual void SetEdgeColor(Color inputColor, Color outputColor)
		{
			if (edgeControl == null) return;
			
			edgeControl.inputColor = inputColor;
			edgeControl.outputColor = outputColor;
		}

		public virtual void FlashEdge(Color color, float duration)
		{
			if (_isFlashing) return;

			_originalInputColor = edgeControl.inputColor;
			_originalOutputColor = edgeControl.outputColor;
			_targetColor = color;
			_flashDuration = duration;
			_flashStartTime = (float)EditorApplication.timeSinceStartup;
			_isFlashing = true;

			EditorApplication.update += UpdateFlash;
		}
		
		private void UpdateFlash()
		{
			var elapsed = (float)EditorApplication.timeSinceStartup - _flashStartTime;
			var halfFlashDuration = _flashDuration * 0.5f;

			if (elapsed < halfFlashDuration)
			{
				SetEdgeColor(_targetColor, _targetColor);
			}
			else if (elapsed < _flashDuration)
			{
				var t = (elapsed - halfFlashDuration) / halfFlashDuration;
				var currentInputColor = Color.Lerp(_targetColor, _originalInputColor, t);
				var currentOutputColor = Color.Lerp(_targetColor, _originalOutputColor, t);
				SetEdgeColor(currentInputColor, currentOutputColor);
			}
			else
			{
				SetEdgeColor(_originalInputColor, _originalOutputColor);
				_isFlashing = false;
				EditorApplication.update -= UpdateFlash;
			}
		}
		
		private void UpdateLabelPosition(GeometryChangedEvent evt)
		{
			var midPoint = (edgeControl.controlPoints[0] + edgeControl.controlPoints[3]) * 0.5f;
			_durationLabel.style.left = midPoint.x - (_durationLabel.contentRect.width * 0.5f);
			_durationLabel.style.top = midPoint.y - (_durationLabel.contentRect.height * 0.5f);
		}
	}
}