using UnityEngine;
using TMPro;
using DG.Tweening;

namespace DungeonRush.UI.HUD
{
    public class GoldDisplayView : MonoBehaviour, IGoldDisplayView
    {
        private const float GainPunchScale = 1.2f;
        private const float GainPunchDuration = 0.2f;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _goldText;

        public bool IsVisible => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetGoldText(string text)
        {
            _goldText.text = text;
        }

        public void PlayGainAnimation()
        {
            transform.DOPunchScale(
                    Vector3.one * (GainPunchScale - 1f),
                    GainPunchDuration)
                .SetEase(Ease.OutCubic);
        }
    }
}
