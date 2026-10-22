
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

    private const int MaxCourage = 3;
    private const float ScareCooldown = 3f;

    private int _currentCourage = MaxCourage;
    private float _nextScareTime;

    private Vector3 _dogStartPosition;
    private Vector3 _dogStartScale;
    private Vector3 _kostikStartScale;

    private Vector3 _dogAwayDirection;
    private Vector3 _kostikAwayDirection;

    private float _recoilAmount;

    private void Start()
    {
        if (_dogModel != null)
        {
            _dogStartPosition = _dogModel.localPosition;
            _dogStartScale = _dogModel.localScale;
        }

        if (_kostikModel != null)
            _kostikStartScale = _kostikModel.localScale;
    }

    public void GetScared(Vector3 ghostPosition)
    {
        if (_currentCourage <= 0 ||
            Time.time < _nextScareTime)
            return;

        _currentCourage--;
        _nextScareTime = Time.time + ScareCooldown;

        // Направления от призрака
        _dogAwayDirection = transform.position - ghostPosition;
        _dogAwayDirection.y = 0f;

        if (_dogAwayDirection.sqrMagnitude < 0.001f)
            _dogAwayDirection = -transform.forward;

        _dogAwayDirection.Normalize();

        if (_kostikModel != null)
        {
            _kostikAwayDirection =
                _kostikModel.position - ghostPosition;

            _kostikAwayDirection.y = 0f;

            if (_kostikAwayDirection.sqrMagnitude < 0.001f)
                _kostikAwayDirection = _dogAwayDirection;

            _kostikAwayDirection.Normalize();
        }

        Debug.Log("Собака и Костик испугались! Смелость: "
                  + _currentCourage + "/" + MaxCourage);

        StartCoroutine(ScareAnimation());

        if (_currentCourage <= 0)
        {
            Debug.Log("ПРОИГРЫШ! Собака и Костик напуганы!");
        }
    }

    private IEnumerator ScareAnimation()
    {
        float timer = 0f;
        float duration = Mathf.Max(0.01f, _recoilDuration);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / duration);

            // Резко отскакиваем и плавно возвращаемся
            if (progress < 0.25f)
            {
                _recoilAmount = Mathf.SmoothStep(
                    0f, 1f, progress / 0.25f);
            }
            else
            {
                _recoilAmount = Mathf.SmoothStep(
                    1f, 0f, (progress - 0.25f) / 0.75f);
            }

            yield return null;
        }

        _recoilAmount = 0f;
    }

    private void LateUpdate()
    {
        // Откидывание собаки
        if (_dogModel != null)
        {
            Vector3 direction =
                _dogModel.parent.InverseTransformDirection(
                    _dogAwayDirection);

            _dogModel.localPosition =
                _dogStartPosition +
                direction * _recoilDistance * _recoilAmount;

            _dogModel.localScale =
                _dogStartScale * (1f - 0.15f * _recoilAmount);
        }

        // Откидывание Костика
        if (_kostikModel != null)
        {
            Vector3 direction =
                _kostikModel.parent.InverseTransformDirection(
                    _kostikAwayDirection);

            _kostikModel.localPosition +=
                direction * _recoilDistance * _recoilAmount;

            _kostikModel.localScale =
                _kostikStartScale * (1f - 0.15f * _recoilAmount);
        }
    }
}
