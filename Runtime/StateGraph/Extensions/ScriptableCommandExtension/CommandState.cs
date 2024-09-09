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
			_cts = new CancellationTokenSource();
			_ = ExecuteCommands();
		}

		public override void OnExitState()
		{
			if (_cts == null) return;
			
			_cts.Cancel();
			_cts.Dispose();
			_cts = null;
		}

		private async Task ExecuteCommands()
		{
			try
			{
				await ExecuteCommandsAsync();
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error executing commands: {ex}");
			}
			
			OnComplete?.Invoke();
		}

		private async Task ExecuteCommandsAsync()
		{
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
		}
	}
}
#endif