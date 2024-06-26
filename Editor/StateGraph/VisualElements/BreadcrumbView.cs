using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class BreadcrumbView : VisualElement
	{
		private Label _title;
		private VisualElement _tip;
		private StateMachineModel _model;

		public BreadcrumbView()
		{
			name = "breadcrumb";
			
			Build();
			ApplyStyle();
			ToggleTip(false);
			
			this.AddManipulator(new Clickable(HandleClick));
		}

		public void ToggleTip(bool show)
		{
			AddToClassList(show ? "show-tip" : "hide-tip");
			RemoveFromClassList(show ? "hide-tip" : "show-tip");
		}

		public void SetModel(StateMachineModel model)
		{
			if (!model) return;
			
			_model = model;
			SetText(model.ModelName);
		}

		private void HandleClick(EventBase eventBase)
		{
			if (!_model) return;
			
			ModelSelection.ActiveModel = _model;
		}

		public void SetAsOrigin()
		{
			AddToClassList("origin");
		}

		public void SetAsEnd()
		{
			ToggleTip(false);
			AddToClassList("end");
		}

		private void Build()
		{
			pickingMode = PickingMode.Position;
			
			_title = new Label
			{
				name = "breadcrumb-title",
				pickingMode = PickingMode.Ignore
			};
			Add(_title);
			
			_tip = new VisualElement();
			_tip.AddToClassList("breadcrumb-tip");
			Add(_tip);
		}

		private void ApplyStyle()
		{
			var styleSheet = UnityEngine.Resources.Load<StyleSheet>(nameof(BreadcrumbView));
			styleSheets.Add(styleSheet);
		}

		private void SetText(string txt)
		{
			_title.text = txt;
		}
	}
}