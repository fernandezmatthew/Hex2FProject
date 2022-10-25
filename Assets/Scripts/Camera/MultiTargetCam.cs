using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pretty much the same code from https://www.youtube.com/watch?v=aLpixrPvlB8&ab_channel=Brackeys

[RequireComponent(typeof(Camera))]
public class MultiTargetCam : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 camOffset;

    public float minZoomDistance;
    public float maxZoomDistance;
    public float zoomLimiter = 50f;

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
        float newZoom = Mathf.Lerp(minZoomDistance, maxZoomDistance, GetGreatestDistance() / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
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
