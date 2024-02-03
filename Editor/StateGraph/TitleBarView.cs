using System;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class TitleBarView : VisualElement
	{
		public event Action OnRecenter;
		
		private Label _label;
		private Label _gridPositionLabel;
		private Button _recenterButton;

		public TitleBarView()
		{
			this.name = "titleBar";

			AddStateMachineModelLabel();
			AddGridPositionLabel();
			AddRecenterButton();
		}

		public void SetTitle(string modelName)
		{
			_label.text = modelName;
		}

		public void SetGridPosition(Vector2 position)
		{
			_gridPositionLabel.text = $"x: {position.x}, y: {position.y}";
		}

		private void AddRecenterButton()
		{
			_recenterButton = new Button(() => OnRecenter?.Invoke());
			_recenterButton.name = "toolbtn";
			
			var icon = new Image();
			icon.name = "toolbtn-icon";
			icon.scaleMode = ScaleMode.ScaleToFit;
			
			var iconTexture = ImageService.FetchTexture(NodeIcon.VsmRecenter);
			icon.image = iconTexture;
			icon.style.display = DisplayStyle.Flex;
			
			_recenterButton.Add(icon);
			
			Add(_recenterButton);
		}

		private void AddGridPositionLabel()
		{
			_gridPositionLabel = new Label();
			_gridPositionLabel.name = "gridPositionLabel";
			Add(_gridPositionLabel);
		}

		private void AddStateMachineModelLabel()
		{
			_label = new Label();
			_label.name = "titleLabel";
			Add(_label);
		}
	}
}