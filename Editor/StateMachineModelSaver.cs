namespace VisualStateMachine.Editor.Utils
{
	using UnityEditor;
	using UnityEngine;
	using Nonatomic.VSM2.StateGraph;

	[InitializeOnLoad]
	public static class StateMachineModelSaver
	{
		static StateMachineModelSaver()
		{
			EditorApplication.projectChanged += SaveAllScriptableObjects;
		}

		private static void SaveAllScriptableObjects()
		{
			var guids = AssetDatabase.FindAssets("t:StateMachineModel");
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var stateMachineModel = AssetDatabase.LoadAssetAtPath<StateMachineModel>(path);
				if (!stateMachineModel) continue;
				
				EditorUtility.SetDirty(stateMachineModel);
			}

			AssetDatabase.SaveAssets();
		}
	}
}