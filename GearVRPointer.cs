using UnityEngine;
using UnityEngine.Events;

public class GearVRPointer : MonoBehaviour
{
    public int[] door = new int[4] { 0, 0,0,0 };
    public int[] swit = new int[6] {0,0,0,0, 0, 0 };
    public string state = "";



    [System.Serializable]
    public class HoverCallback : UnityEvent<Transform> { }

    [System.Serializable]
    public class SelectionCallback : UnityEvent<Transform> { }

    [Header("OVR Rig Transforms")]
    [Tooltip("Set to: OVRCameraRig/TrackingSpace/LeftHandAnchor")]
    public Transform leftHandAnchor = null;
    [Tooltip("Set to: OVRCameraRig/TrackingSpace/RightHandAnchor")]
    public Transform rightHandAnchor = null;
    [Tooltip("Set to: OVRCameraRig/TrackingSpace/CenterEyeAnchor")]
    public Transform centerEyeAnchor = null;

    [Header("Selection Ray")]
    [Tooltip("If false, hide the selction ray")]
    public bool showSelectionRay = true;
    public LineRenderer lineRenderer = null;
    [Tooltip("Maximum distance the ray can reach, used for raycasting and visualization")]
    public float maxRayDistance = 500.0f;
    [Tooltip("Any layers the ray should not hit")]
    public LayerMask excludeLayers;

    [Header("Hover Callbacks")]
    public GearVRPointer.HoverCallback onHoverEnter;
    public GearVRPointer.HoverCallback onHoverExit;
    public GearVRPointer.HoverCallback onHover;

    [Header("Selection Callbacks")]
    public GearVRPointer.SelectionCallback onTriggerSelect;
    public GearVRPointer.SelectionCallback onPadSelect;
    public GearVRPointer.SelectionCallback onSelected;

    protected Ray pointer;
    protected Transform lastHit = null;
    protected Transform triggerDown = null;
    protected Transform padDown = null;

    public Ray Pointer
    {
        get
        {
            return pointer;
        }
    }

    public bool IsRayAvailable
    {
        get
        {
            return Controller != OVRInput.Controller.None && showSelectionRay && lineRenderer != null;
        }
    }

    public OVRInput.Controller Controller
    {
        get
        {
            OVRInput.Controller controller = OVRInput.GetConnectedControllers();
            if ((controller & OVRInput.Controller.LTrackedRemote) != OVRInput.Controller.None)
            {
                return OVRInput.Controller.LTrackedRemote;
            }
            else if ((controller & OVRInput.Controller.RTrackedRemote) != OVRInput.Controller.None)
            {
                return OVRInput.Controller.RTrackedRemote;
            }

            return OVRInput.Controller.None;
        }
    }

    public Transform ControllerTransform
    {
        get
        {
            OVRInput.Controller controller = Controller;
            if (controller == OVRInput.Controller.LTrackedRemote)
            {
                return leftHandAnchor;
            }
            else if (controller == OVRInput.Controller.RTrackedRemote)
            {
                return rightHandAnchor;
            }
            return centerEyeAnchor;
        }
    }

    void Awake()
    {
        // Find VR Transforms if not set
        if (leftHandAnchor == null)
        {
            Debug.LogWarning("Assign LeftHandAnchor in the inspector!");
            GameObject left = GameObject.Find("LeftHandAnchor");
            if (left != null)
            {
                leftHandAnchor = left.transform;
            }
        }
        if (rightHandAnchor == null)
        {
            Debug.LogWarning("Assign RightHandAnchor in the inspector!");
            GameObject right = GameObject.Find("RightHandAnchor");
            if (right != null)
            {
                rightHandAnchor = right.transform;
            }
        }
        if (centerEyeAnchor == null)
        {
            Debug.LogWarning("Assign CenterEyeAnchor in the inspector!");
            GameObject center = GameObject.Find("CenterEyeAnchor");
            if (center != null)
            {
                centerEyeAnchor = center.transform;
            }
        }

        // Create line renderer if not set
        if (lineRenderer == null)
        {
            Debug.LogWarning("Assign a line renderer in the inspector!");
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.widthMultiplier = 0.02f;
        }

        pointer = new Ray(new Vector3(0, 0, 0), Vector3.forward);
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            door[i] = 0;
            swit[i] = 0;
        }
        swit[4] = 0;
        swit[5] = 0;

        // If used without a OVRManager in the scene
        if (OVRManager.instance == null)
        {
            OVRInput.Update();
        }

        bool isRayAvailable = IsRayAvailable;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = isRayAvailable;
        }

        Transform input = ControllerTransform;
        if (input == null)
        {
            Debug.LogError("Controller transform not found, is OVRCameraRig present in scene?");
            pointer.origin = Vector3.zero;
            pointer.direction = Vector3.forward;
            return;
        }

        pointer.origin = input.position;
        pointer.direction = input.forward;

        if (isRayAvailable)
        {
            lineRenderer.SetPosition(0, pointer.origin);
            lineRenderer.SetPosition(1, pointer.origin + pointer.direction * maxRayDistance);
        }

        RaycastHit hit; // Was anything hit?
        if (Physics.Raycast(pointer, out hit, maxRayDistance, ~excludeLayers))
        {

            // Adjust the ray visual
            if (isRayAvailable)
            {
                lineRenderer.SetPosition(1, hit.point);
            }

            if (lastHit != null && lastHit != hit.transform)
            {
                if (onHoverExit != null)
                {
                    onHoverExit.Invoke(lastHit);
                }
                lastHit = null;
            }

            if (lastHit == null)
            {
                if (onHoverEnter != null)
                {
                    onHoverEnter.Invoke(hit.transform);
                }
            }

            if (onHover != null)
            {
                onHover.Invoke(hit.transform);
            }

            lastHit = hit.transform;
            //
            state = hit.transform.name;
            //
            // Handle selection callbacks. An object is selected if the button selecting it was
            // pressed AND released while hovering over the object.
            if (Controller != OVRInput.Controller.None)
            {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
                {
                    padDown = lastHit;
                }
                else if (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad))
                {
                    if (padDown != null && padDown == lastHit)
                    {
                        if (onPadSelect != null)
                        {
                            onPadSelect.Invoke(padDown);
                        }
                        if (onSelected != null)
                        {
                            onSelected.Invoke(padDown);
                        }
                    }
                }
                if (!OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
                {
                    padDown = null;
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                {
                    triggerDown = lastHit;
                }
                else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
                {
                    if (triggerDown != null && triggerDown == lastHit)
                    {
                        if (onTriggerSelect != null)
                        {
                            onTriggerSelect.Invoke(triggerDown);
                        }
                        if (onSelected != null)
                        {
                            onSelected.Invoke(triggerDown);
                        }
                    }
                }
                if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    triggerDown = null;
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                //               if (Physics.Raycast(pointer, out hit))
                {

                    Debug.Log(hit.transform.name);
                    switch (hit.transform.name) {
                        case "door1":
                            door[0] = 1;
                            break;
                        case "door2":
                            door[1] = 1;
                            break;
                        case "door3":
                            door[2] = 1;
                            break;
                        case "toilet":
                            door[3] = 1;
                            break;
                        case "swit_liv":
                            swit[0] = 1;
                            break;
                        case "swit_room1":
                            swit[1] = 1;
                            break;
                        case "swit_room2":
                            swit[2] = 1;
                            break;
                        case "swit_room3":
                            swit[3] = 1;
                            break;
                        case "swit_kitch":
                            swit[4] = 1;
                            break;
                        case "swit_toilet":
                            swit[5] = 1;
                            break;
                    }
                }
            }
        }
        // Nothing was hit, handle exit callback
        else if (lastHit != null)
        {
            if (onHoverExit != null)
            {
                onHoverExit.Invoke(lastHit);
            }
            lastHit = null;
        }
    }

    private void FixedUpdate()
    {
        // If used without a OVRManager in the scene
        if (OVRManager.instance == null)
        {
            OVRInput.FixedUpdate();
        }
    }
}
