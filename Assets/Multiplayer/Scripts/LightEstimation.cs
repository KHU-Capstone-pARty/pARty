using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class LightEstimation : MonoBehaviour
{
	[SerializeField]
	private	ARCameraManager	arCameraManager;
	private	Light curlight;

	private void Awake()
	{
		if ( arCameraManager == null )
		{
			Debug.LogError("AR Camera Manager is Nothing!!");
			Destroy(this);
		}

		curlight = GetComponent<Light>();
	}

	private void OnEnable()
	{
		arCameraManager.frameReceived += FrameChanged;
	}

	private void OnDisable()
	{
		arCameraManager.frameReceived -= FrameChanged;
	}

	private void FrameChanged(ARCameraFrameEventArgs args)
	{
		if ( args.lightEstimation.averageBrightness.HasValue )
		{
			curlight.intensity = args.lightEstimation.averageBrightness.Value;
		}

		if ( args.lightEstimation.averageColorTemperature.HasValue )
		{
			curlight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
		}

		if ( args.lightEstimation.colorCorrection.HasValue )
		{
			curlight.color = args.lightEstimation.colorCorrection.Value;
		}

		if ( args.lightEstimation.mainLightDirection.HasValue )
		{
			curlight.transform.rotation = Quaternion.LookRotation(args.lightEstimation.mainLightDirection.Value);
		}

		if ( args.lightEstimation.mainLightIntensityLumens.HasValue )
		{
			curlight.intensity = args.lightEstimation.averageMainLightBrightness.Value;
		}

		if ( args.lightEstimation.ambientSphericalHarmonics.HasValue )
		{
			RenderSettings.ambientMode	= AmbientMode.Skybox;
			RenderSettings.ambientProbe	= args.lightEstimation.ambientSphericalHarmonics.Value;
		}
	}
}

