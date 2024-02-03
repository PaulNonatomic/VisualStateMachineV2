using Nonatomic.VSM2.Logging;
using UnityEngine;

namespace Nonatomic.VSM2.Utils
{
	public static class GuardUtils
	{
		public static bool GuardAgainstRuntimeOperation(string errorMessage = "Cannot perform this operation at runtime")
		{
			if (Application.isPlaying)
			{
				GraphLog.LogError(errorMessage);
				return true;
			}
			
			return false;
		}
	}
}