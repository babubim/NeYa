
using System;
using UnityEngine;

public class CourageController : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] private CourageView courageView;

    [Header("Игровая механика")]
    [SerializeField] private DogCourage dogCourage;

    private CourageModel courageModel;

    public event Action OnCourageDepleted;

    private void Awake()
    {
        courageModel = new CourageModel();
    }

    private void OnEnable()
    {
        if (dogCourage != null)
            dogCourage.OnScared += LoseCourage;
    }

    private void OnDisable()
    {
        if (dogCourage != null)
            dogCourage.OnScared -= LoseCourage;
    }

    private void Start()
    {
        UpdateView();
    }

    [ContextMenu("Test Lose Courage")]
    public void LoseCourage()
    {
        if (courageModel.CurrentCourage <= 0)
            return;

        courageModel.LoseCourage();
        UpdateView();

        Debug.Log("Храбрость: " +
            courageModel.CurrentCourage + "/3");

        if (courageModel.CurrentCourage == 0)
        {
            if (dogCourage != null)
                dogCourage.SetScareEnabled(false);

            OnCourageDepleted?.Invoke();
        }
    }

    [ContextMenu("Test Restore Courage")]
    public void RestoreCourage()
    {
        courageModel.RestoreCourage();
        UpdateView();

        if (dogCourage != null &&
            courageModel.CurrentCourage > 0)
        {
            dogCourage.SetScareEnabled(true);
        }
    }

    private void UpdateView()
    {
        courageView.ShowCourage(
            courageModel.CurrentCourage
        );
    }
}
