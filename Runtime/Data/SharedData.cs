using System;
using System.Collections.Generic;
using System.Threading;

namespace Nonatomic.VSM2.Data
{
	public interface ISharedData
	{
		event Action<string, object> OnDataChanged;
		event Action OnDataCleared;
		event Action<string> OnDataRemoved;
		
		T GetData<T>(string key);
		bool TryGetData<T>(string key, out T value);
		bool HasData(string key);
		void SetData<T>(string key, T value);
		void RemoveData(string key);
		void ClearAllData();
		IEnumerable<string> GetKeys();
	}
	
	public class SharedData : ISharedData
	{
		public event Action<string, object> OnDataChanged;
		public event Action OnDataCleared;
		public event Action<string> OnDataRemoved;
		
		private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public T GetData<T>(string key)
		{
			_lock.EnterReadLock();
			
			try
			{
				if (!_data.TryGetValue(key, out var value)) return default;
				
				if (value is T typedValue)
				{
					return typedValue;
				}
				
				throw new InvalidOperationException($"Attempted to retrieve type {typeof(T)} but data is of type {value.GetType()}");
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
		
		public bool TryGetData<T>(string key, out T value)
		{
			_lock.EnterReadLock();
			
			try
			{
				if (_data.TryGetValue(key, out var objValue) && objValue is T typedValue)
				{
					value = typedValue;
					return true;
				}
				
				value = default;
				return false;
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
		
		public bool HasData(string key)
		{
			_lock.EnterReadLock();
			
			try
			{
				return _data.ContainsKey(key);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public void SetData<T>(string key, T value)
		{
			_lock.EnterWriteLock();
			
			try
			{
				_data[key] = value;
				OnDataChanged?.Invoke(key, value);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}
		
		public void RemoveData(string key)
		{
			_lock.EnterWriteLock();
			try
			{
				if (_data.Remove(key))
				{
					OnDataRemoved?.Invoke(key);
				}
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}
		
		public void ClearAllData()
		{
			_lock.EnterWriteLock();
			try
			{
				_data.Clear();
				OnDataCleared?.Invoke();
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}
	
		public IEnumerable<string> GetKeys()
		{
			_lock.EnterReadLock();
			try
			{
				return _data.Keys;
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
	}
}