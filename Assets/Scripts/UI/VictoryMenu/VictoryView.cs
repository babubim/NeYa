
using System;
using UnityEngine;
using UnityEngine.UI;

public class VictoryView : MonoBehaviour
{
    [Header("Ёкран победы")]
    [SerializeField] private GameObject victoryPanel;

    [Header(" нопки")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button mainMenuButton;

    public event Action OnNewGameClicked;
    public event Action OnMainMenuClicked;

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        if (newGameButton != null)
            newGameButton.onClick.AddListener(NewGameClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(MainMenuClicked);
    }

    private void OnDisable()
    {
        if (newGameButton != null)
            newGameButton.onClick.RemoveListener(NewGameClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(MainMenuClicked);
    }

    public void Show()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void Hide()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    private void NewGameClicked()
    {
        OnNewGameClicked?.Invoke();
    }

    private void MainMenuClicked()
    {
        OnMainMenuClicked?.Invoke();
    }
}
