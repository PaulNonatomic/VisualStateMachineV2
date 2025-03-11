using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class DelayNodeView : BaseStateNodeView
	{
		private FloatField _floatField;
		private Image _icon;
		private VisualElement _propertyContainer;

		public DelayNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
		}

		protected override void InitializeNode()
		{
			Debug.Log("DelayNodeView initialized");
			base.InitializeNode();

			StyleManager.AddStyleSheet(nameof(DelayNodeView));

			AddToClassList("compact-node");

			StyleManager.RemoveTitleLabel();
			AnimationController.AddGlowBorder();
			PortManager.AddInputPorts(StyleManager.TitleContainer);

			_propertyContainer = PropertyPanel.CreatePropertyContainer();
			_propertyContainer.AddToClassList("compact-container"); // Add class for styling
			StyleManager.TitleContainer.Add(_propertyContainer);

			AddTitleIcon();
			AddDurationField();
			PortManager.AddOutputPorts(StyleManager.TitleContainer);

			// Explicitly set width
			style.width = 120;
			style.minWidth = 120;
			style.maxWidth = 120;

			UpdatePosition();
		}

		public override void Update()
		{
			AnimationController.UpdateAnimations();
			UpdateIconSpin();
		}

		private void AddDurationField()
		{
			var delayState = (BaseDelayState)NodeModel.State;
			_floatField = new FloatField("")
			{
				bindingPath = nameof(delayState.Duration)
			};
			_floatField.style.minWidth = 40;
			_floatField.style.maxWidth = 60;
			this.Bind(new SerializedObject(delayState));

			_propertyContainer.Add(_floatField);
		}

		private void UpdateIconSpin()
		{
			// Make sure _icon is initialized
			if (_icon == null) return;

			var rot = _icon.style.rotate;
			var ang = rot.value;
			ang.angle = NodeModel.Active ? NodeModel.LastActive % 1f * 350f : 0f;
			rot.value = ang;

			_icon.style.rotate = rot;
		}

		private void AddTitleIcon()
		{
			_icon = StyleManager.CreateNodeIcon();
			_icon.style.width = 16;
			_icon.style.height = 16;
			_propertyContainer.Insert(0, _icon);
		}
	}
}