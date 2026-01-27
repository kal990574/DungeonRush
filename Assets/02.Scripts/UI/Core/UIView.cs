using UnityEngine;

public abstract class UIView : MonoBehaviour, IView
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public bool IsVisible { get; private set; }

    protected virtual void Awake()
    {
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        IsVisible = true;
        OnShow();
    }

    public virtual void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        IsVisible = false;
        OnHide();
        gameObject.SetActive(false);
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}
