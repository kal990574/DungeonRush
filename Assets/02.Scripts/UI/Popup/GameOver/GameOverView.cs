using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DungeonRush.UI.Popup
{
    public class GameOverView : MonoBehaviour, IGameOverView
    {
        private const float ShowDuration = 0.5f;
        private const float StatEntryDelay = 0.15f;
        private const float StatEntryDuration = 0.3f;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI _stageText;
        [SerializeField] private TextMeshProUGUI _killsText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Buttons")]
        [SerializeField] private Button _retryButton;

        [Header("Panel")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform[] _statRows;

        public event Action OnRetryClicked;

        public bool IsVisible => gameObject.activeSelf;

        private void Start()
        {
            _retryButton.onClick.AddListener(() => OnRetryClicked?.Invoke());
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, ShowDuration)
                .SetEase(Ease.OutCubic);

            AnimateStatRows();
        }

        public void Hide()
        {
            _canvasGroup.DOFade(0f, ShowDuration)
                .SetEase(Ease.InCubic)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void SetStageText(string text)
        {
            _stageText.text = text;
        }

        public void SetKillsText(string text)
        {
            _killsText.text = text;
        }

        public void SetTimeText(string text)
        {
            _timeText.text = text;
        }

        public void SetScoreText(string text)
        {
            _scoreText.text = text;
        }

        private void AnimateStatRows()
        {
            if (_statRows == null)
            {
                return;
            }

            for (int i = 0; i < _statRows.Length; i++)
            {
                _statRows[i].localScale = Vector3.zero;
                _statRows[i].DOScale(Vector3.one, StatEntryDuration)
                    .SetDelay(StatEntryDelay * i)
                    .SetEase(Ease.OutBack);
            }
        }
    }
}
