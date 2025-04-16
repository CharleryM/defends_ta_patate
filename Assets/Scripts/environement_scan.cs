using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
public class PlaceOnPlane : MonoBehaviour
{
    public GameObject objectToPlace;
    private ARRaycastManager _raycastManager;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    void Start()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (_raycastManager.Raycast(touch.position, _hits,
                    TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = _hits[0].pose;
                Instantiate(objectToPlace, hitPose.position,
                    hitPose.rotation);
            }
        }
    }
}