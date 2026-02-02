using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DungeonRush.UI.HUD
{
    public class XPBarView : MonoBehaviour, IXPBarView
    {
        private const float FillAnimationDuration = 0.3f;
        private const float LevelUpPunchDuration = 0.3f;

        [Header("References")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _levelText;

        public bool IsVisible => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetFillAmount(float ratio)
        {
            _fillImage.DOFillAmount(ratio, FillAnimationDuration)
                .SetEase(Ease.OutCubic);
        }

        public void SetLevelText(string text)
        {
            _levelText.text = text;
        }

        public void PlayLevelUpAnimation()
        {
            _fillImage.fillAmount = 0f;

            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(
                    new Vector3(0f, 0.3f, 0f),
                    LevelUpPunchDuration)
                .SetEase(Ease.OutCubic);
        }
    }
}
