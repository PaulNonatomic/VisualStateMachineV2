using System;

namespace Nonatomic.VSM2.StateGraph.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeWidthAttribute : Attribute
	{
		public float Width { get; private set; }

		public NodeWidthAttribute(float width)
		{
			Width = width;
		}
	}
}