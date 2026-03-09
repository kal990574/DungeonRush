using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;

    private IObjectPool<DamagePopup> _pool;
    private DamagePopupSettings _settings;

    public void Initialize(IObjectPool<DamagePopup> pool, DamagePopupSettings settings)
    {
        _pool = pool;
        _settings = settings;
    }

    public void Play(float damage, Vector3 position)
    {
        float randomX = Random.Range(-_settings.randomOffsetX, _settings.randomOffsetX);
        transform.position = position + _settings.offset + new Vector3(randomX, 0f, 0f);
        transform.localScale = Vector3.one;

        _text.text = Mathf.RoundToInt(damage).ToString();
        _text.color = _settings.normalColor;
        _text.fontSize = _settings.fontSize;

        Sequence seq = DOTween.Sequence();
        seq.Join(transform.DOMoveY(
            transform.position.y + _settings.floatHeight,
            _settings.duration).SetEase(Ease.OutCubic));
        seq.Join(_text.DOFade(0f, _settings.duration).SetEase(Ease.InQuad));
        seq.Insert(0f, transform.DOPunchScale(
            Vector3.one * (_settings.punchScale - 1f),
            _settings.punchDuration));
        seq.OnComplete(() => _pool.Release(this));
    }
}