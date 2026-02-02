using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DungeonRush.UI.HUD
{
    public class HPBarView : MonoBehaviour, IHPBarView
    {
        private const float FillAnimationDuration = 0.3f;
        private const float DamageShakeStrength = 5f;
        private const float DamageShakeDuration = 0.2f;
        private const float HealPunchScale = 1.1f;
        private const float HealPunchDuration = 0.2f;

        [Header("References")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private Image _damageFillImage;
        [SerializeField] private TextMeshProUGUI _hpText;

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

            _damageFillImage.DOFillAmount(ratio, FillAnimationDuration * 2f)
                .SetEase(Ease.InCubic)
                .SetDelay(FillAnimationDuration * 0.5f);
        }

        public void SetText(string text)
        {
            _hpText.text = text;
        }

        public void PlayDamageAnimation()
        {
            transform.DOShakePosition(DamageShakeDuration, DamageShakeStrength)
                .SetEase(Ease.OutCubic);
        }

        public void PlayHealAnimation()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(
                    Vector3.one * (HealPunchScale - 1f),
                    HealPunchDuration)
                .SetEase(Ease.OutCubic);
        }
    }
}
