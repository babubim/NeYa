
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    private MainMenuModel _model;

    public event Action PlayClicked;
    public event Action QuitClicked;

    private void Awake()
    {
        _playButton.onClick.AddListener(OnPlayClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void Bind(MainMenuModel model)
    {
        _model = model;

        _model.StateChanged += OnStateChanged;

        OnStateChanged(_model.IsBusy);
    }

    private void OnStateChanged(bool isBusy)
    {
        _playButton.interactable = !isBusy;
        _quitButton.interactable = !isBusy;
    }

    private void OnPlayClicked()
    {
        PlayClicked?.Invoke();
    }

    private void OnQuitClicked()
    {
        QuitClicked?.Invoke();
    }

    private void OnDestroy()
    {
        if (_model != null)
            _model.StateChanged -= OnStateChanged;

        _playButton.onClick.RemoveListener(OnPlayClicked);
        _quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
