using System.IO;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Utils
{
	public static class ScriptableObjectUtils
	{
		public static T CreateInstanceInProject<T>(bool selectInstance = true) where T : ScriptableObject
		{
			var asset = ScriptableObject.CreateInstance<T>();

			var path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (string.IsNullOrEmpty(path))
			{
				path = "Assets";
			}
			else if (Path.GetExtension(path) != "")
			{
				path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}

			var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).Name + ".asset");

			AssetDatabase.CreateAsset(asset, assetPathAndName);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			
			if(selectInstance) Selection.activeObject = asset;

			return asset;
		}
		
		/// <summary>
		/// Clones the specified ScriptableObject.
		/// </summary>
		/// <typeparam name="T">Type of the ScriptableObject.</typeparam>
		/// <param name="original">The original ScriptableObject to clone.</param>
		/// <returns>A deep copy of the original ScriptableObject.</returns>
		public static T Clone<T>(T original) where T : ScriptableObject
		{
			if (!original) return null;
			
			return Object.Instantiate(original);
		}
	}
}