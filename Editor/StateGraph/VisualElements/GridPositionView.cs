using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class GridPositionView : VisualElement
	{
		private Label _gridPositionLabel;

		public GridPositionView()
		{
			name = "grid-position-container";
			ApplyStyle();
			AddGridPositionLabel();
		}

		public void SetGridPosition(Vector2 position)
		{
			_gridPositionLabel.text = $"x: {position.x}, y: {position.y}";
		}

		private void ApplyStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>(nameof(GridPositionView));
			styleSheets.Add(style);
		}

		private void AddGridPositionLabel()
		{ 
			_gridPositionLabel = new Label();
			_gridPositionLabel.name = "grid-position-label";
			Add(_gridPositionLabel);
		}
	}
}