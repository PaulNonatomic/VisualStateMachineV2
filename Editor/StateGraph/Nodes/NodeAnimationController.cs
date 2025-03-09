using Nonatomic.VSM2.StateGraph;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class NodeAnimationController
	{
		private readonly StateNodeModel _nodeModel;
		private readonly BaseStateNodeView _nodeView;
		private VisualElement _glowBorder;
		private ProgressBar _progressBar;

		public NodeAnimationController(BaseStateNodeView nodeView, StateNodeModel nodeModel)
		{
			_nodeView = nodeView;
			_nodeModel = nodeModel;
		}

		public void AddGlowBorder()
		{
			_glowBorder = new VisualElement
			{
				name = "state-border",
				pickingMode = PickingMode.Ignore
			};
			_nodeView.Add(_glowBorder);
		}

		public void AddProgressBar()
		{
			_progressBar = new ProgressBar
			{
				name = "progress-bar"
			};

			var nodeTitle = _nodeView.Q<VisualElement>("title");
			nodeTitle.Add(_progressBar);
		}

		public void UpdateAnimations()
		{
			UpdateGlowBorder();
			UpdateProgressBar();
		}

		private void UpdateGlowBorder()
		{
			if (_glowBorder == null) return;

			var timeElapsed = Time.time - _nodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = Mathf.Approximately(_nodeModel.LastActive, -1f) ? 0 : timeOpacity;
			_glowBorder.style.opacity = opacity;
		}

		private void UpdateProgressBar()
		{
			if (_progressBar == null) return;

			var progressFill = _progressBar.Q(className: "unity-progress-bar__progress");
			progressFill.visible = _nodeModel.Active;
			_progressBar.value = Time.time % 1f * 100f;
		}
	}
}