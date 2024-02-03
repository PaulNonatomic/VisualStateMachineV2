using System;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph
{
	[Serializable]
	public class StatePortModel : PortModel
	{
		public int FrameDelay = 1;
		public string PortLabel = string.Empty;
		public string PortColor = default;
	}
}