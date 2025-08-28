using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillBarView : MonoBehaviour
{
    public int Max
    {
        get => max;
        set
        {
            max = value;
            FillAmount = (float)current / max;
            ProgressString = $"{current} / {max}";
        }
    }

    public int Current
    {
        get => current;
        set
        {
            current = value;
            FillAmount = (float)current / max;
            ProgressString = $"{current} / {max}";
        }
    }

    public float FillAmount
    {
        get => fillAmount;
        set
        {
            fillAmount = value;
            fillImage.fillAmount = fillAmount;
        }
    }

    public string ProgressString
    {
        get => progressString;
        set
        {
            progressString = value;
            progressTmp.text = progressString;
        }
    }

    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text progressTmp;
    [SerializeField] private int max;
    [SerializeField] private int current;
    [SerializeField] private float fillAmount;
    [SerializeField] private string progressString;

    public void Initialize(int newCurrent, int newMax)
    {
        current = newCurrent;
        Max = newMax;
    }
}
