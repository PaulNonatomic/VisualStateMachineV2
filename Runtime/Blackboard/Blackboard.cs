using System.Collections.Generic;
using Nonatomic.VSM2.NodeGraph;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Blackboard
{
	[CreateAssetMenu(fileName = "Blackboard", menuName = Constants.AssetMenuRoot + "/Blackboard")]
	public class Blackboard : SubAssetContainer
	{
		private Dictionary<VariableKey, object> _variables = new Dictionary<VariableKey, object>();
		
		public void SetValue<T>(VariableKey key, T value)
		{
			_variables[key] = value;
		}

		public T GetValue<T>(VariableKey key)
		{
			if (_variables.TryGetValue(key, out var value))
			{
				return (T)value;
			}
			
			return default;
		}
		
		public bool TryAddValue<T>(VariableKey key, T value)
		{
			if (key == null || _variables.ContainsKey(key)) return false;
			
			_variables.Add(key, value);
			AddSubAsset(key);
			MarkDirty();
			return true;
		}
		
		public bool TryRemoveValue(VariableKey key)
		{
			if (key == null || !_variables.ContainsKey(key)) return false;
			
			_variables.Remove(key);
			RemoveSubAsset(key);
			MarkDirty();
			return true;
		}
		
		private void MarkDirty()
		{
			#if UNITY_EDITOR
			{
				EditorUtility.SetDirty(this);
			}
			#endif
		}
		
		protected override void ValidateSubAssets()
		{
			base.ValidateSubAssets();
			
			var keysToRemove = new List<VariableKey>();

			foreach (var kvp in _variables)
			{
				if (!kvp.Key) 
				{
					keysToRemove.Add(kvp.Key);
				}
			}

			foreach (var key in keysToRemove)
			{
				_variables.Remove(key);
			}
		}
	}
}