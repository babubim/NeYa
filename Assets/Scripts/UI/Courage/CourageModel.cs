
public class CourageModel
{
    public int CurrentCourage { get; private set; } = 3;

    public void LoseCourage()
    {
        if (CurrentCourage > 0)
        {
            CurrentCourage--;
        }
    }

    public void RestoreCourage()
    {
        if (CurrentCourage < 3)
        {
            CurrentCourage++;
        }
    }
}
