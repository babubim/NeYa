using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DogController : MonoBehaviour
{
    [SerializeField] private float _speedWalk = 5f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;

    private CharacterController _characterController;
    private Vector3 _walkDirection;
    private float _verticalVelocity;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;

        // Убираем наклон камеры вверх/вниз
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Движение относительно камеры
        _walkDirection = cameraForward * z + cameraRight * x;

        if (_walkDirection.magnitude > 1f)
        {
            _walkDirection.Normalize();
        }

        // Поворот собаки в сторону движения
        if (_walkDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_walkDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }

        // Движение
        _characterController.Move(
            _walkDirection * _speedWalk * Time.deltaTime
        );

        DoGravity();
    }

    private void DoGravity()
    {
        if (_characterController.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
        }

        _characterController.Move(
            new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime
        );
    }
}