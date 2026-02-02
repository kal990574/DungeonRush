using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DungeonRush.UI.HUD
{
    public class SkillButtonView : MonoBehaviour, ISkillButtonView
    {
        private const float ReadyPunchScale = 1.15f;
        private const float ReadyPunchDuration = 0.2f;

        [Header("References")]
        [SerializeField] private Image _borderLight;
        [SerializeField] private Slider _cooldownSlider;
        [SerializeField] private Image _iconImage;
        [SerializeField] private GameObject _lockOverlay;
        [SerializeField] private Button _button;

        public bool IsVisible => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
        }

        public void SetCooldown(float ratio)
        {
            _cooldownSlider.value = ratio;
            _button.interactable = ratio <= 0f;
        }

        public void ShowLockedState()
        {
            _lockOverlay.SetActive(true);
            _borderLight.enabled = false;
            _button.interactable = false;
            _iconImage.enabled = false;
            _cooldownSlider.value = 0f;
        }

        public void ShowReadyState()
        {
            _lockOverlay.SetActive(false);
            _borderLight.enabled = true;
            _button.interactable = true;

            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(
                    Vector3.one * (ReadyPunchScale - 1f),
                    ReadyPunchDuration)
                .SetEase(Ease.OutCubic);
        }
    }
}
