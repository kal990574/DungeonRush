using UnityEngine;
using TMPro;
using DG.Tweening;

namespace DungeonRush.UI.HUD
{
    public class StageDisplayView : MonoBehaviour, IStageDisplayView
    {
        private const float TransitionDuration = 0.3f;
        private const float BossPunchScale = 1.3f;
        private const float BossPunchDuration = 0.4f;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _stageText;
        [SerializeField] private GameObject _bossIndicator;

        public bool IsVisible => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetStageText(string text)
        {
            _stageText.text = text;

            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(
                    Vector3.one * 0.1f,
                    TransitionDuration)
                .SetEase(Ease.OutCubic);
        }

        public void ShowBossIndicator(bool show)
        {
            _bossIndicator.SetActive(show);

            if (show)
            {
                _bossIndicator.transform.DOKill();
                _bossIndicator.transform.localScale = Vector3.one;
                _bossIndicator.transform.DOPunchScale(
                        Vector3.one * (BossPunchScale - 1f),
                        BossPunchDuration)
                    .SetEase(Ease.OutElastic);
            }
        }
    }
}
