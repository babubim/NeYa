
using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
    [Header("ѕанель паузы")]
    [SerializeField] private GameObject _pausePanel;

    [Header(" нопки")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _mainMenuButton;

    private PauseMenuModel _model;

    public event Action ResumeClicked;
    public event Action NewGameClicked;
    public event Action MainMenuClicked;

    private void Awake()
    {
        _resumeButton.onClick.AddListener(OnResumeClicked);
        _newGameButton.onClick.AddListener(OnNewGameClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    public void Bind(PauseMenuModel model)
    {
        if (_model != null)
            _model.PauseStateChanged -= OnPauseStateChanged;

        _model = model;

        _model.PauseStateChanged += OnPauseStateChanged;

        OnPauseStateChanged(_model.IsPaused);
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        _pausePanel.SetActive(isPaused);
    }

    private void OnResumeClicked()
    {
        ResumeClicked?.Invoke();
    }

    private void OnNewGameClicked()
    {
        NewGameClicked?.Invoke();
    }

    private void OnMainMenuClicked()
    {
        MainMenuClicked?.Invoke();
    }

    private void OnDestroy()
    {
        if (_model != null)
            _model.PauseStateChanged -= OnPauseStateChanged;

        _resumeButton.onClick.RemoveListener(OnResumeClicked);
        _newGameButton.onClick.RemoveListener(OnNewGameClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }
}
