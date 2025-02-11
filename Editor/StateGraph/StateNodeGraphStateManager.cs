using System.Linq;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateNodeGraphStateManager : NodeGraphStateManager
	{
		public string StateControllerId { get; private set; } = string.Empty;
		
		private const string StateMachineControllerIdKey = "StateMachineControllerId";
		
		public StateNodeGraphStateManager(string id) : base(id)
		{
			// ...
		}

		public void LoadModelFromStateController()
		{
			// Look for the controller that matches the stored ID.
			var stateMachines = GameObject.FindObjectsByType<StateMachineController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			var controller = stateMachines.FirstOrDefault(smc => smc.Id == StateControllerId);
			
			if (controller)
			{
				SetModel(controller.Model);
			}
			else
			{
				Debug.LogWarning($"StateMachineController with ID {StateControllerId} not found.");
			}
		}
		
		public void SetStateControllerId(string id)
		{
			if (string.IsNullOrEmpty(id)) return;
			
			StateControllerId = id;
			SaveState();
		}
		
		protected override void ResetState()
		{
			base.ResetState();
			EditorPrefs.DeleteKey(GetKey(StateMachineControllerIdKey));
		}
		
		public override void LoadState()
		{
			base.LoadState();
			StateControllerId = EditorPrefs.GetString(GetKey(StateMachineControllerIdKey));
		}

		public override void SaveState()
		{
			base.SaveState();
			if (!Model) return;
		
			EditorPrefs.SetString(GetKey(StateMachineControllerIdKey), StateControllerId);
		}
	}
}