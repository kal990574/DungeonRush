using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DungeonRush.Data;

namespace DungeonRush.UI.Popup
{
    public class CardSelectView : MonoBehaviour, ICardSelectView
    {
        private const float CardShowDelay = 0.1f;
        private const float CardShowDuration = 0.3f;

        [Header("Card Slots")]
        [SerializeField] private Button[] _cardButtons = new Button[3];
        [SerializeField] private Image[] _cardIcons = new Image[3];
        [SerializeField] private TextMeshProUGUI[] _cardNames = new TextMeshProUGUI[3];
        [SerializeField] private TextMeshProUGUI[] _cardDescriptions = new TextMeshProUGUI[3];

        [Header("Reroll")]
        [SerializeField] private Button _rerollButton;
        [SerializeField] private TextMeshProUGUI _rerollText;

        [Header("Panel")]
        [SerializeField] private CanvasGroup _canvasGroup;

        public event Action<int> OnCardClicked;
        public event Action OnRerollClicked;

        public bool IsVisible => gameObject.activeSelf;

        private void Start()
        {
            for (int i = 0; i < _cardButtons.Length; i++)
            {
                int index = i;
                _cardButtons[i].onClick.AddListener(() => OnCardClicked?.Invoke(index));
            }

            _rerollButton.onClick.AddListener(() => OnRerollClicked?.Invoke());
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, CardShowDuration)
                .SetEase(Ease.OutCubic);
        }

        public void Hide()
        {
            _canvasGroup.DOFade(0f, CardShowDuration)
                .SetEase(Ease.InCubic)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void ShowCards(CardData[] cards)
        {
            for (int i = 0; i < _cardButtons.Length; i++)
            {
                if (i >= cards.Length || cards[i] == null)
                {
                    _cardButtons[i].gameObject.SetActive(false);
                    continue;
                }

                _cardButtons[i].gameObject.SetActive(true);
                _cardIcons[i].sprite = cards[i].icon;
                _cardNames[i].text = cards[i].cardName;
                _cardDescriptions[i].text = cards[i].description;

                AnimateCardEntry(_cardButtons[i].transform, i);
            }
        }

        public void SetRerollButtonText(string text)
        {
            _rerollText.text = text;
        }

        public void SetRerollButtonInteractable(bool interactable)
        {
            _rerollButton.interactable = interactable;
        }

        private void AnimateCardEntry(Transform cardTransform, int index)
        {
            cardTransform.localScale = Vector3.zero;
            cardTransform.DOScale(Vector3.one, CardShowDuration)
                .SetDelay(CardShowDelay * index)
                .SetEase(Ease.OutBack);
        }
    }
}
