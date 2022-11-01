using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Derived from https://www.youtube.com/watch?v=aLpixrPvlB8&ab_channel=Brackeys with some changes

[RequireComponent(typeof(Camera))]
public class MultiTargetCam : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 camOffset;

    public float minZoomDistance = 12f;
    public float minZoom = 15f;
    public float zoomSlope = 1f;
    public float zoomSpeed = 1f;

    private float smoothTime;
    private Vector3 camVelocity;
    private Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
        smoothTime = .2f;
    }

    void LateUpdate() {
        if (targets.Count == 0) {
            return;
        }
        Move();
        Zoom();
    }

    private void Move() {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + camOffset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref camVelocity, smoothTime);
    }

    private void Zoom() {
        // if we choose a number lower than main for our camera, this will default it to the min defined in this script
        if (cam.orthographicSize < minZoom) {
            cam.orthographicSize = minZoom;
        }

        // by default or newZoom will be the old zoom
        float newZoom = cam.orthographicSize;

        // calculate the current distance between our targets
        float distanceOffset = GetGreatestDistance() - minZoomDistance;

        if (distanceOffset > 0) {
            // if the distance is larger than the mindistacnce threshold, zoom out
            newZoom = minZoom + (distanceOffset * zoomSlope);
        }
        else {
            // if we are below the threshold, make our zoom our minZoom
            newZoom = minZoom;
        }

        // interpolate to our new zoom value
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, zoomSpeed * Time.deltaTime);
    }

    private Vector3 GetCenterPoint() {
        if (targets.Count == 1) {
            return targets[0].position;
        }

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }

    private float GetGreatestDistance() {
        if (targets.Count == 1) {
            return 0;
        }

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }
}
