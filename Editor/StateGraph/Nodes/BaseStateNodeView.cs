using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class BaseStateNodeView : NodeView
	{
		protected void ColorizeTitle(StateNodeModel nodeModel)
		{
			var title = this.Query<VisualElement>("title").First();
			if (title == null) return;
			
			var stateType = nodeModel.State.GetType();
			if (AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt))
			{
				if(ColorUtility.TryParseHtmlString(colorAtt.HexColor, out var color))
				{
					color.a = 0.8f;
					title.style.backgroundColor = color;
				}
			}
		}

		protected void ApplyStateColorToPortData(StateNodeModel nodeModel, PortModel portModel)
		{
			var stateType = nodeModel.State.GetType();
			
			if (AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt))
			{
				portModel.PortColor = colorAtt.HexColor;
			}
		}
		
		protected bool TryUpdatePortDataFromState(StateNodeModel nodeModel, string portId, out PortModel portModel)
		{
			portModel = null;
			
			var stateType = nodeModel.State.GetType();
			
			var eventInfo = stateType.GetEvent(portId);
			if (eventInfo == null) return false;
			
			var attributes = eventInfo.GetCustomAttributes(typeof(TransitionAttribute), false);
			if (attributes.Length == 0) return false;
				
			var transAtt = (TransitionAttribute) attributes[0];
			portModel = transAtt.GetPortData(eventInfo, 0);
			return true;
		}
		
		protected void UpdatePosition(StateNodeModel nodeModel, StateMachineModel model)
		{
			var position = nodeModel.Position;
			
			var rect = this.GetPosition();
			rect.position = position;
			
			SetPosition(rect);
			
			EditorUtility.SetDirty(model);
			EditorApplication.delayCall += () => AssetDatabase.SaveAssets();
		}
		
		protected Image MakeIcon(StateNodeModel nodeModel)
		{
			var stateType = nodeModel.State.GetType();
			
			var icon = new Image();
			icon.name = "title-icon";
			icon.scaleMode = ScaleMode.ScaleToFit;
			icon.style.display = DisplayStyle.None;
			
			var nodeIcon = AttributeUtils.GetInheritedCustomAttribute<NodeIconAttribute>(stateType);
			if (nodeIcon == null) return icon;
			
			var iconTexture = ImageService.FetchTexture(nodeIcon.Path, nodeIcon.Source);
			icon.image = iconTexture;
			icon.style.opacity = nodeIcon.Opacity;
			icon.style.display = DisplayStyle.Flex;
			
			if (ColorUtility.TryParseHtmlString(nodeIcon.Color, out var color))
			{
				icon.tintColor = color;
			}

			return icon;
		}
	}
}