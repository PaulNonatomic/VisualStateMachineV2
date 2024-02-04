using UnityEngine;

namespace Samples.TrafficLightUK.States
{
	public class LightController : MonoBehaviour
	{
		[SerializeField] private Renderer _lightOn;
		[SerializeField] private Renderer _lightOff;
		
		public void Toggle(bool on)
		{
			_lightOn.enabled = on;
			_lightOff.enabled = !on;
		}
	}
}