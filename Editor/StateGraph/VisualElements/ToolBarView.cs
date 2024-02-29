using System;
using System.Collections.Generic;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.NodeGraph;
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
		private VisualElement _labelContainer;
		private List<LayerLabelView> _layerLabelList = new ();
		
		public ToolBarView()
		{
			this.name = "toolBar";

			ApplyStyle();
			AddLabelContainer();
			AddButtonContainer();
			AddRecenterButton();
		}

		private void AddLabelContainer()
		{
			_labelContainer = new VisualElement();
			_labelContainer.name = "label-container";
			Add(_labelContainer);
		}

		public void SetModel(StateMachineModel model)
		{
			_labelContainer.Clear();
			_layerLabelList.Clear();
			Debug.Log($"/////////// CLEAR");
			
			CreateLayerButton(model);
			CreateParentLayerButton(model);
			AddLayerButtons();
		}

		private void AddLayerButtons()
		{
			for (var index = 0; index < _layerLabelList.Count; index++)
			{
				var label = _layerLabelList[index];
				_labelContainer.Add(label);
				
				if (index > 0)
				{
					label.ToggleTip(true);
				}
			}
		}

		private void CreateParentLayerButton(StateMachineModel model)
		{
			var parent = model.Parent;
			Debug.Log($"Add Parent: {parent?.name}");
			if (parent == null) return;
			if (parent == model) return;
			
			CreateLayerButton(parent);
			CreateParentLayerButton(parent);
		}

		private void ApplyStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>("ToolBarView");
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

		private void CreateLayerButton(NodeGraphDataModel model)
		{
			Debug.Log($"CreateLayerButton: {model.name}");
			var layerLabel = new LayerLabelView();
			layerLabel.SetText(model.name);
			_layerLabelList.Add(layerLabel);
		}
	}
}