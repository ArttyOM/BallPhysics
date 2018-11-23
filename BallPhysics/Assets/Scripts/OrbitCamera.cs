using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт, реализующий вращение камеры с помощью клавиатуры и мыши.
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    private Camera _camera;
    private Transform _target;
    private Transform _cameraTransform;

    public float rotSpeed = 1.5f;

    private float _rotY;
    private float _rotX;
    private Vector3 _offset;

    private void Awake()
    {
        Messenger<Transform>.AddListener(MessengerEvents.kCameraOwnerChanged, OnCameraOwnerChanged);
    }
    private void OnDestroy()
    {
        Messenger<Transform>.RemoveListener(MessengerEvents.kCameraOwnerChanged, OnCameraOwnerChanged);
    }

    void Start() {
        _camera = Camera.main;
        _cameraTransform = _camera.transform;
        _target = _cameraTransform.parent;//первичный владелец камеры
        _rotY = _cameraTransform.transform.eulerAngles.y;
        _rotX = _cameraTransform.transform.eulerAngles.x;
        _offset = _target.position - _cameraTransform.position;
    }

    // Update is called once per frame
    void LateUpdate() {
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");
        if (horInput != 0) {
            _rotY -= horInput * rotSpeed; //медленный поворот клавишами
        } else
        if (verInput!=0)
        {
            _rotX += verInput * rotSpeed;
        }
        if (Input.GetMouseButton(1)){
            _rotY += Input.GetAxis("Mouse X") * rotSpeed * 3; //быстрый поворот мышкой
            _rotX += Input.GetAxis("Mouse Y") * rotSpeed * 3;
        }

        Quaternion rotation = Quaternion.Euler(_rotX, _rotY, 0);
        _cameraTransform.position = _target.position - (rotation * _offset);
        _cameraTransform.LookAt(_target);
    }

    private void OnCameraOwnerChanged(Transform newOwner)
    {
        _target = newOwner;//мы смотрим на того, к кому камера прикреплена

    }
}
