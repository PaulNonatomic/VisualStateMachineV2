using Nonatomic.VSM2.StateGraph;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class FooterBarView : VisualElement
	{
		private Label _gridPositionLabel;
		private VisualElement _gridPositionContainer;
		private StateLabelView _stateLabelView;

		public FooterBarView()
		{
			this.name = "footerBar";

			ApplyStyle();
			AddGridPositionContainer();
			AddGridPositionLabel();
			AddStateLabel();
			ApplySpacing();
		}

		private void ApplySpacing()
		{
			var children = _gridPositionContainer.Children();
			foreach (var child in children) {
				child.style.marginLeft = 4; // Adjust as needed
			}
		}

		private void AddGridPositionContainer()
		{
			_gridPositionContainer = new VisualElement();
			_gridPositionContainer.name = "grid-position-container";
			Add(_gridPositionContainer);
		}

		private void AddStateLabel()
		{
			_stateLabelView = new StateLabelView();
			_gridPositionContainer.Add(_stateLabelView);
		}

		private void ApplyStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>("FooterBarView");
			styleSheets.Add(style);
		}

		public void SetModel(StateMachineModel model)
		{
			_stateLabelView.SetModel(model);
		}

		public void SetGridPosition(Vector2 position)
		{
			_gridPositionLabel.text = $"x: {position.x}, y: {position.y}";
		}
		
		private void AddGridPositionLabel()
		{ 
			_gridPositionLabel = new Label();
			_gridPositionLabel.name = "grid-position-label";
			_gridPositionContainer.Add(_gridPositionLabel);
		}
	}
}