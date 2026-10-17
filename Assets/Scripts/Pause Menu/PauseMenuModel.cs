
using System;

public class PauseMenuModel
{
    public bool IsPaused { get; private set; }

    public event Action<bool> PauseStateChanged;

    public void SetPaused(bool isPaused)
    {
        if (IsPaused == isPaused)
            return;

        IsPaused = isPaused;

        PauseStateChanged?.Invoke(IsPaused);
    }
}
