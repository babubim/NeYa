
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class AudioListenerFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private void Start()
    {
        if (_camera == null && Camera.main != null)
        {
            _camera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (_camera == null) return;

        // Поворачиваем слушателя вместе с камерой
        transform.rotation = _camera.rotation;
    }
}
