
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public enum EnemyType
    {
        SmallGhost,
        BigGhost
    }

    [Header("Тип врага")]
    [SerializeField] private EnemyType _enemyType;

    [Header("Эффект получения урона")]
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _flashDuration = 0.2f;

    [Header("Звук получения урона")]
    [SerializeField] private AudioSource _hurtAudio;

    private int _health;
    private GhostAudioPool _audioPool;

    private readonly List<Material> _materials = new();
    private readonly List<Color> _originalColors = new();
    private readonly List<int> _colorProperties = new();

    private Coroutine _flashCoroutine;

    public event Action<Health> Died;

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

        _audioPool = FindFirstObjectByType<GhostAudioPool>();

        SaveMaterials();
    }

    private void SaveMaterials()
    {
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material == null)
                    continue;

                int colorProperty;

                if (material.HasProperty("_BaseColor"))
                    colorProperty =
                        Shader.PropertyToID("_BaseColor");
                else if (material.HasProperty("_Color"))
                    colorProperty =
                        Shader.PropertyToID("_Color");
                else
                    continue;

                _materials.Add(material);
                _originalColors.Add(
                    material.GetColor(colorProperty));

                _colorProperties.Add(colorProperty);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || _health <= 0)
            return;

        _health -= damage;

        bool isDead = _health <= 0;

        // Звук запускаем СРАЗУ при попадании
        if (isDead)
        {
            if (_audioPool != null)
            {
                _audioPool.PlayDeathSound(
                    _enemyType,
                    transform.position
                );
            }
        }
        else
        {
            if (_hurtAudio != null)
                _hurtAudio.Play();
        }

        // Красная вспышка
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(
            DamageFlash(isDead)
        );
    }

    private IEnumerator DamageFlash(bool isDead)
    {
        for (int i = 0; i < _materials.Count; i++)
        {
            Color color = _damageColor;
            color.a = _originalColors[i].a;

            _materials[i].SetColor(
                _colorProperties[i], color);
        }

        yield return new WaitForSeconds(_flashDuration);

        if (isDead)
        {
            Die();
            yield break;
        }

        RestoreColors();
        _flashCoroutine = null;
    }

    private void RestoreColors()
    {
        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetColor(
                _colorProperties[i],
                _originalColors[i]);
        }
    }

    private void Die()
    {
        // Звук уже играет из AudioPool
        Died?.Invoke(this);

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        RestoreColors();
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
