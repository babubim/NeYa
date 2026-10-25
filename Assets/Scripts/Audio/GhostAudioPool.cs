
using UnityEngine;

public class GhostAudioPool : MonoBehaviour
{
    [Header("Маленькие призраки")]
    [SerializeField] private AudioSource[] _smallGhostDeathSources;

    [Header("Большие призраки")]
    [SerializeField] private AudioSource[] _bigGhostDeathSources;

    public void PlayDeathSound(
        Health.EnemyType enemyType,
        Vector3 position)
    {
        AudioSource[] sources;

        // Выбираем нужный набор звуков
        if (enemyType == Health.EnemyType.SmallGhost)
        {
            sources = _smallGhostDeathSources;
        }
        else
        {
            sources = _bigGhostDeathSources;
        }

        // Ищем свободный AudioSource
        foreach (AudioSource source in sources)
        {
            if (source == null)
                continue;

            if (!source.isPlaying)
            {
                // Переносим звук к призраку
                source.transform.position = position;

                // Воспроизводим звук смерти
                source.Play();

                return;
            }
        }

        Debug.LogWarning(
            "В AudioPool нет свободных источников!"
        );
    }
}
