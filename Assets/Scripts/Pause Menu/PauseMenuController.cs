
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private PauseMenuView _view;

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

        // Подписываемся на изменение состояния паузы
        _model.PauseStateChanged += OnPauseStateChanged;
    }

    private void Start()
    {
        Time.timeScale = 1f;
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

        // Изменяем состояние модели
        _model.SetPaused(true);
    }

    public void ResumeGame()
    {
        if (!_model.IsPaused)
            return;

        // Изменяем состояние модели
        _model.SetPaused(false);
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        // Управляем игровым временем
        Time.timeScale = isPaused ? 0f : 1f;

        // Управляем курсором
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = _previousCursorLockState;
            Cursor.visible = _previousCursorVisibility;
        }
    }


    public void NewGame()
    {
        // Восстанавливаем игровое время
        Time.timeScale = 1f;

        // Восстанавливаем состояние курсора
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
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
