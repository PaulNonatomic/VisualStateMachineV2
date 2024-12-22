#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Migration
{
	public static class StateMigrationTool
	{
		// If you have multiple namespaces to ignore, list them here.
		// We'll check if a class's namespace starts with any of these strings.
		private static readonly string[] _ignoredNamespaces = new[]
		{
			"Nonatomic.VSM2.StateGraph.States"
		};
	
		// These dictionaries map old method names to new method names.
		private static readonly (string oldMethod, string newMethod)[] _methodMap =
		{
			("OnAwakeState",      "OnAwake"),
			("OnStartState",      "OnStart"),
			("OnEnterState",      "OnEnter"),
			("OnExitState",       "OnExit"),
			("OnUpdateState",     "OnUpdate"),
			("OnFixedUpdateState","OnFixedUpdate"),
			("OnDestroyState",    "OnDestroy"),
		};

		[MenuItem("Tools/VSM2/Migrate States")]
		public static void MigrateStates()
		{
			var stateClasses = MigrationUtils.FindAllDerivedStates();

			foreach (var stateClass in stateClasses)
			{
				// 1. Check if the class belongs to an ignored namespace:
				if (IsIgnoredNamespace(stateClass.Namespace))
				{
					//
					continue;
				}

				// 2. Get the file path (.cs) for this type.
				var path = MigrationUtils.GetPathForType(stateClass);
				if (string.IsNullOrEmpty(path))
				{
					Debug.LogWarning($"Skipping {stateClass.FullName} - no .cs file found.");
					continue;
				}

				// 3. Prompt user with an OK / Skip / Cancel dialog for each class
				var choice = EditorUtility.DisplayDialogComplex(
					title:      "Migrate State?",
					message:    $"Class: {stateClass.FullName}\nFile: {path}\n\n" +
								"Do you want to migrate this class?",
					ok:         "OK",
					cancel:     "Skip",
					alt:  "Cancel"
				);

				switch (choice)
				{
					case 0: // OK
						MigrateState(stateClass, path);
						break;

					case 1: // Skip
						Debug.Log($"Skipping {stateClass.FullName} at user request.");
						continue;

					case 2: // Cancel
						Debug.LogWarning("Migration canceled by user.");
						return; // Stop processing altogether
				}
			}
		
			AssetDatabase.Refresh();
			Debug.Log("State migration completed successfully.");
		}

		private static bool IsIgnoredNamespace(string ns)
		{
			if (string.IsNullOrEmpty(ns))
			{
				return false;
			}

			// Return true if the namespace starts with any of the ignore patterns
			return _ignoredNamespaces.Any(ignore => ns.StartsWith(ignore));
		}

		private static void MigrateState(Type stateClass, string path)
		{
			var fileContents = File.ReadAllText(path);
			var newContents = fileContents;
		
			MigrateOldMethodNames(ref newContents);
			AddEnterAttribute(ref newContents);
			AddUsingStatements(ref newContents);

			// Save changes if they differ
			if (newContents == fileContents) return;
			
			File.WriteAllText(path, newContents);
			Debug.Log($"Migrated {stateClass.FullName}: {Path.GetFileName(path)}");
		}

		private static void MigrateOldMethodNames(ref string fileContents)
		{
			foreach (var map in _methodMap)
			{
				if (fileContents.Contains(map.oldMethod))
				{
					fileContents = fileContents.Replace(map.oldMethod, map.newMethod);
				}
			}
		}

		private static void AddUsingStatements(ref string fileContents)
		{
			fileContents = UsingStatementInserter.InsertUsingStatementIfMissing(fileContents, "Nonatomic.VSM2.StateGraph");
		}

		private static void AddEnterAttribute(ref string fileContents)
		{
			// Example usage: Insert [Enter] above the OnEnter method if you need to
			if (fileContents.Contains("OnEnter"))
			{
				fileContents = MethodAttributeInserter.InsertAttributeAboveMethod(
					sourceCode: fileContents,
					methodName: "OnEnter",
					attributeToInsert: "[Enter]"
				);
			}
		}
	}
}
#endif
