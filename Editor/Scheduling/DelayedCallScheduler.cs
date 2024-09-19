using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Scheduling
{
	public static class DelayedCallScheduler
	{
		private static readonly HashSet<string> _scheduledActions = new HashSet<string>();

		public static void ScheduleAction(string actionKey, Action action)
		{
			if (_scheduledActions.Contains(actionKey)) return;

			actionKey += Guid.NewGuid().ToString();
			Debug.Log($"ScheduleAction: {actionKey}");

			_scheduledActions.Add(actionKey);
			EditorApplication.delayCall += () =>
			{
				action();
				_scheduledActions.Remove(actionKey);
			};
		}
	}

}