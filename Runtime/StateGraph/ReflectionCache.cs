namespace Nonatomic.VSM2.StateGraph
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public static class ReflectionCache
	{
		private static readonly Dictionary<(Type Type, string EventName), EventInfo> _eventInfoCache = new();
		private static readonly Dictionary<(string MethodName, Type[] GenericArgs), MethodInfo> _genericMethodCache = new();

		public static EventInfo GetEventInfo(Type targetType, string eventName)
		{
			var key = (targetType, eventName);

			if (_eventInfoCache.TryGetValue(key, out var cachedEventInfo))
			{
				return cachedEventInfo;
			}

			var eventInfo = targetType.GetEvent(eventName);
			if (eventInfo != null)
			{
				_eventInfoCache[key] = eventInfo;
			}

			return eventInfo;
		}

		public static MethodInfo GetGenericMethod(
			Type declaringType, 
			string methodName, 
			Type[] argumentTypes, 
			int paramCount)
		{
			var key = (methodName, argumentTypes);

			if (_genericMethodCache.TryGetValue(key, out var cachedMethod))
			{
				return cachedMethod;
			}

			var methodCandidates = declaringType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.Name == methodName && m.IsGenericMethodDefinition);

			foreach (var candidate in methodCandidates)
			{
				var parameters = candidate.GetParameters();
				if (parameters.Length != paramCount) continue;
				
				// make a generic method
				var constructed = candidate.MakeGenericMethod(argumentTypes);
				_genericMethodCache[key] = constructed;
				return constructed;
			}

			return null;
		}
	}
}