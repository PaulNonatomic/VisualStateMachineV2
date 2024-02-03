using System;

namespace Nonatomic.VSM2.StateGraph.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeIconAttribute : Attribute
	{
		public string Color { get; private set; }
		public string Path { get; private set; }
		public float Opacity { get; private set; }
		public ResourceSource Source { get; private set; }

		public NodeIconAttribute(string path, float opacity = 1f, ResourceSource source = ResourceSource.Resources, string color = "#ffffff")
		{
			Source = source;
			Path = path;
			Opacity = opacity;
			Color = color;
		}
	}
}