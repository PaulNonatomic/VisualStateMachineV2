using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class LayerLabelView : VisualElement
	{
		private readonly Label _label;
		private readonly VisualElement _pointy;
		private readonly VisualElement _pointyBackground;

		public LayerLabelView()
		{
			this.name = "layer-label";
			
			_label = new Label();
			_label.name = "title-label";
			Add(_label);
			
			_pointy = new VisualElement();
			_pointy.AddToClassList("pointy-element");
			Insert(0, _pointy);
			
			_pointyBackground = new VisualElement();
			_pointyBackground.AddToClassList("pointy-background");
			Insert(0, _pointyBackground);

			ToggleTip(false);
		}

		public void ToggleTip(bool show)
		{
			_pointy.visible = show;
			_pointyBackground.visible = show;
		}

		public void SetText(string txt)
		{
			_label.text = txt;
		}
	}
}