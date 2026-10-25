
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] private PauseMenuView _view;

    [Header("Музыка")]
    [SerializeField] private AudioMixer _gameMixer;
    [SerializeField] private AudioMixerSnapshot _gameplaySnapshot;
    [SerializeField] private AudioMixerSnapshot _pausedSnapshot;
    [SerializeField] private float _transitionTime = 0.5f;

    private PauseMenuModel _model;

    private CursorLockMode _previousCursorLockState;
    private bool _previousCursorVisibility;

    private void Awake()
    {
        // Создаём модель
        _model = new PauseMenuModel();

        // Связываем View с Model
        _view.Bind(_model);

        // Подписываемся на события View
        _view.ResumeClicked += ResumeGame;
        _view.NewGameClicked += NewGame;
        _view.MainMenuClicked += ReturnToMainMenu;

        // Подписываемся на состояние паузы
        _model.PauseStateChanged += OnPauseStateChanged;

        // Музыка должна переключаться даже на паузе
        if (_gameMixer != null)
        {
            _gameMixer.updateMode =
                AudioMixerUpdateMode.UnscaledTime;
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // Начальное состояние музыки
        if (_gameplaySnapshot != null)
            _gameplaySnapshot.TransitionTo(0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_model.IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (_model.IsPaused)
            return;

        // Запоминаем состояние курсора
        _previousCursorLockState = Cursor.lockState;
        _previousCursorVisibility = Cursor.visible;

        _model.SetPaused(true);
    }

    public void ResumeGame()
    {
        if (!_model.IsPaused)
            return;

        _model.SetPaused(false);
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        // Останавливаем или возобновляем игру
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            // Показываем курсор
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Приглушаем музыку
            if (_pausedSnapshot != null)
                _pausedSnapshot.TransitionTo(_transitionTime);
        }
        else
        {
            // Возвращаем управление мышью
            Cursor.lockState = _previousCursorLockState;
            Cursor.visible = _previousCursorVisibility;

            // Возвращаем обычное звучание
            if (_gameplaySnapshot != null)
                _gameplaySnapshot.TransitionTo(_transitionTime);
        }
    }

    public void NewGame()
    {
        Time.timeScale = 1f;

        // Сбрасываем музыкальный фильтр
        if (_gameplaySnapshot != null)
            _gameplaySnapshot.TransitionTo(0f);

        // Восстанавливаем курсор
        if (_model.IsPaused)
        {
            Cursor.lockState = _previousCursorLockState;
            Cursor.visible = _previousCursorVisibility;
        }

        // Загружаем новую игру
        SceneManager.LoadScene(1);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        // Сбрасываем приглушение музыки
        if (_gameplaySnapshot != null)
            _gameplaySnapshot.TransitionTo(0f);

        // Показываем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        if (_view != null)
        {
            _view.ResumeClicked -= ResumeGame;
            _view.NewGameClicked -= NewGame;
            _view.MainMenuClicked -= ReturnToMainMenu;
        }

        if (_model != null)
        {
            _model.PauseStateChanged -= OnPauseStateChanged;
        }
    }
}
