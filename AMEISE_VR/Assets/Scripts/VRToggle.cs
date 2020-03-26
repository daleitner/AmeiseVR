using Assets.Scripts;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Switches between 1st Person Player and VR-Player
/// </summary>
public static class VRToggle
{
	private const string Untagged = "Untagged";
	private const string MainCamera = "MainCamera";
    public static void ActivateVR(bool value)
    {
	    var fpsCameraTag = value ? Untagged : MainCamera;
	    var vrCameraTag = value ? MainCamera : Untagged;
		GameObjectCollection.FPSController.SetActive(!value);
		GameObjectCollection.VRPlayer.SetActive(value);
		GameObjectCollection.FPSController.transform.Find("FirstPersonCharacter").gameObject.tag = fpsCameraTag;
		GameObjectCollection.VRPlayer.transform.Find("SteamVRObjects").Find("VRCamera").gameObject.tag = vrCameraTag;
		GameObjectCollection.Teleporting.SetActive(value);
		Screen.orientation = value ? ScreenOrientation.Portrait : ScreenOrientation.AutoRotation;
		XRSettings.enabled = value;
		IsVRActive = value;
    }

    public static bool IsVRActive { get; private set; }

    public static void ToggleVR()
    {
		ActivateVR(!IsVRActive);
    }
}
