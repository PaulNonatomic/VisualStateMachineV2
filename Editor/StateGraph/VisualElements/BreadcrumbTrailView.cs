using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.StateGraph;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class BreadcrumbTrailView : VisualElement
	{
		private readonly List<BreadcrumbView> _breadcrumbTrail = new ();
		
		public BreadcrumbTrailView()
		{
			name = "breadcrumb-trail";
			ApplyStyle();
		}
		
		private void ApplyStyle()
		{
			var styleSheet = UnityEngine.Resources.Load<StyleSheet>(nameof(BreadcrumbTrailView));
			styleSheets.Add(styleSheet);
		}

		public void SetModel(StateMachineModel model)
		{
			if(!model) return;
			
			_breadcrumbTrail.Clear();
			Clear();
			CreateBreadcrumb(model);
			CreateBreadcrumbOrigin(model);
			AddBreadcrumbsToTrail();
		}

		private void AddBreadcrumbsToTrail()
		{
			_breadcrumbTrail.Reverse();
			
			foreach (var breadcrumb in _breadcrumbTrail)
			{
				Add(breadcrumb);
				
				if (_breadcrumbTrail.Count > 1)
				{
					breadcrumb.ToggleTip(true);
				}
			}
			
			_breadcrumbTrail.First().SetAsOrigin();
			_breadcrumbTrail.Last().SetAsEnd();
		}

		private void CreateBreadcrumbOrigin(StateMachineModel model)
		{
			while (true)
			{
				if (!model) return;

				var modelParent = model.Parent;
				if (!modelParent) return;
				if (modelParent == model) return;

				CreateBreadcrumb(modelParent);
				model = modelParent;
			}
		}

		private void CreateBreadcrumb(StateMachineModel model)
		{
			if(!model) return;
			
			var breadcrumb = new BreadcrumbView();
			breadcrumb.SetModel(model);
			
			_breadcrumbTrail.Add(breadcrumb);
		}
	}
}