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

		public CopiedData(List<StateNodeModel> selectedNodes)
		{
			SelectedNodes = selectedNodes;
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}
	}
}