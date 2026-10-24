
using System;
using UnityEngine;
using UnityEngine.UI;

public class DefeatView : MonoBehaviour
{
    [Header("Панель поражения")]
    [SerializeField] private GameObject defeatPanel;

    [Header("Кнопки")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button mainMenuButton;

    public event Action OnNewGameClicked;
    public event Action OnMainMenuClicked;

    private void Awake()
    {
        newGameButton.onClick.AddListener(NewGameClicked);
        mainMenuButton.onClick.AddListener(MainMenuClicked);

        Hide();
    }

    private void OnDestroy()
    {
        newGameButton.onClick.RemoveListener(NewGameClicked);
        mainMenuButton.onClick.RemoveListener(MainMenuClicked);
    }

    private void NewGameClicked()
    {
        OnNewGameClicked?.Invoke();
    }

    private void MainMenuClicked()
    {
        OnMainMenuClicked?.Invoke();
    }

    public void Show()
    {
        defeatPanel.SetActive(true);
    }

    public void Hide()
    {
        defeatPanel.SetActive(false);
    }
}
