using System;
using System.Reflection;
using UnityEngine;

namespace Nonatomic.VSM2.NodeGraph
{
	[AttributeUsage(AttributeTargets.Event)]
	public class TransitionAttribute : Attribute
	{
		private PortModel _portModel = new ();
		
		public TransitionAttribute()
		{
			
		}
		
		public TransitionAttribute(string portLabel = default, string portColor = default, int frameDelay = 1)
		{
			_portModel.PortLabel = portLabel;
			_portModel.PortColor = portColor;
			_portModel.FrameDelay = frameDelay;
		}
		
		public PortModel GetPortData(EventInfo eventInfo, int eventIndex)
		{
			_portModel.Id = eventInfo.Name;
			_portModel.Index = eventIndex;

			var args = eventInfo.EventHandlerType.GenericTypeArguments;
			if (args.Length > 0)
			{
				_portModel.TransitionType = args[0];
			}
			
			return _portModel;
		}
	}
}