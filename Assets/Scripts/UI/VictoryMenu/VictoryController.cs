
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class VictoryController : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] private VictoryView victoryView;

    [Header("Победа")]
    [SerializeField] private float victoryDelay = 1.2f;

    [Header("Другие контроллеры")]
    [SerializeField] private DefeatController defeatController;
    [SerializeField] private PauseMenuController pauseMenuController;

    [Header("Аудиомикшер")]
    [SerializeField] private AudioMixer gameMixer;
    [SerializeField] private AudioMixerSnapshot gameplaySnapshot;
    [SerializeField] private AudioMixerSnapshot endSnapshot;

    [Header("Звук победы")]
    [SerializeField] private AudioSource victorySound;
    [SerializeField] private float audioTransitionTime = 0.5f;

    private Health[] enemies;
    private int enemiesRemaining;

    private bool victoryStarted;
    private bool victoryShown;

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
        if (victoryView != null)
        {
            victoryView.OnNewGameClicked += RestartGame;
            victoryView.OnMainMenuClicked += MainMenu;
        }
    }

    private void Start()
    {
        // Находим всех призраков
        enemies = FindObjectsByType<Health>(
            FindObjectsSortMode.None
        );

        enemiesRemaining = enemies.Length;

        // Следим за их смертью
        foreach (Health enemy in enemies)
        {
            enemy.Died += OnEnemyDied;
        }
    }

    private void OnDisable()
    {
        if (victoryView != null)
        {
            victoryView.OnNewGameClicked -= RestartGame;
            victoryView.OnMainMenuClicked -= MainMenu;
        }
    }

    private void OnDestroy()
    {
        if (enemies == null)
            return;

        foreach (Health enemy in enemies)
        {
            if (enemy != null)
                enemy.Died -= OnEnemyDied;
        }
    }

    private void OnEnemyDied(Health enemy)
    {
        if (victoryStarted)
            return;

        enemiesRemaining--;

        if (enemiesRemaining > 0)
            return;

        // Если уже проиграли, победу не запускаем
        if (defeatController != null &&
            defeatController.IsDefeatStarted)
            return;

        StartVictory();
    }

    private void StartVictory()
    {
        if (victoryStarted)
            return;

        victoryStarted = true;

        // Запрещаем открывать паузу
        if (pauseMenuController != null)
            pauseMenuController.enabled = false;

        // Даём последнему призраку докричать
        StartCoroutine(VictoryDelay());
    }

    private IEnumerator VictoryDelay()
    {
        yield return new WaitForSeconds(victoryDelay);

        ShowVictory();
    }

    [ContextMenu("Test Victory")]
    public void ShowVictory()
    {
        if (victoryShown)
            return;

        if (defeatController != null &&
            defeatController.IsDefeatStarted)
            return;

        victoryStarted = true;
        victoryShown = true;

        if (pauseMenuController != null)
            pauseMenuController.enabled = false;

        PlayVictoryAudio();

        if (victoryView != null)
            victoryView.Show();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayVictoryAudio()
    {
        // Приглушаем музыку и выключаем игровые звуки
        if (endSnapshot != null)
        {
            endSnapshot.TransitionTo(
                audioTransitionTime
            );
        }

        // Один раз проигрываем звук победы
        if (victorySound != null)
        {
            victorySound.Play();
        }
    }

    private void RestartGame()
    {
        victoryStarted = false;
        victoryShown = false;

        if (victoryView != null)
            victoryView.Hide();

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
        if (victorySound != null)
            victorySound.Stop();

        if (gameplaySnapshot != null)
            gameplaySnapshot.TransitionTo(0f);
    }
}
