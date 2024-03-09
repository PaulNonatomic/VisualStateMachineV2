using System;

namespace Nonatomic.VSM2.NodeGraph
{
	public static class ModelSelection
	{
		public static event Action<NodeGraphDataModel> OnModelSelected;

		public static NodeGraphDataModel ActiveModel
		{
			get => _selectedModel;
			
			set
			{
				if (value == _selectedModel) return;
				
				_selectedModel = value;
				OnModelSelected?.Invoke(_selectedModel);
			}
		}

		private static NodeGraphDataModel _selectedModel;
	}
}