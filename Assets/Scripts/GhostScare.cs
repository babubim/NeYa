
using UnityEngine;

public class GhostScare : MonoBehaviour
{
    [Header("Запугивание")]
    [SerializeField] private float _scareRadius = 2f;

    [Header("Поворот при испуге")]
    [SerializeField] private float _lookDuration = 0.4f;

    private DogCourage _dog;
    private GhostMovement _movement;

    private bool _isScaring;
    private float _lookEndTime = -1f;

    private void Awake()
    {
        _dog = FindFirstObjectByType<DogCourage>();

        _movement = GetComponent<GhostMovement>();

        if (_movement == null)
            _movement = GetComponentInParent<GhostMovement>();
    }

    private void OnEnable()
    {
        if (_dog != null)
            _dog.OnScared += OnSuccessfulScare;
    }

    private void Update()
    {
        if (_dog == null)
            return;

        // Закончили пугать - смотрим по маршруту
        if (_lookEndTime >= 0f &&
            Time.time >= _lookEndTime)
        {
            _lookEndTime = -1f;

            if (_movement != null)
                _movement.SetLookTarget(null);
        }

        Transform ghostTransform =
            _movement != null
            ? _movement.transform
            : transform;

        float distance = Vector3.Distance(
            ghostTransform.position,
            _dog.transform.position
        );

        bool canScare = distance <= _scareRadius;

        // Сквозь стены не пугаем
        if (_movement != null &&
            _movement.IsPassingThrough)
        {
            canScare = false;
        }

        if (canScare)
        {
            _isScaring = true;

            _dog.GetScared(ghostTransform.position);

            _isScaring = false;
        }
    }

    private void OnSuccessfulScare()
    {
        // Реагирует только призрак,
        // который действительно напугал собаку
        if (!_isScaring)
            return;

        Transform ghostTransform =
            _movement != null
            ? _movement.transform
            : transform;

        Vector3 direction =
            _dog.transform.position -
            ghostTransform.position;

        direction.y = 0f;

        // Мгновенно поворачиваемся к собаке
        if (direction.sqrMagnitude > 0.0001f)
        {
            ghostTransform.rotation =
                Quaternion.LookRotation(direction);
        }

        // Ненадолго удерживаем взгляд
        if (_movement != null)
            _movement.SetLookTarget(_dog.transform);

        _lookEndTime = Time.time + _lookDuration;
    }

    private void OnDisable()
    {
        if (_dog != null)
            _dog.OnScared -= OnSuccessfulScare;

        if (_movement != null)
            _movement.SetLookTarget(null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            _scareRadius
        );
    }
}
