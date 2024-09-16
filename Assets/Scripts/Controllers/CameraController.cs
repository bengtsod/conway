using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        private const float MoveSpeed = 10f;
        private const float RotationSpeed = 50f;
        private const float ZoomAmount = 1f;
        private const float ZoomSpeed = 20f;
        private const float MinCameraSize = 10f;
        private const float MaxCameraSize = 50f;

        private Camera _camera;
        private float _targetCameraOrthographicSize;

        private void Start()
        {
            _camera = Camera.main;
            _targetCameraOrthographicSize = _camera!.orthographicSize;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        private void HandleMovement()
        {
            var inputMoveDir = new Vector2(0, 0);
            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDir.y = +1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputMoveDir.y = -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputMoveDir.x = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputMoveDir.x = +1f;
            }

            var moveVector = transform.up * inputMoveDir.y + transform.right * inputMoveDir.x;
            transform.position += moveVector * (MoveSpeed * Time.deltaTime);
        }

        private void HandleRotation()
        {
            var rotationVector = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.Q))
            {
                rotationVector.z = +1f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotationVector.z = -1f;
            }

            transform.eulerAngles += rotationVector * (RotationSpeed * Time.deltaTime);
        }

        private void HandleZoom()
        {
            switch (Input.mouseScrollDelta.y)
            {
                case < 0:
                    _targetCameraOrthographicSize -= ZoomAmount;
                    break;
                case > 0:
                    _targetCameraOrthographicSize += ZoomAmount;
                    break;
            }

            _targetCameraOrthographicSize = Mathf.Clamp(_targetCameraOrthographicSize, MinCameraSize, MaxCameraSize);
            _camera.orthographicSize =
                Mathf.Lerp(_camera.orthographicSize, _targetCameraOrthographicSize, Time.deltaTime * ZoomSpeed);
        }
    }
}