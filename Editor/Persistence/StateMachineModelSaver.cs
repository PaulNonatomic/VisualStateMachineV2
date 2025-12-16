using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;

namespace Nonatomic.VSM2.Editor.Persistence
{
	/// <summary>
	/// Static class responsible for saving all state machine models in the project when the project changes i.e.
	/// When files or assets are added, deleted or renamed. Also triggered by reimporting.
	/// </summary>
	[InitializeOnLoad]
	public static class StateMachineModelSaver
	{
		/// <summary>
		/// Saves all state machine models in the project by updating their data and marking them as dirty.
		/// </summary>
		public static void SaveAll()
		{
			try
			{
				var guids = AssetDatabase.FindAssets("t:StateMachineModel");
				var totalModels = guids.Length;
				
				for (var i = 0; i < totalModels; i++)
				{
					var guid = guids[i];
					var path = AssetDatabase.GUIDToAssetPath(guid);
					var model = AssetDatabase.LoadAssetAtPath<StateMachineModel>(path);
					if (!model) continue;

					UpdateAndMarkModelDirty(model);
					EditorUtility.DisplayProgressBar("Saving State Machine Models", $"Saving {i + 1} of {totalModels} models...", (float)(i + 1) / totalModels);
				}

				AssetDatabase.SaveAssets();
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}
		
		/// <summary>
		/// Saves the specified state machine model by updating its data and marking it as dirty.
		/// </summary>
		/// <param name="model">The state machine model to save.</param>
		public static void Save(StateMachineModel model)
		{
			if (!model) return;
			
			try
			{
				model?.SelfValidate();
				
				EditorUtility.DisplayProgressBar("Saving State Machine Model", "Saving model...", 0.5f);
				UpdateAndMarkModelDirty(model);
				AssetDatabase.SaveAssets();
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}

		/// <summary>
		/// Updates the data of the specified state machine model and marks it as dirty.
		/// </summary>
		/// <param name="model">The state machine model to update and mark as dirty.</param>
		private static void UpdateAndMarkModelDirty(StateMachineModel model)
		{ 
			if (!model) return;
			
			model = StateMachineModelUtils.UpdatePortDataInModel(model);
			EditorUtility.SetDirty(model);
		}
	}
}