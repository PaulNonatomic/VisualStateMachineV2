using System;

namespace Nonatomic.VSM2.NodeGraph
{
	[Serializable]
	public class PortModel
	{
		public string Id;
		public int Index;
		public int FrameDelay = 1;
		public string PortLabel;
		public string PortColor;
		public Type TransitionType;

		public PortModel Clone()
		{
			return new PortModel()
			{
				Id = Id,
				Index = Index,
				FrameDelay = FrameDelay,
				PortLabel = PortLabel,
				PortColor = PortColor,
				TransitionType = TransitionType
			};
		}
	}
}