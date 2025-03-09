// Base class designed to be open for extension

using System;
using System.Diagnostics;
using System.IO;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes.Base
{
	public class BaseStateNodeView : NodeView
	{
		public StateNodeModel NodeModel { get; private set; }
	
		protected NodeStyleManager StyleManager { get; private set; }
		protected NodePortManager PortManager { get; private set; }
		protected NodePropertyPanel PropertyPanel { get; private set; }
		protected NodeAnimationController AnimationController { get; private set; }
	
		protected GraphView GraphView;
		protected StateMachineModel StateMachineModel;
	
		public BaseStateNodeView(GraphView graphView, StateMachineModel stateMachineModel, StateNodeModel nodeModel)
		{
			GraphView = graphView;
			StateMachineModel = stateMachineModel;
			NodeModel = nodeModel;
		
			name = nodeModel.Id;
			userData = nodeModel;
		
			StyleManager = new NodeStyleManager(this, nodeModel);
			PortManager = new NodePortManager(this, graphView, stateMachineModel, nodeModel);
			PropertyPanel = new NodePropertyPanel(this, nodeModel);
			AnimationController = new NodeAnimationController(this, nodeModel);
		
			InitializeNode();
		
			RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
			RegisterCallback<DetachFromPanelEvent>(HandleLeavePanel);
			RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
			RegisterCallback<FocusEvent>(HandleFocus);
		}
	
		public virtual void Update()
		{
			AnimationController.UpdateAnimations();
		}
	
		protected virtual void InitializeNode()
		{
			StyleManager.ApplyBaseStyles();
			StyleManager.SetupTitleContainer();
		}
	
		protected virtual void HandleFocus(FocusEvent evt) { }
	
		protected virtual void HandleGeometryChanged(GeometryChangedEvent evt)
		{
			if (evt.oldRect.position != evt.newRect.position)
			{
				NodeModel.Position = evt.newRect.position;
			}
		}
	
		protected virtual void HandleLeavePanel(DetachFromPanelEvent evt) { }
	
		protected virtual void HandleAttachToPanel(AttachToPanelEvent evt) { }
	
		protected void UpdatePosition()
		{
			var position = NodeModel.Position;
		
			var rect = GetPosition();
			rect.position = position;
		
			SetPosition(rect);
		
			if(StateMachineModel == null) return;
			EditorUtility.SetDirty(StateMachineModel);
			EditorApplication.delayCall += () => AssetDatabase.SaveAssets();
		}
	
		protected Button CreateEditButton(Action clickHandler)
		{
			if(NodeModel?.State == null) return null;
		
			var stateNamespace = NodeModel.State.GetType().Namespace;
			if (stateNamespace == "Nonatomic.VSM2.StateGraph.States") return null;
		
			var editButton = new Button(clickHandler)
			{
				name = "edit-btn"
			};

			var icon = new Image
			{
				name = "edit-icon",
				scaleMode = ScaleMode.ScaleToFit
			};

			var iconPath = NodeIcon.GetNodeIconPath(NodeIcon.Pencil);
			var iconTexture = ImageService.FetchTexture(iconPath);
			icon.image = iconTexture;
			icon.style.display = DisplayStyle.Flex;
		
			editButton.Add(icon);
		
			return editButton;
		}
	
		// Utility to handle edit button click - accessible to subclasses
		protected void OpenStateScript()
		{
			if(NodeModel?.State == null) return;
		
			var scriptFilePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(NodeModel.State));
			var absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptFilePath));
		
			Process.Start(absolutePath);
		}
	}
}