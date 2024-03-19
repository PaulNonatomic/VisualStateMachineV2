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
			//EditorUtility.FocusProjectWindow();
			if(selectInstance) Selection.activeObject = asset;

			return asset;
		}
	}
}