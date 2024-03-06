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
		
		private Button _recenterButton;
		private VisualElement _buttonContainer;
		private BreadcrumTrailView _breadcrumbTrail;
		
		public ToolBarView()
		{
			this.name = "toolBar";

			ApplyStyle();
			AddBreadcrumTrail();
			AddButtonContainer();
			AddRecenterButton();
		}

		private void AddBreadcrumTrail()
		{
			_breadcrumbTrail = new BreadcrumTrailView();
			Add(_breadcrumbTrail);
		}

		public void SetModel(StateMachineModel model)
		{
			_breadcrumbTrail.SetModel(model);
		}
		
		private void ApplyStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>(nameof(ToolBarView));
			styleSheets.Add(style);
		}

		private void AddButtonContainer()
		{
			_buttonContainer = new VisualElement();
			_buttonContainer.name = "button-container";
			Add(_buttonContainer);
		}

		private void AddRecenterButton()
		{
			_recenterButton = new Button(() => OnRecenter?.Invoke());
			_recenterButton.name = "toolbtn";
			
			var icon = new Image();
			icon.name = "toolbtn-icon";
			icon.scaleMode = ScaleMode.ScaleToFit;
			
			var iconTexture = ImageService.FetchTexture(NodeIcon.V2_CenterSquare);
			icon.image = iconTexture;
			icon.style.display = DisplayStyle.Flex;
			
			_recenterButton.Add(icon);
			_buttonContainer.Add(_recenterButton);
		}
	}
}