using System;
using System.Reflection;
using Nonatomic.VSM2.NodeGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.NodeGraph
{
	public abstract class NodeGraphEditorWindow : GraphViewEditorWindow
	{
		public delegate NodeGraphEditorWindow WindowOpener(NodeGraphDataModel model);
		public static WindowOpener OpenWindowDelegate;
		
		protected static string Title = "Node Graph Editor";
		private static Vector2 _windowDefaultSize = new (800, 600);
		
		protected StyleSheet WindowStyle;
		private static NodeGraphEditorWindow _window;

		public bool IsInitialized { get; private set;}

		public NodeGraphEditorWindow(string title)
		{
			Title = title;
		}
		
		public static T OpenWindow<T>() where T : NodeGraphEditorWindow
		{
			var windows = Resources.FindObjectsOfTypeAll<T>();
			if (windows != null && windows.Length > 0)
			{
				_window = windows[0];
				_window.Focus();
				return (T)_window;
			}

			_window = GetWindow<T>(false, Title, true);
			_window.Initialize();
			_window.Show();
			_window.Reposition();
			return (T)_window;
		}

		public static T OpenWindow<T>(NodeGraphDataModel model) where T : NodeGraphEditorWindow
		{
			var windows = Resources.FindObjectsOfTypeAll<T>();
			if (windows != null && windows.Length > 0)
			{
				_window = windows[0];
				_window.Focus();
				return (T)_window;
			}

			_window = GetWindow<T>(false, Title, true);
			_window.Reposition();
			_window.InitializeWithData(model);
			_window.Show();
			
			return (T)_window;
		}
		
		[InitializeOnLoadMethod]
		public static void OnInitializeOnLoad()
		{
			var windows = UnityEngine.Resources.FindObjectsOfTypeAll<NodeGraphEditorWindow>();
			foreach(var window in windows) window.Initialize();
			
			EditorApplication.projectWindowItemOnGUI -= HandleProjectWindowItemOnGUI;
			EditorApplication.projectWindowItemOnGUI += HandleProjectWindowItemOnGUI;
		}

		private static void HandleProjectWindowItemOnGUI(string guid, Rect selectionrect)
		{
			HandleDoubleClick();
		}

		public static void HandleDoubleClick()
		{
			if (Event.current?.type != EventType.MouseDown || Event.current?.clickCount != 2) return;
			if (Selection.activeObject is not NodeGraphDataModel) return;
			
			ModelSelection.ActiveModel = Selection.activeObject as NodeGraphDataModel;
			OpenWindowDelegate?.Invoke(ModelSelection.ActiveModel);
		}
		
		public virtual void Initialize()
		{
			if (IsInitialized) return;
			IsInitialized = true;

			ApplyWindowStyle();
			AddGraphView();
		}

		public virtual void InitializeWithData(NodeGraphDataModel model)
		{
			if (IsInitialized)
			{
				var graphView = rootVisualElement.Q<NodeGraphView>();
				graphView.PopulateGraph(model);
				
				return;
			}
			
			IsInitialized = true;

			ApplyWindowStyle();
			AddGraphViewWithData(model);
		}
		
		public virtual void OnEnable()
		{
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
		}

		public virtual void OnDisable()
		{
			EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
			AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload -= HandleAfterAssemblyReload;
		}

		public void Reposition()
		{
			if (IsInitialized) return;
			
			var rect = this.position;
			rect.width = _windowDefaultSize.x;
			rect.height = _windowDefaultSize.y;
			rect.center = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height).center;
			position = rect;
		}

		private bool NeedsReinitialization()
		{
			var graphView = rootVisualElement.Q<NodeGraphView>();
			return graphView == null;
		}

		protected virtual NodeGraphView AddGraphView()
		{
			var graphview = new NodeGraphView("NodeGraphView");
			rootVisualElement.Add(graphview);
			return graphview;
		}

		private void AddGraphViewWithData(NodeGraphDataModel model = null)
		{
			var graphView = AddGraphView();
			graphView.PopulateGraph(model);
		}

		protected virtual void ApplyWindowStyle()
		{
			WindowStyle ??= UnityEngine.Resources.Load<StyleSheet>("NodeGraphEditor");
			Assert.IsNotNull(WindowStyle, "NodeGraphEditor.uss not found");
			
			rootVisualElement.styleSheets.Add(WindowStyle);
		}

		private void HandleAfterAssemblyReload()
		{
			if(NeedsReinitialization()) ForceReinitialization();
		}

		private void HandleBeforeAssemblyReload()
		{
			if(NeedsReinitialization()) ForceReinitialization();
		}

		private void HandlePlayModeStateChanged(PlayModeStateChange obj)
		{
			if(NeedsReinitialization()) ForceReinitialization();
		}
		
		private void ForceReinitialization()
		{
			IsInitialized = false;
			Initialize();
		}
	}
}
