using System;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.Commands
{
	[Serializable]
	public abstract class ScriptableCommand : ScriptableObject, IScriptableCommand
	{
		public abstract void Execute();
	}
}