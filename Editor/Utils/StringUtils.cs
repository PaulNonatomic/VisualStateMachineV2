using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Nonatomic.VSM2.Editor.Utils
{
	public static class StringUtils
	{
		public static string RemoveStateSuffix(string title)
		{
			if (title.Length < 5) return title;
			return !title.EndsWith("State", StringComparison.OrdinalIgnoreCase) 
				? title : 
				title[..^5];
		}

		public static string RemoveOnSuffix(string title)
		{
			if (title.Length < 2) return title;
			return !title.StartsWith("On", StringComparison.OrdinalIgnoreCase) 
				? title 
				: title[2..];
		}
		
		public static string ApplyEllipsis(string text, int maxLength)
		{
			if (maxLength - 3 < 0 || maxLength - 3 > text.Length) return text;
			
			return text.Length <= maxLength
				? text
				: text[..(maxLength - 3)] + "...";
		}
		
		public static string PascalCaseToTitleCase(string pascalCaseString)
		{
			var withSpaces = Regex.Replace(pascalCaseString, "(\\B[A-Z])", " $1");
			var textInfo = CultureInfo.CurrentCulture.TextInfo;
			
			return textInfo.ToTitleCase(withSpaces);
		}
		
		public static int FindLevenshteinDistance(string s, string t)
		{
			var n = s.Length;
			var m = t.Length;
			var d = new int[n + 1, m + 1];

			if (n == 0) return m;
			if (m == 0) return n;

			for (var i = 0; i <= n; d[i, 0] = i++) { }
			for (var j = 0; j <= m; d[0, j] = j++) { }

			for (var i = 1; i <= n; i++)
			{
				for (var j = 1; j <= m; j++)
				{
					var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
					d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
				}
			}

			return d[n, m];
		}
		
		public static string ProcessPortName(string portName)
		{
			portName = RemoveOnSuffix(portName);
			portName = PascalCaseToTitleCase(portName);
			portName = ApplyEllipsis(portName, 30);
			
			return portName;
		}
		
		public static string ProcessNodeTitle(string title)
		{
			title = PascalCaseToTitleCase(title);
			title = RemoveStateSuffix(title);
			title = ApplyEllipsis(title, 30);

			return title;
		}
	}
}