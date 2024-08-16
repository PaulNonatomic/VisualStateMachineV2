using System;
using Nonatomic.VSM2.NodeGraph;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.NodeGraph
{
	public class NodeGraphStateManager
	{
		public event Action OnChange;
		public NodeGraphDataModel Model { get; private set; }
		public Vector2 GridPosition { get; private set; } = Vector2.zero;
		public Vector2 GridScale { get; private set; } = Vector2.one;
		public string Id { get; private set; }
		
		private const string ModelPathKey = "ModelPath";
		private const string GridPosXKey = "GridPosX";
		private const string GridPosYKey = "GridPosY";
		private const string GridScaleXKey = "GridScaleX";
		private const string GridScaleYKey = "GridScaleY";

		protected string GetKey(string key) => $"{Id}_{key}";

		public NodeGraphStateManager(string id)
		{
			Id = id;
			LoadState();
		}

		protected virtual void ResetState()
		{
			EditorPrefs.DeleteKey(GetKey(GridPosXKey));
			EditorPrefs.DeleteKey(GetKey(GridPosYKey));
			EditorPrefs.DeleteKey(GetKey(GridScaleXKey));
			EditorPrefs.DeleteKey(GetKey(GridScaleYKey));
		}

		public virtual void LoadState()
		{
			var changed = false;
			
			if (TryLoadModelAtPath(out var model))
			{
				Model = model;
				changed = true;
			}

			if (TryLoadGridPosition(out var gridPosition))
			{
				GridPosition = gridPosition;
				changed = true;
			}
			
			if(TryLoadGridScale(out var gridScale))
			{
				GridScale = gridScale;
				changed = true;
			}

			if (changed)
			{
				OnChange?.Invoke();
			}
		}

		public virtual void SaveState()
		{
			if (!Model) return;

			var modelPath = AssetDatabase.GetAssetPath(Model);
			EditorPrefs.SetString(GetKey(ModelPathKey), modelPath);
			EditorPrefs.SetFloat(GetKey(GridPosXKey), GridPosition.x);
			EditorPrefs.SetFloat(GetKey(GridPosYKey), GridPosition.y);
			EditorPrefs.SetFloat(GetKey(GridScaleXKey), GridScale.x);
			EditorPrefs.SetFloat(GetKey(GridScaleYKey), GridScale.y);
		}

		public void SetModel(NodeGraphDataModel model)
		{
			if (!model) return;
			
			Model = model;
			SaveState();
		}
		
		public void SetGridPosition(Vector2 gridCenter, Vector2 gridPosition)
		{
			var diff = gridPosition - gridCenter;
			GridPosition = diff;
			SaveState();
		}
		
		public void SetGridScale(Vector2 scale)
		{
			GridScale = scale;
			SaveState();
		}

		private bool TryLoadGridPosition(out Vector2 gridPosition)
		{
			gridPosition = Vector2.zero;
			gridPosition.x = EditorPrefs.GetFloat(GetKey(GridPosXKey));
			gridPosition.y = EditorPrefs.GetFloat(GetKey(GridPosYKey));
			var dist = Vector2.Distance(gridPosition, GridPosition);

			return dist > 0;
		}
		
		private bool TryLoadGridScale(out Vector2 gridScale)
		{
			gridScale = Vector2.one;
			gridScale.x = EditorPrefs.GetFloat(GetKey(GridScaleXKey));
			gridScale.y = EditorPrefs.GetFloat(GetKey(GridScaleYKey));
			
			if (gridScale.x == 0 && gridScale.y == 0)
			{
				gridScale = Vector2.one;
			}
			
			var dist = Vector2.Distance(gridScale, GridScale);

			return dist > 0;
		}

		private bool TryLoadModelAtPath(out NodeGraphDataModel model)
		{
			var modelPath = EditorPrefs.GetString(GetKey(ModelPathKey));
			model = AssetDatabase.LoadAssetAtPath<NodeGraphDataModel>(modelPath);
			
			return model;
		}
	}
}