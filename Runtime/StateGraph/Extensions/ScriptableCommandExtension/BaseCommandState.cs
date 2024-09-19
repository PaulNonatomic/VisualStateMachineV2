#if SCRIPTABLE_COMMANDS

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Nonatomic.ScriptableCommands;

namespace Nonatomic.VSM2.StateGraph.States
{
	public abstract class BaseCommandState : State
	{
		protected CancellationTokenSource CancellationTokenSource { get; private set; }

		[SerializeField] private ScriptableCommand[] _commands;

		public override void OnEnterState()
		{
			CancellationTokenSource = new CancellationTokenSource();
			_ = ExecuteCommands();
		}

		public override void OnExitState()
		{
			if (CancellationTokenSource == null) return;
			
			CancellationTokenSource.Cancel();
			CancellationTokenSource.Dispose();
			CancellationTokenSource = null;
		}

		protected virtual async Task ExecuteCommands()
		{
			try
			{
				await ExecuteCommandsAsync();
			}
			catch (Exception ex)
			{
				HandleCommandException(ex);
			}

			HandleTaskCompletion();
		}

		protected virtual async Task ExecuteCommandsAsync()
		{
			try
			{
				foreach (var command in _commands)
				{
					await command.ExecuteAsync(CancellationTokenSource.Token);
				}
			}
			catch (Exception ex)
			{
				HandleCommandException(ex);
			}
		}

		protected virtual void HandleTaskCompletion()
		{
			//...
		}

		protected virtual void HandleCommandException(Exception ex)
		{
			//...
		}
	}
}
#endif