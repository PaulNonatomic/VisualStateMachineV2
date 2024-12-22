using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.Utils;

namespace Nonatomic.VSM2.Editor.Migration
{
	public static class StateMachineMigrator
	{
		public static void Migrate(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			MigrateNodePorts(stateMachineModel);
			MigrateTransitions(stateMachineModel);
		}

		private static void MigrateTransitions(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			for (var index = stateMachineModel.Transitions.Count - 1; index >= 0; index--)
			{
				var transition = stateMachineModel.Transitions[index];
				
				// Handle migration (Older state machines had an OnEnterState method)
				if (transition.DestinationNodeId == "OnEnterState")
				{
					transition.DestinationNodeId = "OnEnter";
				}
			}
		}
		
		private static void MigrateNodePorts(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			foreach (var node in stateMachineModel.Nodes)
			{
				foreach (var port in node.InputPorts)
				{
					if(port.Id == "OnEnterState")
					{
						port.Id = "OnEnter";
					}
				}

				foreach (var port in node.OutputPorts)
				{
					if(port.Id == "OnEnterState")
					{
						port.Id = "OnEnter";
					}
				}
			}
		}
	}
}
