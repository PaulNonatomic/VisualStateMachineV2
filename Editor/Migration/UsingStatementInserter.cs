using System;

namespace Nonatomic.VSM2.Editor.Migration
{
	public static class UsingStatementInserter
	{
		/// <summary>
		/// Inserts a 'using {namespaceToAdd};' statement at the top of the file if 
		/// it's not already present.
		/// </summary>
		/// <param name="sourceCode">Full .cs file contents as a string.</param>
		/// <param name="namespaceToAdd">The namespace you wish to import, e.g. "System.Collections.Generic".</param>
		/// <returns>Updated source code with the using statement inserted if needed.</returns>
		public static string InsertUsingStatementIfMissing(string sourceCode, string namespaceToAdd)
		{
			if (string.IsNullOrEmpty(sourceCode)) return sourceCode;
			if (string.IsNullOrEmpty(namespaceToAdd)) return sourceCode;

			// 1) Check if this using statement is already in the file
			var usingLine = $"using {namespaceToAdd};";
			if (sourceCode.Contains(usingLine))
			{
				return sourceCode; // It's already there
			}

			// 2) Split into lines so we can find where to insert
			var lines = sourceCode.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			// 3) Figure out where the last using statement is
			//    We'll insert our using statement right after the last existing using line.
			var lastUsingIndex = -1;
			for (int i = 0; i < lines.Length; i++)
			{
				var trimmed = lines[i].TrimStart();
				if (trimmed.StartsWith("using ") && trimmed.EndsWith(";"))
				{
					lastUsingIndex = i;
				}
			}

			// 4) Insert the new using line after the last using line (or at the top if none found)
			if (lastUsingIndex == -1)
			{
				// No existing using lines, so place ours at the very top
				var newLines = new string[lines.Length + 1];
				newLines[0] = usingLine;
				Array.Copy(lines, 0, newLines, 1, lines.Length);
				return string.Join("\n", newLines);
			}
			else
			{
				// Insert after last using line
				var newLines = new string[lines.Length + 1];
				Array.Copy(lines, 0, newLines, 0, lastUsingIndex + 1);
				newLines[lastUsingIndex + 1] = usingLine;
				Array.Copy(lines, lastUsingIndex + 1, newLines, lastUsingIndex + 2, lines.Length - (lastUsingIndex + 1));
				return string.Join("\n", newLines);
			}
		}
	}
}
