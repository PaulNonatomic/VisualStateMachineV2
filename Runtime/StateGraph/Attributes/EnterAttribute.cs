using System;
using System.Reflection;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.Utils;

#if UNITY_EDITOR
#endif

namespace Nonatomic.VSM2.StateGraph
{
	#pragma warning disable 0067
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class EnterAttribute : Attribute
	{
		private PortModel _portModel = new ();
		
		public EnterAttribute(string portLabel = default)
		{
			_portModel.PortLabel = portLabel;
		}
		
		public PortModel GetPortData(MethodInfo methodInfo, int methodIndex)
		{
			_portModel.Id = methodInfo.Name;
			_portModel.Index = methodIndex;

			#if UNITY_EDITOR
			{
				if (string.IsNullOrEmpty(_portModel.PortLabel))
				{
					_portModel.PortLabel = StringUtils.ProcessPortName(methodInfo.Name);
				}
			}
			#endif
			
			var args = methodInfo.GetParameters();
			if (args.Length > 0)
			{
				_portModel.SetPortType(args[0].ParameterType);
			}
			
			return _portModel;
		}
	}
}