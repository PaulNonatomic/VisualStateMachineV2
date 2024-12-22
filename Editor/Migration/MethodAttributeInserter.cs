using System.Text.RegularExpressions;

namespace Nonatomic.VSM2.Editor.Migration
{
	public static class MethodAttributeInserter
	{
		/*
		 * Explanation of this pattern (in Multiline mode):
		 *
		 *  ^(?<indent>[^\S\r\n]*)
		 *    - '^[^\S\r\n]*' means at the start of a line (^), capture any whitespace 
		 *      that is *not* a newline or carriage return. (i.e. spaces or tabs)
		 *
		 *  (?<body>(\[[^\]]*\]\s*)*(public|protected|private|internal)?\s*
		 *             (virtual|abstract|override|static|sealed|async|\s)*[^\s]+\s+
		 *             (<METHOD_NAME_HERE>)\s*\([^)]*\)\s*\{)
		 *    - Captures the rest of the line (any existing attributes, modifiers, method name, etc.)
		 *    - <METHOD_NAME_HERE> is a placeholder that we'll replace with the actual method name.
		 *
		 * RegexOptions.Multiline => '^' and '$' match the start/end of *each* line in the input.
		 */
		private static readonly Regex _methodRegexTemplate = new Regex(
			@"^(?<indent>[^\S\r\n]*)(?<body>(\[[^\]]*\]\s*)*(public|protected|private|internal)?\s*(virtual|abstract|override|static|sealed|async|\s)*[^\s]+\s+(<METHOD_NAME_HERE>)\s*\([^)]*\)\s*\{)",
			RegexOptions.Compiled | RegexOptions.Multiline
		);

		/// <summary>
		/// Inserts <paramref name="attributeToInsert"/> above each matching method (by exact name),
		/// preserving indentation and ensuring no duplicate attributes or extra blank lines.
		/// </summary>
		public static string InsertAttributeAboveMethod(
			string sourceCode,
			string methodName,
			string attributeToInsert)
		{
			if (string.IsNullOrEmpty(sourceCode)) return sourceCode;
			if (string.IsNullOrEmpty(methodName))  return sourceCode;
			if (string.IsNullOrEmpty(attributeToInsert)) return sourceCode;

			// Build a new regex by substituting the actual method name
			var pattern = _methodRegexTemplate
				.ToString()
				.Replace("<METHOD_NAME_HERE>", Regex.Escape(methodName));

			var methodRegex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Multiline);

			// Perform the replacement
			return methodRegex.Replace(sourceCode, match =>
			{
				var indent = match.Groups["indent"].Value; // Only spaces or tabs
				var body   = match.Groups["body"].Value;

				// If we already have the attribute in the body, don't add it again
				if (body.Contains(attributeToInsert))
				{
					return match.Value; 
				}

				/*
				 * We rebuild:
				 *	<indent>[Attribute]
				 *	<indent>public void OnEnter() {
				 *		...
				 *	}
				 */
				return $"{indent}{attributeToInsert}\n{indent}{body}";
			});
		}
	}
}
