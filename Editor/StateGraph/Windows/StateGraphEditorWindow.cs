using System;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateGraphEditorWindow : NodeGraphEditorWindow
	{
		protected static string WindowTitle = "State Machine Editor";
		
		public StateGraphEditorWindow() : base("State Machine Editor")
		{
			
		}
		
		[MenuItem("Window/State Machine Editor")]
		public static void OpenWindow()
		{
			var window = NodeGraphEditorWindow.OpenWindow<StateGraphEditorWindow>();
			window.titleContent.text = WindowTitle;
		}
		
		public override void OnEnable()
		{
			base.OnEnable();
			OpenWindow();
		}

		private void OnFocus()
		{
			Initialize();
		}

		[InitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			OpenWindowDelegate = OpenSpecificWindow;
		}
		
		private static NodeGraphEditorWindow OpenSpecificWindow(NodeGraphDataModel model)
		{
			if (model is not StateMachineModel) return null;

			var window = OpenWindow<StateGraphEditorWindow>(model);
			window.titleContent.text = WindowTitle;

			return window;
		}
		
		protected override NodeGraphView AddGraphView()
		{
			try
			{
				var graphview = new StateGraphView("StateGraphView");
				rootVisualElement.Add(graphview);
				return graphview;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}