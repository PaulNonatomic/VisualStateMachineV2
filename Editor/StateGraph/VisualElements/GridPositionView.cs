using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class GridPositionView : VisualElement
	{
		private Label _gridPositionLabel;
		private VisualElement _container;

		public GridPositionView()
		{
			name = "grid-position-container";
			ApplyStyle();
			
			_container = new VisualElement()
			{
				name = "grid-position-label-container"
			};
			Add(_container);
			
			AddGridPositionLabel();
		}

		public void SetGridPosition(Vector2 position)
		{
			// Limit precision to one decimal place
			var formattedX = position.x.ToString("F1");
			var formattedY = position.y.ToString("F1");
    
			// Update the label text
			_gridPositionLabel.text = $"x: {formattedX}, y: {formattedY}";
		}

		private void ApplyStyle()
		{
			var styleSheet = UnityEngine.Resources.Load<StyleSheet>(nameof(GridPositionView));
			styleSheets.Add(styleSheet);
		}

		private void AddGridPositionLabel()
		{
			_gridPositionLabel = new Label
			{
				name = "grid-position-label"
			};

			_container.Add(_gridPositionLabel);
		}
	}
}