using System;

namespace Nonatomic.VSM2.NodeGraph
{
	[Serializable]
	public struct TransitionEventData
	{
		public static readonly TransitionEventData Empty = new (null, null);
		public bool HasValue => _value != null;
		
		private object _value;
		private Type _valueType;

		public TransitionEventData(object value, Type valueType)
		{
			_value = value;
			_valueType = valueType;
		}
		
		public T GetValueOrDefault<T>(T defaultValue = default)
		{
			return TryGetValue<T>(out var result) 
				? result 
				: defaultValue;
		}
		
		public T GetValue<T>()
		{
			if (_value == null)
			{
				throw new InvalidOperationException("No value is present.");
			}
			
			if (typeof(T) != _valueType && !_valueType.IsAssignableFrom(typeof(T)))
			{
				throw new InvalidCastException($"Cannot cast value of type '{_valueType.Name}' to '{typeof(T).Name}'.");
			}
			
			return (T)_value;
		}
	
		public bool TryGetValue<T>(out T result)
		{
			if (_value != null && _valueType == typeof(T))
			{
				result = (T)_value;
				return true;
			}
			
			result = default;
			return false;
		}
		
		public bool HasValueOfType<T>()
		{
			return _value != null && _valueType == typeof(T);
		}
	}
}