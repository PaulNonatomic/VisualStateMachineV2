using System.IO;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes.Base
{
	public class NodeStyleManager
	{
		private readonly StateNodeModel _nodeModel;

		private readonly BaseStateNodeView _nodeView;

		public NodeStyleManager(BaseStateNodeView nodeView, StateNodeModel nodeModel)
		{
			_nodeView = nodeView;
			_nodeModel = nodeModel;
		}

		public VisualElement Title { get; private set; }
		public VisualElement TitleContainer { get; private set; }

		public void ApplyBaseStyles()
		{
			AddStyleSheet();
			ApplyNodeWidth();
			ColorizeTitle();
		}

		public void SetupTitleContainer()
		{
			Title = _nodeView.Q<VisualElement>("title");

			var titleButton = Title.Q<VisualElement>("title-button-container");
			Title.Remove(titleButton);

			TitleContainer = new VisualElement();
			TitleContainer.name = "title-container";
			Title.Add(TitleContainer);
		}

		public void AddStyleSheet(string stylePath = null)
		{
			var path = string.IsNullOrEmpty(stylePath)
				? Path.Combine("Nodes", _nodeView.GetType().Name)
				: Path.Combine("Nodes", stylePath);

			var styleSheet = Resources.Load<StyleSheet>(path);
			if (styleSheet != null) _nodeView.styleSheets.Add(styleSheet);
		}

		public void ColorizeTitle()
		{
			var nodeTitle = _nodeView.Q<VisualElement>("title");
			if (nodeTitle == null) return;

			var stateType = _nodeModel.State.GetType();
			if (!AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt)) return;
			if (!ColorUtility.TryParseHtmlString(colorAtt.HexColor, out var color)) return;

			nodeTitle.style.backgroundColor = color;
		}

		public void ApplyNodeWidth()
		{
			var stateType = _nodeModel.State.GetType();
			if (!AttributeUtils.TryGetInheritedCustomAttribute<NodeWidthAttribute>(stateType, out var widthAtt)) return;

			var width = widthAtt.Width;
			_nodeView.style.maxWidth = width;
			_nodeView.style.width = width;
		}

		public Image CreateNodeIcon()
		{
			var stateType = _nodeModel.State.GetType();

			var icon = new Image
			{
				name = "title-icon",
				scaleMode = ScaleMode.ScaleToFit,
				style = { display = DisplayStyle.None }
			};

			var nodeIcon = AttributeUtils.GetInheritedCustomAttribute<NodeIconAttribute>(stateType);
			if (nodeIcon == null) return icon;

			var iconPath = NodeIcon.GetNodeIconPath(nodeIcon.Path);
			var iconTexture = ImageService.FetchTexture(iconPath, nodeIcon.Source);
			icon.image = iconTexture;
			icon.style.opacity = nodeIcon.Opacity;
			icon.style.display = DisplayStyle.Flex;

			if (ColorUtility.TryParseHtmlString(nodeIcon.Color, out var color)) icon.tintColor = color;

			return icon;
		}

		public void AddTitleLabel(string label = null)
		{
			var titleString = label ?? _nodeModel.State.GetType().Name;
			_nodeView.title = StringUtils.ProcessNodeTitle(titleString);

			var titleLabel = Title.Q<VisualElement>("title-label");
			TitleContainer.Add(titleLabel);
		}

		public void RemoveTitleLabel()
		{
			var titleLabel = Title.Q<VisualElement>("title-label");
			titleLabel?.parent.Remove(titleLabel);
		}
	}
}