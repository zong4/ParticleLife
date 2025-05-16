using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 10f;

    public float zoomSpeed = 2f;
    // public float minZoom = 5f;
    // public float maxZoom = 20f;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        var horizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        var vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;

        transform.Translate(new Vector3(horizontal, vertical, 0));
    }

    private void HandleZoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        _cam.orthographicSize -= scroll * zoomSpeed;
        // _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
    }

    public void SetMovementSpeed(string str)
    {
        if (!float.TryParse(str, out var result)) return;
        movementSpeed = result;
    }

    public void SetZoomSpeed(string str)
    {
        if (!float.TryParse(str, out var result)) return;
        zoomSpeed = result;
    }
}