using System;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateGraphEditorWindow : NodeGraphEditorWindow
	{
		public StateGraphEditorWindow() : base("State Machine Editor")
		{
			
		}
		
		[MenuItem("Window/State Machine Editor")]
		public static void OpenWindow()
		{
			NodeGraphEditorWindow.OpenWindow<StateGraphEditorWindow>();
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

			Title = "State Machine Editor";
			return OpenWindow<StateGraphEditorWindow>(model);
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