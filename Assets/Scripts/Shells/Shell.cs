
using UnityEngine;

public class Shell : MonoBehaviour
{
    private bool _hasCollided;

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCollided) return;

        _hasCollided = true;

        Destroy(gameObject, 0.03f);
    }
}
