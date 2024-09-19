using System;
using Nonatomic.VSM2.Extensions;

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
		public string PortTypeName;
		public string PortTypeLabel;
		public Type PortType;

		public PortModel Clone()
		{
			return new PortModel()
			{
				Id = Id,
				Index = Index,
				FrameDelay = FrameDelay,
				PortLabel = PortLabel,
				PortColor = PortColor,
				PortType = PortType,
				PortTypeLabel = PortTypeLabel,
				PortTypeName = PortTypeName
			};
		}
		
		public void SetPortType(Type type)
		{
			PortType = type;
			PortTypeLabel = type?.GetSimplifiedName() ?? string.Empty;
			PortTypeName = type?.FullName ?? string.Empty;
		}
	}
}