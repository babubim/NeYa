
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class KostikFollow : MonoBehaviour
{
    [Header("Следование")]
    [SerializeField] private Transform _dog;
    [SerializeField] private Vector3 _offset = new Vector3(0.5f, 0f, 0f);

    [Header("Парение")]
    [SerializeField] private Transform _kostik;
    [SerializeField] private float _floatHeight = 0.05f;
    [SerializeField] private float _floatSpeed = 2f;

    [Header("Прицеливание")]
    [SerializeField] private float _aimDistance = 100f;

    private NavMeshAgent _agent;
    private Camera _camera;
    private Vector3 _startPosition;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;

        // Агент не должен вращать Костика
        _agent.updateRotation = false;

        _startPosition = _kostik.localPosition;
    }

    private void Update()
    {
        if (_dog == null || !_agent.isOnNavMesh) return;

        // Желаемая позиция рядом с собакой
        Vector3 targetPosition =
            _dog.position + _dog.rotation * _offset;

        // Ищем ближайшую доступную точку на NavMesh
        if (NavMesh.SamplePosition(
            targetPosition,
            out NavMeshHit hit,
            1f,
            NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    private void LateUpdate()
    {
        if (_kostik == null || _camera == null) return;

        // Парение Костика
        float offset =
            Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;

        _kostik.localPosition =
            _startPosition + Vector3.up * offset;

        // Находим точку прицеливания
        Ray ray = _camera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 aimPoint = ray.GetPoint(_aimDistance);

        if (Physics.Raycast(ray, out RaycastHit hit, _aimDistance))
        {
            aimPoint = hit.point;
        }

        // Поворачиваем Костика к прицелу
        Vector3 direction = aimPoint - _kostik.position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            _kostik.rotation = Quaternion.LookRotation(direction);
        }
    }
}
