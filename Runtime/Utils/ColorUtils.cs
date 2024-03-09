using System.Globalization;
using UnityEngine;

namespace Nonatomic.VSM2.Utils
{
	public static class ColorUtils
	{
		public static Color HexToColor(string hex)
		{
			if (hex.StartsWith("#")) hex = hex[1..];
			if (hex.Length == 6) hex += "FF";

			var r = byte.Parse(hex[0..2], NumberStyles.HexNumber);
			var g = byte.Parse(hex[2..4], NumberStyles.HexNumber);
			var b = byte.Parse(hex[4..6], NumberStyles.HexNumber);
			var a = byte.Parse(hex[6..8], NumberStyles.HexNumber);

			return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		}
		
		public static Color GetColorFromValue(int value, int maxValue)
		{
			var hue = (float)value / maxValue * 360;
			return Color.HSVToRGB(hue / 360f, 1f, 1f);
		}
		
		public static Color GetColorFromValue(int value, int maxValue, float brightness)
		{
			var hue = (float)value / maxValue * 360;
			brightness = Mathf.Clamp01(brightness);
			return Color.HSVToRGB(hue / 360f, 1f, brightness);
		}
		
		/// <summary>
		/// Generates a color based on a value, maximum value, and brightness, with an optional hue shift.
		/// The color is generated in HSV color space and then converted to RGB.
		/// </summary>
		/// <param name="value">The value used to determine the hue. It's scaled to the range [0, 360] based on the maxValue.</param>
		/// <param name="maxValue">The maximum value that 'value' can take. It defines the scale for the hue.</param>
		/// <param name="brightness">The brightness of the color. It is clamped to the range [0, 1].</param>
		/// <param name="hueShift">Optional. The amount by which to shift the hue, in degrees. Can be negative or positive. Default is 0. Range [-360, 360]</param>
		/// <returns>Returns a <see cref="Color"/> object representing the HSV color converted to RGB.</returns>
		/// <remarks>
		/// The hue is calculated as a proportion of 'value' to 'maxValue', then adjusted by 'hueShift'.
		/// The resulting hue is wrapped within the range [0, 360]. Saturation is set to 1.
		/// </remarks>
		public static Color GetColorFromValue(int value, int maxValue, float brightness, float hueShift)
		{
			var hue = ((float)value / maxValue * 360 + hueShift) % 360;
			if (hue < 0) hue += 360;  // Ensure hue is not negative

			brightness = Mathf.Clamp01(brightness);
			return Color.HSVToRGB(hue / 360f, 1f, brightness);
		}
	}
}