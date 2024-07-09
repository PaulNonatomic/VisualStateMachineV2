using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	public abstract class BaseDelayState : State
	{
		[SerializeField, Tooltip("Duration in seconds")] 
		public float Duration = 1f;
	}
}