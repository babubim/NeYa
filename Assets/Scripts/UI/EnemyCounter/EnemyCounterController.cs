
using System;
using UnityEngine;

public class EnemyCounterController : MonoBehaviour
{
    [SerializeField] private EnemyCounterView counterView;

    private EnemyCounterModel counterModel;
    private Health[] enemies;

    public event Action OnAllEnemiesDefeated;

    private void Start()
    {
        enemies = FindObjectsByType<Health>(
            FindObjectsSortMode.None
        );

        counterModel = new EnemyCounterModel(
            enemies.Length
        );

        foreach (Health enemy in enemies)
        {
            enemy.Died += OnEnemyDied;
        }

        UpdateView();
        CheckVictory();
    }

    private void OnEnemyDied(Health enemy)
    {
        enemy.Died -= OnEnemyDied;

        counterModel.RemoveEnemy();

        UpdateView();
        CheckVictory();
    }

    private void UpdateView()
    {
        counterView.ShowCount(
            counterModel.Killed,
            counterModel.Total
        );
    }

    private void CheckVictory()
    {
        if (counterModel.Remaining == 0)
        {
            OnAllEnemiesDefeated?.Invoke();
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
}
