
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class DefeatController : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] private DefeatView defeatView;

    [Header("Храбрость")]
    [SerializeField] private CourageController courageController;

    [Header("Поражение")]
    [SerializeField] private float defeatDelay = 1.2f;

    [Header("Контроллер паузы")]
    [SerializeField] private PauseMenuController pauseMenuController;

    [Header("Аудиомикшер")]
    [SerializeField] private AudioMixer gameMixer;
    [SerializeField] private AudioMixerSnapshot gameplaySnapshot;
    [SerializeField] private AudioMixerSnapshot endSnapshot;

    [Header("Звук поражения")]
    [SerializeField] private AudioSource defeatSound;
    [SerializeField] private float audioTransitionTime = 0.5f;

    private bool defeatStarted;
    private bool defeatShown;

    public bool IsDefeatStarted => defeatStarted;

    private void Awake()
    {
        if (gameMixer != null)
        {
            gameMixer.updateMode =
                AudioMixerUpdateMode.UnscaledTime;
        }
    }

    private void OnEnable()
    {
        if (defeatView != null)
        {
            defeatView.OnNewGameClicked += RestartGame;
            defeatView.OnMainMenuClicked += MainMenu;
        }

        if (courageController != null)
        {
            courageController.OnCourageDepleted += StartDefeat;
        }
    }

    private void OnDisable()
    {
        if (defeatView != null)
        {
            defeatView.OnNewGameClicked -= RestartGame;
            defeatView.OnMainMenuClicked -= MainMenu;
        }

        if (courageController != null)
        {
            courageController.OnCourageDepleted -= StartDefeat;
        }
    }

    private void StartDefeat()
    {
        if (defeatStarted)
            return;

        defeatStarted = true;

        // Запрещаем открывать обычную паузу
        if (pauseMenuController != null)
            pauseMenuController.enabled = false;

        // Пока НЕ останавливаем время и звук!
        // Даём героям отыграть испуг,
        // а призраку закончить крик.
        StartCoroutine(DefeatDelay());
    }

    private IEnumerator DefeatDelay()
    {
        yield return new WaitForSeconds(defeatDelay);

        ShowDefeat();
    }

    [ContextMenu("Test Defeat")]
    public void ShowDefeat()
    {
        if (defeatShown)
            return;

        defeatStarted = true;
        defeatShown = true;

        if (pauseMenuController != null)
            pauseMenuController.enabled = false;

        // Теперь приглушаем музыку
        // и отключаем игровые звуки
        PlayDefeatAudio();

        // Показываем экран поражения
        if (defeatView != null)
            defeatView.Show();

        // Останавливаем игру
        Time.timeScale = 0f;

        // Освобождаем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayDefeatAudio()
    {
        if (endSnapshot != null)
        {
            endSnapshot.TransitionTo(
                audioTransitionTime
            );
        }

        if (defeatSound != null)
        {
            defeatSound.Play();
        }
    }

    private void RestartGame()
    {
        defeatStarted = false;
        defeatShown = false;

        if (defeatView != null)
            defeatView.Hide();

        ResetAudio();

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void MainMenu()
    {
        ResetAudio();

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(0);
    }

    private void ResetAudio()
    {
        if (defeatSound != null)
            defeatSound.Stop();

        if (gameplaySnapshot != null)
            gameplaySnapshot.TransitionTo(0f);
    }
}
