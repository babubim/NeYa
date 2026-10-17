
using UnityEngine;

public class KostikFollow : MonoBehaviour
{
    [Header("Следование")]
    [SerializeField] private Transform _dog;
    [SerializeField] private Vector3 _offset = new Vector3(0.5f, 0.5f, 0f);
    [SerializeField] private float _followSmoothTime = 0.2f;

    [Header("Парение")]
    [SerializeField] private float _floatHeight = 0.05f;
    [SerializeField] private float _floatSpeed = 2f;

    [Header("Прицеливание")]
    [SerializeField] private float _aimDistance = 100f;

    [Header("Препятствия")]
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private float _collisionRadius = 0.2f;

    private Vector3 _velocity;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_dog == null || _camera == null) return;

        // Позиция рядом с собакой
        Vector3 targetPosition =
            _dog.position + _dog.rotation * _offset;

        // Парение
        float floatOffset =
            Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;

        targetPosition.y += floatOffset;

        // Не позволяем Костику занимать место за стеной
        Vector3 anchor = _dog.position +
            Vector3.up * (_offset.y + floatOffset);

        Vector3 offsetDirection = targetPosition - anchor;
        float offsetDistance = offsetDirection.magnitude;

        if (offsetDistance > 0.001f &&
            Physics.SphereCast(
                anchor,
                _collisionRadius,
                offsetDirection.normalized,
                out RaycastHit offsetHit,
                offsetDistance,
                _wallMask,
                QueryTriggerInteraction.Ignore))
        {
            targetPosition = anchor +
                offsetDirection.normalized *
                Mathf.Max(0f, offsetHit.distance - 0.02f);
        }

        // Плавное следование
        Vector3 nextPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            _followSmoothTime
        );

        // Проверка стены по пути движения
        Vector3 movement = nextPosition - transform.position;

        if (movement.sqrMagnitude > 0.000001f &&
            Physics.SphereCast(
                transform.position,
                _collisionRadius,
                movement.normalized,
                out RaycastHit wallHit,
                movement.magnitude,
                _wallMask,
                QueryTriggerInteraction.Ignore))
        {
            nextPosition = transform.position +
                movement.normalized *
                Mathf.Max(0f, wallHit.distance - 0.02f);

            _velocity = Vector3.zero;
        }

        transform.position = nextPosition;

        // Точка, на которую наведён прицел
        Ray ray = _camera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 aimPoint = ray.GetPoint(_aimDistance);

        if (Physics.Raycast(ray, out RaycastHit hit, _aimDistance))
        {
            aimPoint = hit.point;
        }

        // Поворот Костика к прицелу
        Vector3 direction = aimPoint - transform.position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
