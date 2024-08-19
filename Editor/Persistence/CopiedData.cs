using System;
using System.Collections.Generic;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Persistence
{
	[Serializable]
	public class CopiedData
	{
		public List<StateNodeModel> SelectedNodes;
		public List<StateTransitionModel> SelectedTransitions;

		public CopiedData(List<StateNodeModel> selectedNodes, List<StateTransitionModel> selectedTransitions)
		{
			SelectedNodes = selectedNodes;
			SelectedTransitions = selectedTransitions;
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}
	}
}