
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MainMenuView _view;

    private MainMenuModel _model;

    private void Awake()
    {
        // Создаём модель
        _model = new MainMenuModel();

        // Связываем View с Model
        _view.Bind(_model);

        // Подписываемся на события кнопок
        _view.PlayClicked += PlayGame;
        _view.QuitClicked += QuitGame;
    }

    public void PlayGame()
    {
        // Проверяем, можно ли начать действие
        if (!_model.TryBeginAction())
            return;

        // Загружаем игровую сцену
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        // Проверяем, можно ли начать действие
        if (!_model.TryBeginAction())
            return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        if (_view != null)
        {
            _view.PlayClicked -= PlayGame;
            _view.QuitClicked -= QuitGame;
        }
    }
}
