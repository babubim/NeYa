
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatController : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] private DefeatView defeatView;

    [Header("Храбрость")]
    [SerializeField] private CourageController courageController;

    [Header("Поражение")]
    [SerializeField] private float defeatDelay = 0.6f;

    private bool defeatStarted;

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
        defeatView.Show();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void RestartGame()
    {
        // Сбрасываем состояние поражения
        defeatStarted = false;

        // Прячем экран поражения
        defeatView.Hide();

        // Возобновляем игру
        Time.timeScale = 1f;

        // Перезапускаем текущую сцену
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }


    private void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}
