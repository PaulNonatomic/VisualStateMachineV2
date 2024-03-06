using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.StateGraph;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class BreadcrumTrailView : VisualElement
	{
		private List<BreadcrumbView> _breadcrumbTrail = new ();
		
		public BreadcrumTrailView()
		{
			name = "breadcrumb-trail";
			ApplyStyle();
		}
		
		private void ApplyStyle()
		{
			var style = UnityEngine.Resources.Load<StyleSheet>(nameof(BreadcrumTrailView));
			styleSheets.Add(style);
		}

		public void SetModel(StateMachineModel model)
		{
			_breadcrumbTrail.Clear();
			this.Clear();
			
			CreateBreadcrumb(model);
			CreateBreadcrumbOrigin(model);
			AddBreadcrumbsToTrail();
		}

		private void AddBreadcrumbsToTrail()
		{
			_breadcrumbTrail.Reverse();
			
			for (var index = 0; index < _breadcrumbTrail.Count; index++)
			{
				var breadcrumb = _breadcrumbTrail[index];
				this.Add(breadcrumb);
				
				if(_breadcrumbTrail.Count > 1) breadcrumb.ToggleTip(true);
			}
			
			_breadcrumbTrail.First().SetAsOrigin();
			_breadcrumbTrail.Last().SetAsEnd();
		}

		private void CreateBreadcrumbOrigin(StateMachineModel model)
		{
			var modelParent = model.Parent;
			if (modelParent == null) return;
			if (modelParent == model) return;
			
			CreateBreadcrumb(modelParent);
			CreateBreadcrumbOrigin(modelParent);
		}
		
		private void CreateBreadcrumb(StateMachineModel model)
		{
			var breadcrum = new BreadcrumbView();
			breadcrum.SetText(model.ModelName);
			
			_breadcrumbTrail.Add(breadcrum);
		}
	}
}