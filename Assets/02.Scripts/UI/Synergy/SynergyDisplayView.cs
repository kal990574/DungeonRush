using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using DungeonRush.Core;

namespace DungeonRush.UI.Synergy
{
    public class SynergyDisplayView : MonoBehaviour, ISynergyDisplayView
    {
        private const float ActivatePunchScale = 1.2f;
        private const float ActivatePunchDuration = 0.3f;

        [Header("References")]
        [SerializeField] private Transform _synergyContainer;
        [SerializeField] private TextMeshProUGUI _synergyCountText;

        public bool IsVisible => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetSynergies(IReadOnlyList<SynergyEntry> synergies)
        {
            _synergyCountText.text = synergies.Count.ToString();
        }

        public void PlayActivateAnimation()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(
                    Vector3.one * (ActivatePunchScale - 1f),
                    ActivatePunchDuration)
                .SetEase(Ease.OutCubic);
        }

        public void PlayDeactivateAnimation()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(
                    Vector3.one * -0.1f,
                    ActivatePunchDuration)
                .SetEase(Ease.InCubic);
        }
    }
}
