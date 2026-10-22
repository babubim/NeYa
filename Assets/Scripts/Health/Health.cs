
using UnityEngine;

public class Health : MonoBehaviour
{
    public enum EnemyType
    {
        SmallGhost,
        BigGhost
    }

    [SerializeField] private EnemyType _enemyType;

    private int _health;

    private void Awake()
    {
        switch (_enemyType)
        {
            case EnemyType.SmallGhost:
                _health = 1;
                break;

            case EnemyType.BigGhost:
                _health = 3;
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || _health <= 0) return;

        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamageSource source =
            collision.gameObject.GetComponent<DamageSource>();

        if (source != null)
        {
            TakeDamage(source.Damage);
        }
    }
}
