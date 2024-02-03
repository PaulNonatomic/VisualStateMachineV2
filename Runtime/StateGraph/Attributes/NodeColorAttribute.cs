using System;

namespace Nonatomic.VSM2.StateGraph.Attributes
{
	public static class NodeColor
	{
		public const string Grey = "#444444";
		public const string Red = "#990e23";
		public const string Orange = "#915710";
		public const string LimeGreen = "#6d9111";
		public const string Green = "#116f1c";
		public const string ForestGreen = "#08704a";
		public const string Teal = "#066670";
		public const string LightBlue = "#037091";
		public const string Blue = "#084870";
		public const string Purple = "#4a0e99";
		public const string Violet = "#740e99";
		public const string Pink = "#750b55";
		
		public const string Black = "#000000";
		public const string White = "#ffffff";
	}
	
	
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeColorAttribute : Attribute
	{
		public string HexColor { get; private set; }
		
		public NodeColorAttribute(string hexColor)
		{
			HexColor = hexColor;
		}
	}
}