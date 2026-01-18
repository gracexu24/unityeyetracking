using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Logger = LearnXR.Core.Logger;

public class GazePasscodeFeature : MonoBehaviour
{
    [SerializeField] private string secretPasscode;
    
    [SerializeField] private UnityEvent OnPasscodeValid = new();
    
    [SerializeField] private UnityEvent OnPasscodeInValid = new();

    [SerializeField] private LayerMask layersToIncludeWithRay;
    [SerializeField] private TextMeshPro passcodeText;
    [SerializeField] [Range(1.0f, 10.0f)] private float minGazeTimeOverNumbers = 2.0f;

    // private variables
    private MeshRenderer[] codeRenderers;
    private Material fillMaterial;
    private float gazeOverCodeTracker;
    private readonly int fillProgressProperty = Shader.PropertyToID("_FillProgress");
    private const float MIN_FILL_RANGE = -0.6f;
    private const float MAX_FILL_RANGE = 0.6f;
    
    private void Awake()
    {
        passcodeText.text = string.Empty;
        var allButtons = GameObject.FindGameObjectsWithTag($"CodeButton");
        codeRenderers = allButtons
            .Select(n => n.GetComponent<MeshRenderer>())
            .ToArray();
    }
    
    void Update()
    {
        Debug.Log("Update Passcode Feature");
        if (!GazeInputManager.Instance.EyeTrackingPermissionGranted) return;
        
        var gazePosition = GazeInputManager.Instance.GazePosition;
        var gazeRotation = GazeInputManager.Instance.GazeRotation;
        
        Ray ray = new Ray(gazePosition, gazeRotation * Vector3.forward);

        // Add a new (Passcode) layer to Unity and the tutorial script
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layersToIncludeWithRay))
        {
            Debug.Log("hit: " + hit.collider.gameObject.name);
            // clear any prior fillings
            ClearAllFillings(hit.transform.gameObject);
            
            var passcodeNumBox = hit.collider.gameObject;
            var passcodeNumBoxRenderer = passcodeNumBox.GetComponent<Renderer>();
            
            var passcodeNumText = passcodeNumBox.GetComponentInChildren<TextMeshPro>();
            Debug.Log("passcodeNumText: " + passcodeNumText.text);

            gazeOverCodeTracker += Time.deltaTime;
            Debug.Log("gazeOverCodeTracker: " + gazeOverCodeTracker);
            var progress = gazeOverCodeTracker / minGazeTimeOverNumbers;
            var fillProgressValue = ConvertPercentageToRange(progress);
            passcodeNumBoxRenderer.material.SetFloat(fillProgressProperty, fillProgressValue);
            
            if (gazeOverCodeTracker >= minGazeTimeOverNumbers)
            {
        
                //Logger.Instance.LogInfo($"(Gaze) Code Selected: {hit.collider.gameObject.name}");
                Debug.Log("Gaze Code Selected: " + hit.collider.gameObject.name);
                Debug.Log("passcodeText: " + passcodeText.text);
                // reset executed
                if (passcodeNumText.text == "RESET")
                {
                    Debug.Log("RESET");
                    passcodeText.text = string.Empty;
                    gazeOverCodeTracker = 0;
                    return;
                }
                
                // clear it out if max
                if (passcodeText.text.Length >= secretPasscode.Length){
                    Debug.Log("maxed");
                    passcodeText.text = string.Empty;
                }
                
                string gazeNumberSelected = passcodeNumText.text;
                passcodeText.text += gazeNumberSelected;

                // check passcode combination
                if (passcodeText.text.Length >= secretPasscode.Length)
                {
                    Debug.Log("passcodeText.text: " + passcodeText.text);
                    Debug.Log("secretPasscode: " + secretPasscode);
                    if (passcodeText.text == secretPasscode)
                    {
                        Debug.Log("Passcode Is Valid - Executing OnPasscodeValid Unity Event...");
                        //Logger.Instance.LogInfo("Passcode Is Valid - Executing OnPasscodeValid Unity Event...");
                        OnPasscodeValid?.Invoke();
                    }
                    else
                    {
                        Debug.Log("Passcode Is Not Valid - Executing OnPasscodeInValid Unity Event...");
                        //Logger.Instance.LogWarning("Passcode Is Not Valid - Executing OnPasscodeInValid Unity Event...");
                        OnPasscodeInValid?.Invoke();
                    }
                }

                // Zero it out to prevent quickly re-typing
                gazeOverCodeTracker = 0;
                passcodeNumBoxRenderer.material.SetFloat(fillProgressProperty, 0);
            }
        }
        else
        {
            gazeOverCodeTracker = 0;
        }
    }
    
    private float ConvertPercentageToRange(float percentage)
    {
        float rangeSize = MAX_FILL_RANGE - (MIN_FILL_RANGE);
        float convertedValue = (percentage * rangeSize) - MAX_FILL_RANGE;
        return convertedValue;
    }

    private void ClearAllFillings(GameObject gameObjectToExclude = null)
    {
        float zeroPercent = ConvertPercentageToRange(0);
        foreach (var currentRenderer in codeRenderers)
        {
            if(currentRenderer.gameObject != gameObjectToExclude)
                currentRenderer.material.SetFloat(fillProgressProperty, zeroPercent);
        }
    }
}