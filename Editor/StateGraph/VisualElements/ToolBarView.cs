using System;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class ToolBarView : VisualElement
	{
		public event Action OnRecenter;
		public event Action OnSave;
		
		private Button _recenterButton;
		private Button _saveButton;
		private VisualElement _buttonContainer;
		private BreadcrumbTrailView _breadcrumbTrail;

		public ToolBarView()
		{
			this.name = "toolBar";

			ApplyStyle();
			AddBreadcrumbTrail();
			AddButtonContainer();
			AddSaveButton();
			AddRecenterButton();
		}

		private void AddBreadcrumbTrail()
		{
			_breadcrumbTrail = new BreadcrumbTrailView();
			Add(_breadcrumbTrail);
		}

		public void SetModel(StateMachineModel model)
		{
			_breadcrumbTrail.SetModel(model);
		}

		private void ApplyStyle()
		{
			var styleSheet = UnityEngine.Resources.Load<StyleSheet>(nameof(ToolBarView));
			styleSheets.Add(styleSheet);
		}

		private void AddButtonContainer()
		{
			_buttonContainer = new VisualElement
			{
				name = "button-container"
			};
			Add(_buttonContainer);
		}

		private void AddSaveButton()
		{
			_saveButton = new Button(() => OnSave?.Invoke())
			{
				name = "toolbtn"
			};

			var icon = new Image
			{
				name = "toolbtn-icon",
				scaleMode = ScaleMode.ScaleToFit,
				style =
				{
					display = DisplayStyle.Flex
				}
			};

			var iconPath = NodeIcon.GetNodeIconPath(NodeIcon.FloppyDisk);
			var iconTexture = ImageService.FetchTexture(iconPath);
			icon.image = iconTexture;

			_saveButton.Add(icon);
			_buttonContainer.Add(_saveButton);
		}

		private void AddRecenterButton()
		{
			_recenterButton = new Button(() => OnRecenter?.Invoke())
			{
				name = "toolbtn"
			};

			var icon = new Image
			{
				name = "toolbtn-icon",
				scaleMode = ScaleMode.ScaleToFit,
				style =
				{
					display = DisplayStyle.Flex
				}
			};

			var iconPath = NodeIcon.GetNodeIconPath(NodeIcon.CenterSquare);
			var iconTexture = ImageService.FetchTexture(iconPath);
			icon.image = iconTexture;

			_recenterButton.Add(icon);
			_buttonContainer.Add(_recenterButton);
		}
	}
}