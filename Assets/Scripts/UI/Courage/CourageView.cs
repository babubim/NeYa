
using UnityEngine;
using UnityEngine.UI;

public class CourageView : MonoBehaviour
{
    [SerializeField] private Image courageBar;
    [SerializeField] private Image courageFace;

    [SerializeField] private Sprite[] barSprites = new Sprite[4];
    [SerializeField] private Sprite[] faceSprites = new Sprite[4];

    public void ShowCourage(int courage)
    {
        courageBar.sprite = barSprites[courage];
        courageFace.sprite = faceSprites[courage];
    }
}
