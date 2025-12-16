using System;

namespace Nonatomic.VSM2.NodeGraph
{
	[Serializable]
	public struct TransitionEventData
	{
		public static readonly TransitionEventData Empty = new (null, null);
		public bool HasValue => Value != null;
		public object Value { get; }
		public Type Type { get; }

		public TransitionEventData(object value, Type valueType)
		{
			Value = value;
			Type = valueType;
		}
		
		public T GetValueOrDefault<T>(T defaultValue = default)
		{
			return TryGetValue<T>(out var result) 
				? result 
				: defaultValue;
		}
		
		public T GetValue<T>()
		{
			if (Value == null)
			{
				throw new InvalidOperationException("No value is present.");
			}
			
			if (typeof(T) != Type && !Type.IsAssignableFrom(typeof(T)))
			{
				throw new InvalidCastException($"Cannot cast value of type '{Type.Name}' to '{typeof(T).Name}'.");
			}
			
			return (T)Value;
		}
	
		public bool TryGetValue<T>(out T result)
		{
			if (Value != null && Type == typeof(T))
			{
				result = (T)Value;
				return true;
			}
			
			result = default;
			return false;
		}
		
		public bool HasValueOfType<T>()
		{
			return Value != null && Type == typeof(T);
		}
	}
}