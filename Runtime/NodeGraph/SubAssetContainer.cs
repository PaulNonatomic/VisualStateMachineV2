using System.Collections.Generic;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nonatomic.VSM2.NodeGraph
{
	/// <summary>
	/// Represents a container for managing sub-assets in the Unity Editor. 
	/// This class provides methods for adding, removing, and validating sub-assets.
	/// </summary>
	public abstract class SubAssetContainer : ScriptableObject
	{
		[SerializeField] private List<Object> _subAssets = new ();

		/// <summary>
		/// Initializes the SubAssetContainer and subscribes to the assembly reload event.
		/// </summary>
		public virtual void OnEnable()
		{
			#if UNITY_EDITOR
			{
				AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
			}
			#endif
		}

		/// <summary>
		/// Unsubscribes from the assembly reload event when the SubAssetContainer is disabled.
		/// </summary>
		public virtual void OnDisable()
		{
			#if UNITY_EDITOR
			{
				AssemblyReloadEvents.afterAssemblyReload -= HandleAfterAssemblyReload;
			}
			#endif
		}

		/// <summary>
		/// Handles the event triggered after assembly reload and validates the sub-assets.
		/// </summary>
		protected virtual void HandleAfterAssemblyReload()
		{
			ValidateSubAssets();
		}

		/// <summary>
		/// Attempts to add a sub-asset to this container. Logs a warning if the addition fails.
		/// </summary>
		/// <param name="subAsset">The sub-asset to add.</param>
		protected virtual void AddSubAsset(Object subAsset)
		{
			if (subAsset == null) return;

			EditorApplication.delayCall += () =>
			{
				if (this == null) return;

				if (SubAssetUtils.TryAddSubAsset(this, subAsset))
				{
					_subAssets.Add(subAsset);
				}
				else
				{
					GraphLog.LogWarning($"Could not add sub asset {subAsset}");
				}
			};
		}

		/// <summary>
		/// Attempts to remove a specified sub-asset from this container. Logs a warning if the removal fails.
		/// </summary>
		/// <param name="subAsset">The sub-asset to remove.</param>
		protected virtual void RemoveSubAsset(Object subAsset)
		{
			if (subAsset == null) return;

			EditorApplication.delayCall += () =>
			{
				if (this == null) return;

				if (SubAssetUtils.TryRemoveSubAsset(subAsset))
				{
					_subAssets.Remove(subAsset);
				}
				else
				{
					GraphLog.LogWarning($"Could not remove sub asset {subAsset}");
				}
			};
		}
		
		/// <summary>
		/// Removes a sub-asset at the specified index within the container. Logs a warning if the removal fails.
		/// </summary>
		/// <param name="index">The index of the sub-asset to remove.</param>
		protected virtual void RemoveSubAssetAtIndex(int index)
		{
			if (index < 0 || index >= _subAssets.Count) return;

			var asset = _subAssets[index];
			if (SubAssetUtils.TryRemoveSubAsset(asset))
			{
				_subAssets.RemoveAt(index);
			}
			else
			{
				GraphLog.LogWarning($"Could not remove sub asset at index {index}");
			}
		}
		
		/// <summary>
		/// Validates the sub-assets within the container.
		/// It removes any sub-assets that are either null or of the base Object type
		/// which only happens if it's type has been deleted.
		/// </summary>
		protected virtual void ValidateSubAssets()
		{
			var invalidAssetIndices = new List<int>();

			for (var i = 0; i < _subAssets.Count; i++)
			{
				var subAsset = _subAssets[i];
				if (subAsset == null || subAsset.GetType() == typeof(Object))
				{
					invalidAssetIndices.Add(i);
				}
			}

			for (var i = invalidAssetIndices.Count - 1; i >= 0; i--)
			{
				var index = invalidAssetIndices[i];
				if (SubAssetUtils.TryRemoveSubAsset(_subAssets[index]))
				{
					GraphLog.LogWarning($"Removed invalid sub-asset at index {i}");
					_subAssets.RemoveAt(index);
				}
				else
				{
					GraphLog.LogWarning($"Could not remove invalid sub-asset at index {i}");
				}
			}
		}
	}
}