using System.Collections.Generic;
using LearnXR.Core;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.OpenXR.Features.Interactions;
using InputDevice = UnityEngine.XR.InputDevice;
using Logger = LearnXR.Core.Logger;

public class GazeInputManager : Singleton<GazeInputManager>
{
    private InputDevice eyeTrackingDevice;
    public bool EyeTrackingPermissionGranted { get; private set; }
    public Vector3 GazePosition { get; private set; }
    public Quaternion GazeRotation { get; private set; }
    
    void Start()
    {
        Debug.Log("Starting eye tracking");
        MagicLeap.Android.Permissions.RequestPermission(MLPermission.EyeTracking, OnPermissionGranted, 
            OnPermissionDenied, OnPermissionDenied);
    }

    private void Update()
    {
        Debug.Log("Updating eye tracking");

        if (!EyeTrackingPermissionGranted) {
            Debug.Log("Eye tracking permission not granted");
            return; 
        } 
       
        if (!eyeTrackingDevice.isValid)
        {
            Debug.Log("Eye tracking device not valid");
            List<InputDevice> inputDeviceList = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, inputDeviceList);
            //Logger not working, use Debug.Log instead
            //Logger.Instance.LogError($"Eye devices found: {inputDeviceList.Count}");
            if (inputDeviceList.Count > 0)
            {
                eyeTrackingDevice = inputDeviceList[0];
                Debug.Log("Eye device found, more than 1: " + inputDeviceList[0].name);
            }

            if (!eyeTrackingDevice.isValid)
            {
                Logger.Instance.LogWarning($"Unable to get eye tracking information");
                Debug.Log("Unable to get eye tracking information");
                return;
            }
        }

        Debug.Log("Eye tracking device valid");
        
        bool hasData = eyeTrackingDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked);
        hasData &= eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazePosition, out Vector3 position);
        hasData &= eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazeRotation, out Quaternion rotation);
        Debug.Log("isTracked: " + isTracked);
        Debug.Log("hasData: " + hasData);
        if (isTracked && hasData)
        {
            GazePosition = position;
            GazeRotation = rotation;
            Debug.Log("Gaze position: " + GazePosition);
            Debug.Log("Gaze rotation: " + GazeRotation);
        }
    }

    private void OnPermissionDenied(string permission)
    {
        Logger.Instance.LogError($"Eye tracking permission denied.");
        Debug.Log("Eye tracking permission denied.");
    }

    private void OnPermissionGranted(string permission)
    {
        EyeTrackingPermissionGranted = true;
    }
}