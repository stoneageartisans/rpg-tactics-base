using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float distance = 30.0f;
    public float minDistance = 15.0f;
    public float maxDistance = 50.0f;

    public float horizontalSpeed = 10.0f;
    public float verticalSpeed = 100.0f;
    public float zoomSpeed = 20.0f;

    public float minVerticalAngle = 30.0f;
    public float maxVerticalAngle = 90.0f;

    float rotationX = 0.0f;
    float rotationY = 0.0f;

    Transform target;

    void Start() 
    {
        target = (new GameObject()).transform;

        rotationX = transform.eulerAngles.y;
        rotationY = transform.eulerAngles.x;
    }

    void Update() 
    {
        rotationX += (Input.GetAxis("Mouse X") * horizontalSpeed * distance * 0.02f);
        rotationY -= (Input.GetAxis("Mouse Y") * verticalSpeed * 0.02f);

        if(rotationY < -360.0f)
        {
            rotationY += 360.0f;
        }

        if(rotationY > 360.0f)
        {
            rotationY -= 360.0f;
        }

        rotationY = Mathf.Clamp(rotationY, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0.0f);

        distance = Mathf.Clamp(distance - (Input.GetAxis("Mouse ScrollWheel") * zoomSpeed), minDistance, maxDistance);

        Vector3 position = (target.position + (rotation * (new Vector3(0.0f, 0.0f, -distance))));

        transform.rotation = rotation;
        transform.position = position;
    }
}
