using Nonatomic.VSM2.StateGraph;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class FooterBarView : VisualElement
	{
		private Label _gridPositionLabel;
		private VisualElement _footerBar;
		private GridPositionView _gridPosition;
		private StateLabelView _stateLabelView;

		public FooterBarView()
		{
			name = "footerBar";

			ApplyStyle();
			AddGridPosition();
			AddStateLabel();
			ApplySpacing();
		}

		private void AddGridPosition()
		{
			_gridPosition = new GridPositionView();
			Add(_gridPosition);
		}

		private void ApplySpacing()
		{
			var children = Children();
			
			foreach (var child in children) 
			{
				child.style.marginLeft = 4;
			}
		}

		private void AddStateLabel()
		{
			_stateLabelView = new StateLabelView();
			Add(_stateLabelView);
		}

		private void ApplyStyle()
		{
			var styleSheet = UnityEngine.Resources.Load<StyleSheet>(nameof(FooterBarView));
			styleSheets.Add(styleSheet);
		}

		public void SetModel(StateMachineModel model)
		{
			_stateLabelView.SetModel(model);
		}

		public void SetGridPosition(Vector2 position)
		{
			_gridPosition.SetGridPosition(position);
		}
	}
}