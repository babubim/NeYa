
public class EnemyCounterModel
{
    public int Total { get; private set; }
    public int Remaining { get; private set; }

    public int Killed => Total - Remaining;

    public EnemyCounterModel(int enemyCount)
    {
        Total = enemyCount;
        Remaining = enemyCount;
    }

    public void RemoveEnemy()
    {
        if (Remaining > 0)
            Remaining--;
    }
}
