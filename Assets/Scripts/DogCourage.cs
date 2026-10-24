
using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(50)]
public class DogCourage : MonoBehaviour
{
    [Header("Визуальная реакция")]
    [SerializeField] private Transform _dogModel;
    [SerializeField] private Transform _kostikModel;

    [Header("Испуг")]
    [SerializeField] private float _recoilDistance = 0.4f;
    [SerializeField] private float _recoilDuration = 0.5f;

    private const float ScareCooldown = 3f;

    private float _nextScareTime;
    private bool _canBeScared = true;

    private Vector3 _dogStartScale;
    private Vector3 _kostikStartScale;

    private Coroutine _scareCoroutine;

    public event Action OnScared;

    private void Start()
    {
        if (_dogModel != null)
            _dogStartScale = _dogModel.localScale;

        if (_kostikModel != null)
            _kostikStartScale = _kostikModel.localScale;
    }

    public void GetScared(Vector3 ghostPosition)
    {
        if (!_canBeScared || Time.time < _nextScareTime)
            return;

        _nextScareTime = Time.time + ScareCooldown;

        // Направление от призрака для собаки
        Vector3 dogDirection =
            transform.position - ghostPosition;

        dogDirection.y = 0f;

        if (dogDirection.sqrMagnitude < 0.001f)
            dogDirection = -transform.forward;

        dogDirection.Normalize();

        // Направление от призрака для Костика
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

        // Запускаем отпрыгивание
        if (_scareCoroutine != null)
            StopCoroutine(_scareCoroutine);

        _scareCoroutine = StartCoroutine(
            ScareAnimation(dogDirection, kostikDirection)
        );

        // Сообщаем контроллеру об испуге
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

            // Плавное отпрыгивание
            float moveProgress = Mathf.SmoothStep(
                0f, 1f, progress);

            float stepDistance =
                (moveProgress - previousProgress)
                * _recoilDistance;

            previousProgress = moveProgress;

            // Собака отпрыгивает
            if (_dogModel != null)
            {
                _dogModel.position +=
                    dogDirection * stepDistance;
            }

            // Костик отпрыгивает
            if (_kostikModel != null)
            {
                Vector3 movement =
                    kostikDirection * stepDistance;

                // Если Костик внутри модели собаки,
                // учитываем уже полученное смещение
                if (_dogModel != null &&
                    _kostikModel.IsChildOf(_dogModel))
                {
                    movement -= dogDirection * stepDistance;
                }

                _kostikModel.position += movement;
            }

            // Сжатие персонажей при испуге
            float squash = Mathf.Sin(progress * Mathf.PI);

            if (_dogModel != null)
            {
                _dogModel.localScale =
                    _dogStartScale * (1f - 0.15f * squash);
            }

            if (_kostikModel != null)
            {
                _kostikModel.localScale =
                    _kostikStartScale * (1f - 0.15f * squash);
            }

            yield return null;
        }

        // Возвращаем только исходный размер!
        // Позиции НЕ возвращаем.

        if (_dogModel != null)
            _dogModel.localScale = _dogStartScale;

        if (_kostikModel != null)
            _kostikModel.localScale = _kostikStartScale;

        _scareCoroutine = null;
    }
}
