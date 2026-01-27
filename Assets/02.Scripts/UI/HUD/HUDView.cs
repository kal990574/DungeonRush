using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDView : UIView, IHUDView
{
    [Header("HP")]
    [SerializeField] private Image _hpBarFill;
    [SerializeField] private TextMeshProUGUI _hpText;

    [Header("XP")]
    [SerializeField] private Image _xpBarFill;
    [SerializeField] private TextMeshProUGUI _xpText;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI _goldText;

    [Header("Stage")]
    [SerializeField] private TextMeshProUGUI _stageText;

    public void UpdateHPBar(float fillRatio)
    {
        _hpBarFill.fillAmount = fillRatio;
    }

    public void UpdateHPText(float current, float max)
    {
        _hpText.text = $"{current:F0}/{max:F0}";
    }

    public void UpdateXPBar(float fillRatio)
    {
        _xpBarFill.fillAmount = fillRatio;
    }

    public void UpdateXPText(int current, int required)
    {
        _xpText.text = $"{current}/{required}";
    }

    public void UpdateGold(int amount)
    {
        _goldText.text = $"{amount}G";
    }

    public void UpdateStage(int chapter, int wave, bool isBoss)
    {
        if (isBoss)
        {
            _stageText.text = $"Chapter {chapter}>";
        }
        else
        {
            _stageText.text = $"{chapter}-{wave}";
        }
    }
}
