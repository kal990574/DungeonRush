using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DungeonRush.Core;
using DungeonRush.Data;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace DungeonRush.UI.Popup
{
    public class CardSelectView : MonoBehaviour, ICardSelectView
    {
        private const float PanelFadeDuration = 0.3f;
        private const float CardScaleUpDuration = 0.2f;
        private const float CardScaleDownDuration = 0.15f;
        private const float CardFlipDuration = 0.2f;
        private const float DelayBeforeFlip = 0.1f;
        private const float DelayAfterFlip = 0.1f;
        private const float CardHighlightScale = 1.15f;
        private const float CardFlipStretchScale = 1.2f;

        [Header("Card Slots")]
        [SerializeField] private Button[] _cardButtons = new Button[3];
        [SerializeField] private Image[] _cardIcons = new Image[3];
        [SerializeField] private TextMeshProUGUI[] _cardNames = new TextMeshProUGUI[3];
        [SerializeField] private TextMeshProUGUI[] _cardDescriptions = new TextMeshProUGUI[3];
        [SerializeField] private Image[] _cardFrames = new Image[3];
        [SerializeField] private Image[] _cardDecoLeaves = new Image[3];

        [Header("Card Two-Sided (Feel)")]
        [SerializeField] private RectTransform[] _cardFlipTransforms = new RectTransform[3];
        [SerializeField] private MMTwoSidedUI[] _cardTwoSidedUIs = new MMTwoSidedUI[3];

        [Header("Card Shake (Feel)")]
        [SerializeField] private MMF_Player _cardContainerShakeFeedback;

        private Sequence _cardSequence;

        [Header("Reroll (Per Card)")]
        [SerializeField] private Button[] _rerollButtons = new Button[3];
        [SerializeField] private TextMeshProUGUI[] _rerollTexts = new TextMeshProUGUI[3];

        [Header("Rarity Colors")]
        [SerializeField] private Color _commonColor = new Color(0.78f, 0.78f, 0.78f);
        [SerializeField] private Color _rareColor = new Color(0.30f, 0.60f, 1.00f);
        [SerializeField] private Color _epicColor = new Color(0.70f, 0.30f, 1.00f);
        [SerializeField] private Color _legendaryColor = new Color(1.00f, 0.75f, 0.20f);

        [Header("Panel")]
        [SerializeField] private CanvasGroup _canvasGroup;

        public event Action<int> OnCardClicked;
        public event Action<int> OnRerollClicked;

        public bool IsVisible => gameObject.activeSelf;

        private void Start()
        {
            for (int i = 0; i < _cardButtons.Length; i++)
            {
                int index = i;
                _cardButtons[i].onClick.AddListener(() => OnCardClicked?.Invoke(index));
                _rerollButtons[i].onClick.AddListener(() => OnRerollClicked?.Invoke(index));
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, PanelFadeDuration)
                .SetEase(Ease.OutCubic);
        }

        public void Hide()
        {
            _cardSequence?.Kill();
            _canvasGroup.DOFade(0f, PanelFadeDuration)
                .SetEase(Ease.InCubic)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void OnDestroy()
        {
            _cardSequence?.Kill();
        }

        public void ShowCards(CardData[] cards)
        {
            int activeCardCount = 0;

            for (int i = 0; i < _cardButtons.Length; i++)
            {
                if (i >= cards.Length || cards[i] == null)
                {
                    _cardButtons[i].gameObject.SetActive(false);
                    _rerollButtons[i].gameObject.SetActive(false);
                    continue;
                }

                _cardButtons[i].gameObject.SetActive(true);
                _rerollButtons[i].gameObject.SetActive(true);
                SetCardSlot(i, cards[i]);
                InitializeCardForFlip(i);
                activeCardCount++;
            }

            // Animate cards sequentially.
            AnimateCardsSequentially(activeCardCount);
        }

        public void ShowCard(int index, CardData card)
        {
            SetCardSlot(index, card);
            AnimateCardFlipOnly(index);
        }

        public void SetRerollText(int index, string text)
        {
            _rerollTexts[index].text = text;
        }

        public void SetRerollInteractable(int index, bool interactable)
        {
            _rerollButtons[index].interactable = interactable;
        }

        private void SetCardSlot(int index, CardData card)
        {
            _cardIcons[index].sprite = card.icon;
            _cardNames[index].text = card.cardName;
            _cardDescriptions[index].text = card.description;

            Color frameColor = GetRarityColor(card.rarity);
            _cardFrames[index].color = frameColor;
            _cardDecoLeaves[index].color = GetAccentColor(frameColor);
        }

        private Color GetRarityColor(CardRarity rarity)
        {
            return rarity switch
            {
                CardRarity.Common => _commonColor,
                CardRarity.Rare => _rareColor,
                CardRarity.Epic => _epicColor,
                CardRarity.Legendary => _legendaryColor,
                _ => Color.white
            };
        }

        private static Color GetAccentColor(Color baseColor)
        {
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);
            s = Mathf.Clamp01(s + 0.1f);
            v = Mathf.Clamp01(v - 0.02f);
            return Color.HSVToRGB(h, s, v);
        }

        private void InitializeCardForFlip(int index)
        {
            if (_cardFlipTransforms[index] != null)
            {
                _cardFlipTransforms[index].localScale = new Vector3(-1f, 1f, 1f);
            }

            _cardButtons[index].transform.localScale = Vector3.one;
        }

        private void AnimateCardsSequentially(int cardCount)
        {
            _cardSequence?.Kill();
            _cardSequence = DOTween.Sequence();

            for (int i = 0; i < cardCount; i++)
            {
                int index = i;
                Transform cardTransform = _cardButtons[index].transform;
                RectTransform flipTransform = _cardFlipTransforms[index];

                _cardSequence.AppendCallback(() =>
                {
                    if (_cardContainerShakeFeedback != null)
                    {
                        _cardContainerShakeFeedback.PlayFeedbacks();
                    }
                });

                _cardSequence.Append(
                    cardTransform.DOScale(CardHighlightScale, CardScaleUpDuration)
                        .SetEase(Ease.OutBack)
                );

                _cardSequence.AppendInterval(DelayBeforeFlip);

                if (flipTransform != null)
                {
                    _cardSequence.Append(CreateFlipTween(flipTransform));
                }

                _cardSequence.Append(
                    cardTransform.DOScale(1f, CardScaleDownDuration)
                        .SetEase(Ease.InOutSine)
                );

                if (index < cardCount - 1)
                {
                    _cardSequence.AppendInterval(DelayAfterFlip);
                }
            }
        }

        private void AnimateCardFlipOnly(int index)
        {
            if (_cardFlipTransforms[index] != null)
            {
                _cardFlipTransforms[index].localScale = new Vector3(-1f, 1f, 1f);
                CreateFlipTween(_cardFlipTransforms[index]);
            }
        }

        private Sequence CreateFlipTween(RectTransform flipTransform)
        {
            float halfDuration = CardFlipDuration * 0.5f;

            Sequence stretchTween = DOTween.Sequence()
                .Append(flipTransform.DOScaleY(CardFlipStretchScale, halfDuration).SetEase(Ease.OutSine))
                .Append(flipTransform.DOScaleY(1f, halfDuration).SetEase(Ease.InSine));

            return DOTween.Sequence()
                .Append(flipTransform.DOScaleX(1f, CardFlipDuration).SetEase(Ease.InOutSine))
                .Join(stretchTween);
        }
    }
}
