using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseLook : MonoBehaviour
    {
        [SerializeField] private  float mouseSensitivity = 1500f;
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform orientation;
        [Header("SMOOTHNESS")]
        [Range(0,1)]
        [SerializeField] private float cameraSmoothness;
        [SerializeField] private SmoothnessMethod smoothnessMethod;

        private float xRotation;
        private float yRotation;

        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            
            yRotation += mouseX;
            if (smoothnessMethod == SmoothnessMethod.Lerp)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(xRotation, yRotation, 0f), cameraSmoothness);

            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRotation, yRotation, 0f), cameraSmoothness);

            }
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

public enum SmoothnessMethod
{
    Lerp,
    Slerp
}