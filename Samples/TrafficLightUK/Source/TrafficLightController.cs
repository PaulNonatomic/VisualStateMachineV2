using Samples.TrafficLightUK.States;
using UnityEngine;

namespace Samples.TrafficLightUK
{
	public class TrafficLightController : MonoBehaviour
	{
		[SerializeField] private LightController _redLight;
		[SerializeField] private LightController _amberLight;
		[SerializeField] private LightController _greenLight;

		public void ToggleLights(bool redOn, bool amberOn, bool greenOn)
		{
			_redLight.Toggle(redOn);
			_amberLight.Toggle(amberOn);
			_greenLight.Toggle(greenOn);
		}
	}
}