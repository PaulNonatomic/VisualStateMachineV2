using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Utils
{
	public class SubAssetValidator
	{
		/**
		 * This solution was sourced:
		 * https://gitlab.com/RotaryHeart-UnityShare/subassetmissingscriptdelete/-/blob/master/FixMissingScript
		 *
		 * This approach first requires that you are aware of a corrupted sub-asset so is perhaps bests suited for
		 * manual fixing of corrupted sub-assets.
		 *
		 * For an automated solution subscribe to AssemblyReloadEvents.afterAssemblyReload and check for corrupted
		 * sub-assets by maintaining an internal list of sub-assets and checking for either null references or checking
		 * the asset type to see if it's reverted to UnityEngine.Object.
		 */
		public static void RemoveBrokenSubAsset(Object parentAsset)
		{
			#if UNITY_EDITOR
			{
				var newInstance = ScriptableObject.CreateInstance(parentAsset.GetType());

				//Copy the original content to the new instance
				EditorUtility.CopySerialized(parentAsset, newInstance);
				newInstance.name = parentAsset.name;

				var toDeletePath = AssetDatabase.GetAssetPath(parentAsset);
				var clonePath = toDeletePath.Replace(".asset", "CLONE.asset");

				//Create the new asset on the project files
				AssetDatabase.CreateAsset(newInstance, clonePath);
				AssetDatabase.ImportAsset(clonePath);

				//Unhide sub-assets
				var subAssets = AssetDatabase.LoadAllAssetsAtPath(toDeletePath);
				var flags = new HideFlags[subAssets.Length];
				for (var i = 0; i < subAssets.Length; i++)
				{
					//Ignore the "corrupt" one
					if (subAssets[i] == null) continue;

					//Store the previous hide flag
					flags[i] = subAssets[i].hideFlags;
					subAssets[i].hideFlags = HideFlags.None;
					EditorUtility.SetDirty(subAssets[i]);
				}

				EditorUtility.SetDirty(parentAsset);
				AssetDatabase.SaveAssets();

				//Reparent the subAssets to the new instance
				foreach (var subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(toDeletePath))
				{
					//Ignore the "corrupt" one
					if (subAsset == null) continue;

					//We need to remove the parent before setting a new one
					AssetDatabase.RemoveObjectFromAsset(subAsset);
					AssetDatabase.AddObjectToAsset(subAsset, newInstance);
				}

				//Import both assets back to unity
				AssetDatabase.ImportAsset(toDeletePath);
				AssetDatabase.ImportAsset(clonePath);

				//Reset sub-asset flags
				for (var i = 0; i < subAssets.Length; i++)
				{
					//Ignore the "corrupt" one
					if (subAssets[i] == null) continue;

					subAssets[i].hideFlags = flags[i];
					EditorUtility.SetDirty(subAssets[i]);
				}

				EditorUtility.SetDirty(newInstance);
				AssetDatabase.SaveAssets();

				//Here's the magic. First, we need the system path of the assets
				var globalToDeletePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), toDeletePath);
				var globalClonePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), clonePath);

				//We need to delete the original file (the one with the missing script asset)
				//Rename the clone to the original file and finally
				//Delete the meta file from the clone since it no longer exists

				System.IO.File.Delete(globalToDeletePath);
				System.IO.File.Delete(globalClonePath + ".meta");
				System.IO.File.Move(globalClonePath, globalToDeletePath);

				AssetDatabase.Refresh();
			}
			#endif
		}
	}
}