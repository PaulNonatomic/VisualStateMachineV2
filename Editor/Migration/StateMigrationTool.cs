#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.Utils;
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

		[MenuItem("Tools/VSM2/Migrate to 0.9.0-beta")]
		public static void Migrate()
		{
			MigrateStates();
			MigrateModels();
		}

		private static void MigrateModels()
		{
			var models = AssetUtils.FindAllScriptableObjectsOfType<StateMachineModel>();
			foreach (var model in models)
			{
				StateMachineMigrator.Migrate(model);
			}
		}

		private static void MigrateStates()
		{
			var derivedTypes = AssetUtils.GetAllDerivedTypes<State>();

			foreach (var stateClass in derivedTypes)
			{
				if (IsIgnoredNamespace(stateClass.Namespace)) continue;
				
				var path = MigrationUtils.GetPathForType(stateClass);
				if (string.IsNullOrEmpty(path)) continue;
				
				MigrateState(stateClass, path);
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
