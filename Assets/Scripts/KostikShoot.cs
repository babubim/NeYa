
using UnityEngine;

[DefaultExecutionOrder(100)]
public class KostikShoot : MonoBehaviour
{
    [SerializeField] private Rigidbody _shell;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _reloadTime = 2f;

    private Camera _camera;
    private float _nextShotTime;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= _nextShotTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (_camera == null || _shell == null) return;

        // Определяем точку под прицелом
        Ray ray = _camera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 target = ray.GetPoint(100f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            target = hit.point;
        }

        // Создаём Shell прямо в позиции ShootPoint
        Rigidbody shell = Instantiate(
            _shell,
            transform.position,
            transform.rotation
        );

        // Направляем Shell к прицелу
        Vector3 direction = (target - transform.position).normalized;

        shell.linearVelocity = direction * _speed;

        Destroy(shell.gameObject, 5f);

        _nextShotTime = Time.time + _reloadTime;

        Debug.Log("Выстрел из: " + transform.position);
    }
}
