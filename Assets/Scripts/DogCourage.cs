
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(50)]
public class DogCourage : MonoBehaviour
{
    [Header("Персонажи")]
    [SerializeField] private Transform _dogModel;
    [SerializeField] private Transform _kostikModel;

    [Header("Отпрыгивание")]
    [SerializeField] private float _recoilDistance = 1.2f;
    [SerializeField] private float _recoilDuration = 0.25f;

    [Header("Красное мигание")]
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private int _flashCount = 3;
    [SerializeField] private float _flashInterval = 0.07f;

    private const float ScareCooldown = 3f;

    private float _nextScareTime;
    private bool _canBeScared = true;

    private Vector3 _dogStartScale;
    private Vector3 _kostikStartScale;

    private Coroutine _scareCoroutine;
    private Coroutine _flashCoroutine;

    private readonly List<Material> _materials = new();
    private readonly List<Color> _originalColors = new();
    private readonly List<int> _colorProperties = new();

    public event Action OnScared;

    private void Start()
    {
        if (_dogModel != null)
            _dogStartScale = _dogModel.localScale;

        if (_kostikModel != null)
            _kostikStartScale = _kostikModel.localScale;

        SaveMaterials(_dogModel);
        SaveMaterials(_kostikModel);
    }

    private void SaveMaterials(Transform model)
    {
        if (model == null) return;

        Renderer[] renderers =
            model.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material == null ||
                    _materials.Contains(material))
                    continue;

                int colorProperty;

                if (material.HasProperty("_BaseColor"))
                    colorProperty = Shader.PropertyToID("_BaseColor");
                else if (material.HasProperty("_Color"))
                    colorProperty = Shader.PropertyToID("_Color");
                else
                    continue;

                _materials.Add(material);
                _originalColors.Add(
                    material.GetColor(colorProperty));

                _colorProperties.Add(colorProperty);
            }
        }
    }

    public void GetScared(Vector3 ghostPosition)
    {
        if (!_canBeScared || Time.time < _nextScareTime)
            return;

        _nextScareTime = Time.time + ScareCooldown;

        // Направление отпрыгивания собаки
        Vector3 dogDirection =
            transform.position - ghostPosition;

        dogDirection.y = 0f;

        if (dogDirection.sqrMagnitude < 0.001f)
            dogDirection = -transform.forward;

        dogDirection.Normalize();

        // Направление отпрыгивания Костика
        Vector3 kostikDirection = dogDirection;

        if (_kostikModel != null)
        {
            kostikDirection =
                _kostikModel.position - ghostPosition;

            kostikDirection.y = 0f;

            if (kostikDirection.sqrMagnitude < 0.001f)
                kostikDirection = dogDirection;

            kostikDirection.Normalize();
        }

        // Отпрыгивание
        if (_scareCoroutine != null)
            StopCoroutine(_scareCoroutine);

        _scareCoroutine = StartCoroutine(
            ScareAnimation(dogDirection, kostikDirection)
        );

        // Красное мигание
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(FlashAnimation());

        // Сообщаем о потере храбрости
        OnScared?.Invoke();
    }

    public void SetScareEnabled(bool enabled)
    {
        _canBeScared = enabled;
    }

    private IEnumerator ScareAnimation(
        Vector3 dogDirection,
        Vector3 kostikDirection)
    {
        float timer = 0f;
        float duration = Mathf.Max(0.01f, _recoilDuration);
        float previousProgress = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / duration);

            // Резкий старт и плавное замедление
            float moveProgress =
                1f - Mathf.Pow(1f - progress, 3f);

            float stepDistance =
                (moveProgress - previousProgress)
                * _recoilDistance;

            previousProgress = moveProgress;

            // Отпрыгивание собаки
            if (_dogModel != null)
            {
                _dogModel.position +=
                    dogDirection * stepDistance;
            }

            // Отпрыгивание Костика
            if (_kostikModel != null)
            {
                Vector3 movement =
                    kostikDirection * stepDistance;

                if (_dogModel != null &&
                    _kostikModel.IsChildOf(_dogModel))
                {
                    movement -= dogDirection * stepDistance;
                }

                _kostikModel.position += movement;
            }

            // Сжатие во время испуга
            float squash = Mathf.Sin(progress * Mathf.PI);

            if (_dogModel != null)
            {
                _dogModel.localScale =
                    _dogStartScale * (1f - 0.2f * squash);
            }

            if (_kostikModel != null)
            {
                _kostikModel.localScale =
                    _kostikStartScale * (1f - 0.2f * squash);
            }

            yield return null;
        }

        // Возвращаем размер, но НЕ позицию
        if (_dogModel != null)
            _dogModel.localScale = _dogStartScale;

        if (_kostikModel != null)
            _kostikModel.localScale = _kostikStartScale;

        _scareCoroutine = null;
    }

    private IEnumerator FlashAnimation()
    {
        for (int i = 0; i < _flashCount; i++)
        {
            SetFlashColor(true);

            yield return new WaitForSeconds(_flashInterval);

            SetFlashColor(false);

            yield return new WaitForSeconds(_flashInterval);
        }

        SetFlashColor(false);
        _flashCoroutine = null;
    }

    private void SetFlashColor(bool isFlashing)
    {
        for (int i = 0; i < _materials.Count; i++)
        {
            Color color = isFlashing
                ? _flashColor
                : _originalColors[i];

            _materials[i].SetColor(
                _colorProperties[i], color);
        }
    }

    private void OnDisable()
    {
        SetFlashColor(false);

        if (_dogModel != null)
            _dogModel.localScale = _dogStartScale;

        if (_kostikModel != null)
            _kostikModel.localScale = _kostikStartScale;
    }
}
