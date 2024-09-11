using System;
using System.Reflection;
using Nonatomic.VSM2.NodeGraph;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
#pragma warning disable 0067
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class EnterAttribute : Attribute
	{
		private PortModel _portModel = new ();
		
		public EnterAttribute()
		{
			_portModel.PortLabel = "Enter";
		}
		
		public EnterAttribute(string portLabel = default)
		{
			_portModel.PortLabel = portLabel;
		}
		
		public PortModel GetPortData(MethodInfo methodInfo, int methodIndex)
		{
			_portModel.Id = methodInfo.Name;
			_portModel.Index = methodIndex;
			
			var args = methodInfo.GetParameters();
			if (args.Length > 0)
			{
				_portModel.SetPortType(args[0].ParameterType);
			}
			
			return _portModel;
		}
	}
}