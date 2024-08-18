using Nonatomic.VSM2.Editor.Persistence;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public static class CopyPasteHelper
	{
		public static CopiedData LastCopy => _lastCopy;
		private static CopiedData _lastCopy;
		
		public static void CacheCopiedData(CopiedData copy)
		{
			_lastCopy = copy;
		}

		public static void ClearCopyCache()
		{
			_lastCopy = null;
		}
	}
}