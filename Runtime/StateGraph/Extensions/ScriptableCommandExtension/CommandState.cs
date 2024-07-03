#if SCRIPTABLE_COMMANDS

using System;
using System.Threading;
using System.Threading.Tasks;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;
using Nonatomic.ScriptableCommands;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.LightBlue), NodeIcon(NodeIcon.Command)]
	public class CommandState : State
	{
		[Transition] public event Action OnComplete;

		[SerializeField] private ScriptableCommand[] _commands;

		private CancellationTokenSource _cts;

		public override void OnEnterState()
		{
			ExecuteCommands();
		}

		public override void OnExitState()
		{
			//...
		}

		private async void ExecuteCommands()
		{
			await ExecuteCommandsAsync();
			OnComplete?.Invoke();
		}

		private async Task ExecuteCommandsAsync()
		{
			_cts = new CancellationTokenSource();

			try
			{
				foreach (var command in _commands)
				{
					await command.ExecuteAsync(_cts.Token);
				}
			}
			catch (OperationCanceledException)
			{
				Debug.Log("Command execution was cancelled.");
			}
			finally
			{
				_cts?.Dispose();
			}
		}
	}
}
#endif