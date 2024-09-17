#if GRAPH_LOG
using UnityEngine;
#endif

namespace Nonatomic.VSM2.Logging
{
	public static class GraphLog
	{
		public static void Log(object msg, Object context = null)
		{ 
			#if GRAPH_LOG
			Debug.Log(msg, context);
			#endif
		}

		public static void LogWarning(object msg, Object context = null)
		{
			#if GRAPH_LOG
			Debug.LogWarning(msg, context);
			#endif
		}
		
		public static void LogError(object msg, Object context = null)
		{
			#if GRAPH_LOG
			Debug.LogError(msg, context);
			#endif
		}
	}
}