
using UnityEngine;

[System.Serializable]
public class GhostPoint
{
    public Transform point;
    public float waitTime = 1f;
}

public class GhostMovement : MonoBehaviour
{
    [Header("Маршрут")]
    [SerializeField] private GhostPoint[] _points;
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed = 360f;

    [Header("Прохождение препятствий")]
    [SerializeField] private LayerMask _obstacleLayers;
    [SerializeField] private float _slowSpeed = 0.4f;
    [SerializeField] private float _checkRadius = 0.15f;
    [SerializeField] private float _checkDistance = 1.2f;
    [SerializeField] private float _speedChange = 2f;

    [Header("Парение")]
    [SerializeField] private Transform _model;
    [SerializeField] private float _floatHeight = 0.15f;
    [SerializeField] private float _floatSpeed = 2f;

    private Vector3 _startPosition;
    private Vector3 _routeDirection;

    private int _currentPoint = 1;
    private float _waitTimer;
    private float _currentSpeed;

    private Transform _lookTarget;

    public bool IsPassingThrough { get; private set; }

    private void Start()
    {
        _currentSpeed = _moveSpeed;

        // Запоминаем исходное направление призрака
        _routeDirection = transform.forward;

        if (_model != null)
            _startPosition = _model.localPosition;

        // Если маршрута нет, призрак остаётся на месте
        if (_points == null || _points.Length < 2 ||
            _points[0].point == null)
            return;

        transform.position = _points[0].point.position;
        _waitTimer = _points[0].waitTime;
    }

    private void Update()
    {
        Move();
        Float();
    }

    private void LateUpdate()
    {
        if (_lookTarget != null)
        {
            // Во время испуга смотрим на собаку
            Rotate(_lookTarget.position - transform.position);
        }
        else
        {
            // Возвращаемся к обычному направлению
            Rotate(_routeDirection);
        }
    }

    // Указываем цель взгляда
    public void SetLookTarget(Transform target)
    {
        _lookTarget = target;
    }

    private void Move()
    {
        // Нет маршрута - не двигаемся
        if (_points == null || _points.Length < 2)
            return;

        if (_points[_currentPoint].point == null)
            return;

        // Ожидание на точке маршрута
        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            IsPassingThrough = false;
            return;
        }

        Vector3 target = _points[_currentPoint].point.position;
        Vector3 direction = target - transform.position;

        // Запоминаем направление движения
        if (direction.sqrMagnitude > 0.0001f)
        {
            _routeDirection = direction;
        }

        // Проверка препятствий
        float targetSpeed = _moveSpeed;

        bool inside = Physics.CheckSphere(
            transform.position,
            _checkRadius,
            _obstacleLayers,
            QueryTriggerInteraction.Ignore
        );

        IsPassingThrough = inside;

        if (inside)
        {
            targetSpeed = _slowSpeed;
        }
        else if (direction.sqrMagnitude > 0.0001f)
        {
            if (Physics.SphereCast(
                transform.position,
                _checkRadius,
                direction.normalized,
                out RaycastHit hit,
                Mathf.Min(_checkDistance, direction.magnitude),
                _obstacleLayers,
                QueryTriggerInteraction.Ignore))
            {
                IsPassingThrough = true;

                float distance = Mathf.Clamp01(
                    hit.distance / _checkDistance
                );

                targetSpeed = Mathf.Lerp(
                    _slowSpeed,
                    _moveSpeed,
                    distance
                );
            }
        }

        // Плавное изменение скорости
        _currentSpeed = Mathf.MoveTowards(
            _currentSpeed,
            targetSpeed,
            _speedChange * Time.deltaTime
        );

        // Движение по маршруту
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            _currentSpeed * Time.deltaTime
        );

        // Переход к следующей точке
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _waitTimer = Mathf.Max(
                0f,
                _points[_currentPoint].waitTime
            );

            _currentPoint = (_currentPoint + 1) % _points.Length;
        }
    }

    private void Rotate(Vector3 direction)
    {
        // Поворачиваемся только по горизонтали
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }

    private void Float()
    {
        if (_model == null)
            return;

        float offset =
            Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;

        _model.localPosition =
            _startPosition + Vector3.up * offset;
    }
}
