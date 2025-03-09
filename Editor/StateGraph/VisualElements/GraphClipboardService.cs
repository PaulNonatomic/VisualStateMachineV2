using Nonatomic.VSM2.Utils;

namespace Nonatomic.VSM2.Editor.StateGraph.VisualElements
{
	public class GraphClipboardService
	{
		private readonly StateGraphView _graphView;

		public GraphClipboardService(StateGraphView graphView)
		{
			_graphView = graphView;
		}

		public void HandleCopySelected()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			CopyPasteHelper.Copy(_graphView);
		}

		public void HandlePasteSelected()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			CopyPasteHelper.Paste(_graphView);
		}
	}
}