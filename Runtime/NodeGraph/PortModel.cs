using System;
using Nonatomic.VSM2.Extensions;
using UnityEngine.Serialization;

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
				PortTypeName = PortTypeName
			};
		}

		public void SetPortType(Type type)
		{
			PortType = type;
			PortTypeName = type.GetSimplifiedName() ?? string.Empty;
		}
	}
}