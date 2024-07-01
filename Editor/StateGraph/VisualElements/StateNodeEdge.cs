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

		public StateNodeEdge() : base()
		{
			RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(HandleAttach));
			RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(HandleDetach));
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
			FlashEdge(color, 1.0f);
		}

		protected override EdgeControl CreateEdgeControl() => new EdgeControl()
		{
			capRadius = 4f,
			interceptWidth = 6f
		};
		
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

			if (elapsed < _flashDuration)
			{
				var t = Mathf.PingPong(elapsed, halfFlashDuration) / halfFlashDuration;
				var currentInputColor = Color.Lerp(_originalInputColor, _targetColor, t);
				var currentOutputColor = Color.Lerp(_originalOutputColor, _targetColor, t);
				SetEdgeColor(currentInputColor, currentOutputColor);
			}
			else
			{
				SetEdgeColor(_originalInputColor, _originalOutputColor);
				_isFlashing = false;
				EditorApplication.update -= UpdateFlash;
			}
		}
	}
}