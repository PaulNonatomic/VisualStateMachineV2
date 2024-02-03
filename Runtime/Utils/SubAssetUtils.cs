using Nonatomic.VSM2.Logging;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Utils
{
	/// <summary>
	/// Utility class for managing sub-assets within the Unity Editor.
	/// </summary>
	public static class SubAssetUtils
	{
		/// <summary>
		/// Checks if a sub-asset can be added to a main asset.
		/// </summary>
		/// <param name="mainAsset">The main asset to which the sub-asset might be added.</param>
		/// <param name="subAsset">The sub-asset to be added.</param>
		/// <returns>True if the sub-asset can be added to the main asset, false otherwise.</returns>
		public static bool CanAddObjectToAsset(Object mainAsset, Object subAsset)
		{
			var result = false;
			
			#if UNITY_EDITOR
			{
				if (!Application.isEditor) return false;
				if (subAsset == null || mainAsset == null) return false;
				
				result = AssetDatabase.Contains(mainAsset);
			}
			#endif

			return result;
		}

		/// <summary>
		/// Attempts to add a sub-asset to a main asset.
		/// </summary>
		/// <param name="mainAsset">The main asset to which the sub-asset will be added.</param>
		/// <param name="subAsset">The sub-asset to add.</param>
		/// <returns>True if the sub-asset was successfully added, false otherwise.</returns>
		public static bool TryAddSubAsset(Object mainAsset, Object subAsset)
		{
			var result = false;
			
			#if UNITY_EDITOR
			{
				if (CanAddObjectToAsset(mainAsset, subAsset))
				{
					AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
					EditorUtility.SetDirty(mainAsset);
					EditorApplication.delayCall += ()=> AssetDatabase.SaveAssets();
					
					result = true;
				}
				else
				{
					GraphLog.LogWarning("Cannot add object to asset");
				}
			}
			#endif

			return result;
		}

		/// <summary>
		/// Checks if a sub-asset can be removed.
		/// </summary>
		/// <param name="subAsset">The sub-asset to be removed.</param>
		/// <returns>True if the sub-asset can be removed, false otherwise.</returns>
		public static bool CanRemoveSubAsset(Object subAsset)
		{
			var result = false;
			
			#if UNITY_EDITOR
			{
				if (!Application.isEditor) return false;
				if (subAsset == null) return false;
				
				result = AssetDatabase.Contains(subAsset);
			}
			#endif

			return result;
		}

		/// <summary>
		/// Attempts to remove a sub-asset.
		/// </summary>
		/// <param name="subAsset">The sub-asset to be removed.</param>
		/// <returns>True if the sub-asset was successfully removed, false otherwise.</returns>
		public static bool TryRemoveSubAsset(Object subAsset)
		{
			var result = false;
			
			#if UNITY_EDITOR
			{
				if (CanRemoveSubAsset(subAsset))
				{
					AssetDatabase.RemoveObjectFromAsset(subAsset);
					EditorApplication.delayCall += ()=> AssetDatabase.SaveAssets();
					
					result = true;
				}
				else
				{
					GraphLog.LogWarning("Cannot remove sub-asset: it may not exist or is not a valid sub-asset.");
				}
			}
			#endif

			return result;
		}
	}
}