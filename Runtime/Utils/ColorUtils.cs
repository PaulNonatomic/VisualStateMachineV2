using UnityEngine;

namespace Nonatomic.VSM2.Utils
{
	public static class ColorUtils
	{
		public static Color HexToColor(string hex)
		{
			if (hex.StartsWith("#")) hex = hex.Substring(1);
			if (hex.Length == 6) hex += "FF";
			
			var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			var a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

			return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		}
		
		public static Color GetColorFromValue(int value, int maxValue)
		{
			var hue = (float)value / maxValue * 360; // Scale value to [0, 360]
			return Color.HSVToRGB(hue / 360f, 1f, 1f); // Convert HSV to RGB
		}
		
		public static Color GetColorFromValue(int value, int maxValue, float brightness)
		{
			var hue = (float)value / maxValue * 360; // Scale value to [0, 360]
			brightness = Mathf.Clamp01(brightness); // Ensure brightness is between 0 and 1
			return Color.HSVToRGB(hue / 360f, 1f, brightness); // Convert HSV to RGB
		}
	}
}