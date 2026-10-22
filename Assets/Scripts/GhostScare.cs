
using UnityEngine;

public class GhostScare : MonoBehaviour
{
    [Header("Запугивание")]
    [SerializeField] private float _scareRadius = 2f;

    private DogCourage _dog;
    private GhostMovement _movement;

    private void Start()
    {
        _dog = FindFirstObjectByType<DogCourage>();
        _movement = GetComponent<GhostMovement>();
    }

    private void Update()
    {
        if (_dog == null) return;

        // Проверяем расстояние до собаки
        float distance = Vector3.Distance(
            transform.position,
            _dog.transform.position
        );

        bool canScare = distance <= _scareRadius;

        // Во время прохождения стены не пугаем
        if (_movement != null && _movement.IsPassingThrough)
        {
            canScare = false;
        }

        // Поворачиваемся к собаке
        if (_movement != null)
        {
            if (canScare)
            {
                _movement.SetLookTarget(_dog.transform);
            }
            else
            {
                _movement.SetLookTarget(null);
            }
        }

        // Пугаем собаку и Костика
        if (canScare)
        {
            _dog.GetScared(transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _scareRadius);
    }
}
