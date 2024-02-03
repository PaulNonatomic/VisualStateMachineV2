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
			
		}
		
		public void LoadModelFromStateController()
		{
			if (string.IsNullOrEmpty(StateControllerId)) return;
			
			var stateMachines = GameObject.FindObjectsByType<StateMachineController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			var stateMachineController = stateMachines.FirstOrDefault(smc => smc.Id == StateControllerId);
			if (stateMachineController == null) return;

			SetModel(stateMachineController.Model);
		}
		
		public void SetStateControllerId(string id)
		{
			StateControllerId = id;
			SaveState();
		}
		
		public override void ResetState()
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
			
			if (Model == null) return;

			EditorPrefs.SetString(GetKey(StateMachineControllerIdKey), StateControllerId);
		}
	}
}