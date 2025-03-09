using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class StickyNoteNodeView : BaseStateNodeView
	{
		public StickyNoteNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			var contents = this.Q<VisualElement>("contents");

			StyleManager.AddStyleSheet(nameof(StickyNoteNodeView));
			StyleManager.AddTitleLabel("Note");

			var icon = StyleManager.CreateNodeIcon();
			StyleManager.TitleContainer.Insert(0, icon);

			AnimationController.AddProgressBar();

			var propertyContainer = CreateCustomPropertyContainer();
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);

			PortManager.RemovePortContainer();
			StyleManager.ApplyNodeWidth();
			UpdatePosition();
		}

		private VisualElement CreateCustomPropertyContainer()
		{
			var container = new VisualElement();
			var serializedObject = new SerializedObject(NodeModel.State);

			var multilineTextField = new TextField();
			multilineTextField.multiline = true;
			multilineTextField.bindingPath = "_note";
			multilineTextField.style.whiteSpace = WhiteSpace.Normal;
			multilineTextField.style.minHeight = 60;
			multilineTextField.style.overflow = Overflow.Hidden;
			multilineTextField.style.flexGrow = 1;
			multilineTextField.Bind(serializedObject);

			container.Add(multilineTextField);
			return container;
		}
	}
}