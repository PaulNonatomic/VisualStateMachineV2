using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Services;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class BaseStateNodeView : NodeView
	{
		public StateNodeModel NodeModel { get; private set; }
		
		protected VisualElement GlowBorder;
		protected GraphView GraphView;
		protected StateMachineModel StateMachineModel;
		protected Type StateType;
		protected VisualElement Title;
		protected VisualElement TitleContainer;
		
		public BaseStateNodeView(GraphView graphView, StateMachineModel stateMachineModel, StateNodeModel nodeModel)
		{
			GraphView = graphView;
			StateMachineModel = stateMachineModel;
			NodeModel = nodeModel;
			StateType = nodeModel.State.GetType();
			
			name = nodeModel.Id;
			userData = nodeModel;
			
			RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
			RegisterCallback<DetachFromPanelEvent>(HandleLeavePanel);
			RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
			RegisterCallback<FocusEvent>(HandleFocus);
		}

		protected virtual void HandleFocus(FocusEvent evt)
		{
			// ...
		}

		public virtual void Update()
		{
			// ...
		}

		protected virtual void HandleGeometryChanged(GeometryChangedEvent evt)
		{
			if (evt.oldRect.position != evt.newRect.position)
			{
				NodeModel.Position = evt.newRect.position;
			}
		}
		
		protected virtual void HandleLeavePanel(DetachFromPanelEvent evt)
		{
			// ...
		}

		protected virtual void HandleAttachToPanel(AttachToPanelEvent evt)
		{
			// ...
		}

		protected virtual void ColorizeTitle()
		{
			var nodeTitle = this.Query<VisualElement>("title").First();
			if (nodeTitle == null) return;
			
			var stateType = NodeModel.State.GetType();
			if (!AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt)) return;
			if (!ColorUtility.TryParseHtmlString(colorAtt.HexColor, out var color)) return;
			
			nodeTitle.style.backgroundColor = color;
		}

		protected virtual void ApplyNodeWidth(StateNodeModel nodeModel)
		{
			var stateType = nodeModel.State.GetType();
			if (!AttributeUtils.TryGetInheritedCustomAttribute<NodeWidthAttribute>(stateType, out var widthAtt)) return;
			
			var width = widthAtt.Width;
			style.maxWidth = width;
			style.width = width;
		}

		protected virtual void ApplyStateColorToPortData(StateNodeModel nodeModel, PortModel portModel)
		{
			var stateType = nodeModel.State.GetType();
			
			if (AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt))
			{
				portModel.PortColor = colorAtt.HexColor;
			}
		}
		
		protected virtual bool TryUpdatePortDataFromState(StateNodeModel nodeModel, string portId, out PortModel portModel)
		{
			portModel = null;
			
			var stateType = nodeModel.State.GetType();
			
			var eventInfo = stateType.GetEvent(portId);
			if (eventInfo == null) return false;
			
			var attributes = eventInfo.GetCustomAttributes(typeof(TransitionAttribute), false);
			if (attributes.Length == 0) return false;
				
			var transAtt = (TransitionAttribute) attributes[0];
			portModel = transAtt.GetPortData(eventInfo, 0);

			if (portModel.PortColor == default && 
				AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt))
			{
				portModel.PortColor = colorAtt.HexColor;
			}
			
			return true;
		}
		
		protected virtual void UpdatePosition()
		{
			var position = NodeModel.Position;
			
			var rect = this.GetPosition();
			rect.position = position;
			
			SetPosition(rect);
			
			EditorUtility.SetDirty(StateMachineModel);
			EditorApplication.delayCall += () => AssetDatabase.SaveAssets();
		}
		
		protected virtual Image MakeIcon(StateNodeModel nodeModel)
		{
			var stateType = nodeModel.State.GetType();
			
			var icon = new Image
			{
				name = "title-icon",
				scaleMode = ScaleMode.ScaleToFit,
				style =
				{
					display = DisplayStyle.None
				}
			};

			var nodeIcon = AttributeUtils.GetInheritedCustomAttribute<NodeIconAttribute>(stateType);
			if (nodeIcon == null) return icon;
			
			var iconPath = NodeIcon.GetNodeIconPath(nodeIcon.Path);
			var iconTexture = ImageService.FetchTexture(iconPath, nodeIcon.Source);
			icon.image = iconTexture;
			icon.style.opacity = nodeIcon.Opacity;
			icon.style.display = DisplayStyle.Flex;
			
			if (ColorUtility.TryParseHtmlString(nodeIcon.Color, out var color))
			{
				icon.tintColor = color;
			}

			return icon;
		}
		
		protected virtual VisualElement MakePropertyInspector(UnityEngine.Object target, 
			List<string> propertiesToExclude = null)
		{
			var container = new VisualElement();
			var serializedObject = new SerializedObject(target);
			var fields = FieldUtils.GetInheritedSerializedFields(target.GetType());

			foreach (var field in fields)
			{
				if (propertiesToExclude != null && propertiesToExclude.Contains(field.Name)) continue;

				var serializedProperty = serializedObject.FindProperty(field.Name);
				if (serializedProperty == null)
				{
					GraphLog.LogWarning($"Property {field.Name} not found in serialized object.");
					continue;
				}

				if (IsSubStateMachineList(serializedProperty))
				{
					// Handle SubStateMachine list
					var customElement = CreateCustomSubStateMachineUI(serializedProperty);
					container.Add(customElement);
				}
				else
				{
					// Handle regular properties
					var propertyField = new PropertyField(serializedProperty);
					container.Add(propertyField);
				}
			}

			container.Bind(serializedObject);
			return container;
		}
		
		protected static bool IsSubStateMachineList(SerializedProperty property)
		{
			if (!property.isArray) return false;
			return property.arrayElementType == "PPtr<$StateMachineModel>";
		}

		protected static VisualElement CreateCustomSubStateMachineUI(SerializedProperty property)
		{
			return new PropertyField(property);
		}
		
		protected virtual void AddStyle(string stylePath)
		{
			var path = Path.Combine("Nodes", stylePath);
			var styleSheet = Resources.Load<StyleSheet>(path);
			Assert.IsNotNull(styleSheet, $"{path}.uss not found");
			styleSheets.Add(styleSheet);
		}
		
		protected virtual void AddGlowBorder()
		{
			GlowBorder = new VisualElement
			{
				name = "state-border",
				pickingMode = PickingMode.Ignore
			};
			Add(GlowBorder);
		}
		
		protected virtual void UpdateGlowBorder()
		{
			if (GlowBorder == null) return;
			
			var timeElapsed = Time.time - NodeModel.LastActive;
			var timeOpacity = 1.0f - Mathf.Clamp01(timeElapsed / 1f);
			var opacity = Mathf.Approximately(NodeModel.LastActive, -1f) ? 0 : timeOpacity;
			GlowBorder.style.opacity = opacity;
		}
		
		protected virtual void UpdateProgressBar()
		{
			var progressBar = this.Query<ProgressBar>().First();
			if (progressBar == null) return;
			
			var progressFill = progressBar.Q(className:"unity-progress-bar__progress");
			progressFill.visible = NodeModel.Active;
			progressBar.value = (Time.time % 1f) * 100f;
		}
		
		protected virtual void AddInputPorts(VisualElement portContainer)
		{
			foreach (var portData in NodeModel.InputPorts)
			{
				ApplyStateColorToPortData(NodeModel, portData);
				
				StateGraphPortFactory.MakePort(GraphView, 
											   StateMachineModel, 
											   this,
											   portContainer, 
											   Direction.Input, 
											   Port.Capacity.Multi, 
											   portData);
			}
		}
		
		protected virtual void AddOutputPorts(VisualElement portContainer)
		{
			foreach (var t in NodeModel.OutputPorts)
			{
				var portData = t;
				TryUpdatePortDataFromState(NodeModel, portData.Id, out portData);

				StateGraphPortFactory.MakePort(GraphView, 
											   StateMachineModel,
											   this,
											   portContainer, 
											   Direction.Output, 
											   Port.Capacity.Single, 
											   portData);
			}
		}
		
		protected virtual void AddTitleContainer()
		{
			Title = this.Query<VisualElement>("title").First();

			var titleButton = Title.Query<VisualElement>("title-button-container").First();
			Title.Remove(titleButton);
			
			TitleContainer = new VisualElement();
			TitleContainer.name = "title-container";
			Title.Add(TitleContainer);
		}
		
		protected virtual void AddTitleLabel()
		{
			var titleString = StateType.Name;
			title = StringUtils.ProcessNodeTitle(titleString);
			
			var titleLabel = Title.Query<VisualElement>("title-label").First();
			TitleContainer.Add(titleLabel);
		}
		
		protected virtual void AddTitleLabel(string label)
		{
			title = StringUtils.ProcessNodeTitle(label);
			
			var titleLabel = Title.Query<VisualElement>("title-label").First();
			TitleContainer.Add(titleLabel);
		}
		
		protected virtual void RemoveTitleLabel()
		{
			var titleLabel = Title.Query<VisualElement>("title-label").First();
			titleLabel?.parent.Remove(titleLabel);
		}
		
		protected virtual void AddTitleIcon()
		{
			var icon = MakeIcon(NodeModel);
			TitleContainer.Insert(0, icon);
		}

		protected virtual void AddEditButton()
		{
			if(!NodeModel?.State) return;
			
			var stateNamespace = NodeModel.State.GetType().Namespace;
			if (stateNamespace == "Nonatomic.VSM2.StateGraph.States~") return;
			
			var editButton = new Button(HandleEditButton)
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
			TitleContainer.Add(editButton);
		}

		private void HandleEditButton()
		{
			if(!NodeModel?.State) return;
			
			var scriptFilePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(NodeModel.State));
			var absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptFilePath));
			
			Process.Start(absolutePath);
		}

		protected virtual void AddProgressBar()
		{
			var progressBar = new ProgressBar
			{
				name = "progress-bar"
			};

			var nodeTitle = this.Query<VisualElement>("title").First();
			nodeTitle.Add(progressBar);
		}
		
		protected virtual void CheckCustomWidth()
		{
			var stateType = NodeModel.State.GetType();
			var nodeWidth = stateType.GetCustomAttribute<NodeWidthAttribute>();
			if (nodeWidth == null) return;
			
			style.maxWidth = nodeWidth.Width;
			style.minWidth = nodeWidth.Width;
			style.width = nodeWidth.Width;
		}
		
		protected virtual VisualElement CreatePropertyContainer()
		{
			var propertyContainer = new VisualElement();
			propertyContainer.name = "property-container";
			return propertyContainer;
		}
		
		protected virtual void AddProperties(VisualElement container)
		{
			var scrollView = new ScrollView();
			container.Add(scrollView);

			var stateInspector = MakePropertyInspector(NodeModel.State);
			stateInspector.name = "state-inspector";
			scrollView.contentContainer.Add(stateInspector);
				
			if (stateInspector.childCount > 0)
			{
				container.AddToClassList("has-properties");
			}
		}
		
		protected virtual void RemovePortContainer()
		{
			var top = this.Query<VisualElement>("top").First();
			top?.parent.Remove(top);
			
			var divider = this.Query<VisualElement>("divider").First();
			divider?.parent.Remove(divider);
		}
	}
}