using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateNodeEdge : Edge
	{
		private Color _originalInputColor;
		private Color _originalOutputColor;
		private Color _targetColor;
		private bool _isFlashing;
		private float _flashDuration;
		private float _flashStartTime;
		private Label _durationLabel;
		private int _frameDelay;
		
		private const float MinFlashDuration = 0.75f;
		private const float FlashFadeInDuration = MinFlashDuration * 0.5f;
		
		public StateNodeEdge() : base()
		{
			ApplyStyle();
			CreateDurationLabel();
			
			RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(HandleAttach));
			RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(HandleDetach));
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
			
			UpdateDurationLabel();
		}

		private void Subscribe()
		{
			if (userData is not StateTransitionModel transitionModel) return;
			transitionModel.OnTransitionBegin += HandleTransitionBegin;
			transitionModel.OnTransitionUpdate += HandleTransitionUpdate;
			transitionModel.OnTransitionEnd += HandleTransitionEnd;
		}

		private void Unsubscribe()
		{
			if (userData is not StateTransitionModel transitionModel) return;
			transitionModel.OnTransitionBegin -= HandleTransitionBegin;
			transitionModel.OnTransitionUpdate -= HandleTransitionUpdate;
			transitionModel.OnTransitionEnd -= HandleTransitionEnd;
		}

		private void HandleDetach(DetachFromPanelEvent evt)
		{
			Unsubscribe();
		}

		private void HandleAttach(AttachToPanelEvent evt)
		{
			Subscribe();
		}

		private void HandleTransitionEnd()
		{
		}

		private void HandleTransitionUpdate()
		{
		}

		private void HandleTransitionBegin()
		{
			HandleFlash();
		}

		private void HandleFlash()
		{
			if (!ColorUtility.TryParseHtmlString("#4CAF50", out var color)) return;
			if (userData is not StateTransitionModel transitionModel) return;
			
			var flashDuration = Mathf.Max(MinFlashDuration, transitionModel.OriginPort.FrameDelay * (1f / Application.targetFrameRate));
			FlashEdge(color, flashDuration);
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
			var fadeOutStart = _flashDuration - FlashFadeInDuration;

			if (elapsed < FlashFadeInDuration)
			{
				// Fade in
				var t = elapsed / FlashFadeInDuration;
				var currentInputColor = Color.Lerp(_originalInputColor, _targetColor, t);
				var currentOutputColor = Color.Lerp(_originalOutputColor, _targetColor, t);
				SetEdgeColor(currentInputColor, currentOutputColor);
			}
			else if (elapsed < fadeOutStart)
			{
				// Hold target color
				SetEdgeColor(_targetColor, _targetColor);
			}
			else if (elapsed < _flashDuration)
			{
				// Fade out
				var t = (elapsed - fadeOutStart) / FlashFadeInDuration;
				var currentInputColor = Color.Lerp(_targetColor, _originalInputColor, t);
				var currentOutputColor = Color.Lerp(_targetColor, _originalOutputColor, t);
				SetEdgeColor(currentInputColor, currentOutputColor);
			}
			else
			{
				// End flashing
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

			UpdateDurationLabel();
		}

		private void UpdateDurationLabel()
		{
			EditorApplication.delayCall += () =>
			{
				if (userData is not StateTransitionModel transitionModel) return;
				_durationLabel.text = $"{transitionModel.OriginPort.FrameDelay}";
			};
		}
	}
}