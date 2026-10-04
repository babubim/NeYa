using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DogController : MonoBehaviour
{
    [SerializeField] private float _speedWalk = 5f;
    [SerializeField] private float _gravity = 9.81f;

    private CharacterController _characterController;
    private Vector3 _walkDirection;
    private float _verticalVelocity;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Ввод
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        _walkDirection = transform.right * x + transform.forward * z;

        // Ходьба
        _characterController.Move(_walkDirection * _speedWalk * Time.deltaTime);

        // Гравитация
        DoGravity();
    }

    private void DoGravity()
    {
        if (_characterController.isGrounded && _verticalVelocity < 0f)
        {
            // Небольшое отрицательное значение, чтобы контроллер "прилипал" к полу
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
        }

        _characterController.Move(new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
    }
}