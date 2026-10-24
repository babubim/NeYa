
using UnityEngine;
using TMPro;

public class EnemyCounterView : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;

    public void ShowCount(int killed, int total)
    {
        counterText.text = killed + "/" + total;
    }
}
