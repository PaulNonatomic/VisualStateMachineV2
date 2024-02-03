namespace Nonatomic.VSM2.Logging
{
	public static class GraphLog
	{
		public static void Log(object msg)
		{ 
			#if GRAPH_LOG
			Debug.Log(msg);
			#endif
		}

		public static void LogWarning(object msg)
		{
			#if GRAPH_LOG
			Debug.LogWarning(msg);
			#endif
		}
		
		public static void LogError(object msg)
		{
			#if GRAPH_LOG
			Debug.LogError(msg);
			#endif
		}
	}
}