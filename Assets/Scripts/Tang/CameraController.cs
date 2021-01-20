using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float cameraSmoothingFactor = 1;
    [SerializeField]
    private float lookUpMax = 60;
    [SerializeField]
    private float lookUpMin = -60;

    [SerializeField]
    private Transform t_camera;

    private Quaternion camRotation;
    private RaycastHit hit;
    private Vector3 camera_offset;

    private Vector2 cameraInput;



    // Start is called before the first frame update
    void Start()
    {
        camRotation = transform.localRotation;
        camera_offset = t_camera.localPosition;
    }

    void LateUpdate()
    {
        camRotation.x += cameraInput.y * cameraSmoothingFactor * -1;
        camRotation.y += cameraInput.x * cameraSmoothingFactor;

        camRotation.x = Mathf.Clamp(camRotation.x, lookUpMin, lookUpMax);

        transform.localRotation = Quaternion.Euler(camRotation.x, camRotation.y, camRotation.z);

       /* if(Physics.Linecast(transform.position, transform.position + transform.localRotation*camera_offset, out hit))
        {
            Debug.Log(hit.collider);
            t_camera.localPosition = new Vector3(0, 0, -Vector3.Distance(transform.position, hit.point));
        } else
        {
            t_camera.localPosition = Vector3.Lerp(t_camera.localPosition, camera_offset, Time.deltaTime);
        }*/
    }

    public void GetCameraValues(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
        cameraInput.Normalize();
    }
}
