using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nonatomic.VSM2.NodeGraph
{
	[Serializable]
	public class NodeModel
	{
		public string Id;
		public Vector2 Position;
		public List<PortModel> OutputPorts = new List<PortModel>();
		public List<PortModel> InputPorts = new List<PortModel>();
	}
}