using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class DelayNodeView : BaseStateNodeView
	{
		private readonly VisualElement _propertyContainer;
		private Image _icon;

		public DelayNodeView(GraphView graphView, 
							 StateMachineModel stateMachineModel,  
							 StateNodeModel nodeModel) 
							 : base(graphView, stateMachineModel, nodeModel)
		{

			AddStyle("DelayNodeView");
			AddTitleContainer();
			ColorizeTitle();
			RemoveTitleLabel();
			AddGlowBorder();
			AddInputPorts(TitleContainer);
			
			_propertyContainer = CreatePropertyContainer();
			TitleContainer.Add(_propertyContainer);
			
			AddTitleIcon();
			AddDurationField();
			AddOutputPorts(TitleContainer);
			UpdatePosition();
		}

		public override void Update()
		{
			UpdateGlowBorder();
			UpdateIconSpin();
		}

		private void AddDurationField()
		{
			var delayState = (BaseDelayState) NodeModel.State;
			var floatField = new FloatField("")
			{
				bindingPath = nameof(delayState.Duration)
			};
			this.Bind(new SerializedObject(delayState));

			_propertyContainer.Add(floatField);
		}

		private void UpdateIconSpin()
		{
			var rot = _icon.style.rotate;
			var ang = rot.value;
			ang.angle = NodeModel.Active ? (NodeModel.LastActive % 1f) * 350f : 0f;
			rot.value = ang;

			_icon.style.rotate = rot;
		}
		
		protected override void AddTitleIcon()
		{
			_icon = MakeIcon(NodeModel);
			_propertyContainer.Insert(0, _icon);
		}
	}
}