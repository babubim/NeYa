
using System;

public class MainMenuModel
{
    public bool IsBusy { get; private set; }

    public event Action<bool> StateChanged;

    public bool TryBeginAction()
    {
        if (IsBusy)
            return false;

        IsBusy = true;
        StateChanged?.Invoke(IsBusy);

        return true;
    }
}
