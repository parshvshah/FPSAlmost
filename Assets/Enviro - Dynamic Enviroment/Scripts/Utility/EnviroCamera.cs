using UnityEngine;
using System.Collections;

public class EnviroCamera : MonoBehaviour {

	private Camera myCam;

	void Start ()
	{
		myCam = GetComponent<Camera> ();
	}

	void OnPreCull () 
	{
		if (!EnviroSky.instance.started || EnviroSky.instance.PlayerCamera == null)
			return;

		if (myCam != null) {
			myCam.aspect = EnviroSky.instance.PlayerCamera.aspect;
			myCam.fieldOfView = EnviroSky.instance.PlayerCamera.fieldOfView;
		}
		//if (UnityEngine.VR.VRSettings.enabled) {
		//	transform.parent.localPosition = UnityEngine.VR.InputTracking.GetLocalPosition (UnityEngine.VR.VRNode.CenterEye);
		//	transform.parent.localRotation = UnityEngine.VR.InputTracking.GetLocalRotation (UnityEngine.VR.VRNode.CenterEye);
		//} else {
			transform.parent.localPosition = EnviroSky.instance.PlayerCamera.transform.localPosition;
			transform.parent.localRotation = EnviroSky.instance.PlayerCamera.transform.localRotation;
		//}
	}
}
