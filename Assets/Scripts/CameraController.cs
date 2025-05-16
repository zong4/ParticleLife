using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10f;

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
        var horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(new Vector3(horizontal, vertical, 0));
    }

    private void HandleZoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        _cam.orthographicSize -= scroll * zoomSpeed;
        // _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
    }
}