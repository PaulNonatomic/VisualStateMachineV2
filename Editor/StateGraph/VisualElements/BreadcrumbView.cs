using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class BreadcrumbView : VisualElement
	{
		private Label _title;
		private VisualElement _tip;

		public BreadcrumbView()
		{
			name = "breadcrumb";
			
			Build();
			ApplyStyle();
			ToggleTip(false);
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
			_title = new Label();
			_title.name = "breadcrumb-title";
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

		public void ToggleTip(bool show)
		{
			if (show)
			{
				AddToClassList("show-tip");
				RemoveFromClassList("hide-tip");
			}
			else
			{
				AddToClassList("hide-tip");
				RemoveFromClassList("show-tip");
			}
		}

		public void SetText(string txt)
		{
			_title.text = txt;
		}
	}
}