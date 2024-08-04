using System.Collections.Generic;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class StickyNoteNodeView : BaseStateNodeView
	{
		public StickyNoteNodeView(GraphView graphView, StateMachineModel stateMachineModel, StateNodeModel nodeModel) : base(graphView, stateMachineModel, nodeModel)
		{
			var contents = this.Query<VisualElement>("contents").First();
			
			AddStyle("StickyNoteNodeView");
			AddTitleContainer();
			ColorizeTitle();
			AddTitleLabel("Note");
			AddTitleIcon();
			AddProgressBar();
			
			var propertyContainer = CreatePropertyContainer();
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);
			AddProperties(propertyContainer);
			
			RemovePortContainer();
			CheckCustomWidth();
			UpdatePosition();
		}
		
		protected override VisualElement MakePropertyInspector(UnityEngine.Object target, 
			List<string> propertiesToExclude = null)
		{
			var container = new VisualElement();
			var serializedObject = new SerializedObject(target);
			
			var multilineTextField = new TextField();
			multilineTextField.multiline = true;
			multilineTextField.bindingPath = "_note";
			multilineTextField.style.whiteSpace = WhiteSpace.Normal;
			multilineTextField.style.minHeight = 60;
			multilineTextField.style.overflow = Overflow.Hidden; // Hide overflow initially
			multilineTextField.style.flexGrow = 1;
			multilineTextField.Bind(serializedObject);

			container.Add(multilineTextField);
			return container;
		}
	}
}