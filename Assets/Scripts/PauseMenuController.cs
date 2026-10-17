
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;

    private bool _isPaused;

    private CursorLockMode _previousCursorLockState;
    private bool _previousCursorVisibility;

    private void Start()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        _pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (_isPaused) return;

        _isPaused = true;

        // Запоминаем состояние курсора
        _previousCursorLockState = Cursor.lockState;
        _previousCursorVisibility = Cursor.visible;

        // Останавливаем игровое время
        Time.timeScale = 0f;

        // Показываем меню
        _pausePanel.SetActive(true);

        // Освобождаем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        _isPaused = false;

        // Скрываем меню
        _pausePanel.SetActive(false);

        // Возобновляем игровое время
        Time.timeScale = 1f;

        // Восстанавливаем состояние курсора
        Cursor.lockState = _previousCursorLockState;
        Cursor.visible = _previousCursorVisibility;
    }

    public void NewGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void ReturnToMainMenu()
    {
        // Возвращаем нормальную скорость времени
        Time.timeScale = 1f;

        // Освобождаем и показываем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Загружаем главное меню
        SceneManager.LoadScene(0);
    }
}
