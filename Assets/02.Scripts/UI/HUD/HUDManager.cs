using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonRush.UI.HUD
{
    public class HUDManager : MonoBehaviour
    {
        private const int SkillSlotCount = 3;

        [Header("HUD Views")]
        [SerializeField] private HPBarView _hpBarView;
        [SerializeField] private XPBarView _xpBarView;
        [SerializeField] private StageDisplayView _stageDisplayView;
        [SerializeField] private GoldDisplayView _goldDisplayView;
        [SerializeField] private SkillButtonView[] _skillButtonViews = new SkillButtonView[SkillSlotCount];

        private readonly List<IDisposable> _presenters = new List<IDisposable>();

        private void Start()
        {
            CreatePresenters();
        }

        private void OnDestroy()
        {
            DisposePresenters();
        }

        private void CreatePresenters()
        {
            CreatePresenter(new HPBarModel(), _hpBarView);
            CreatePresenter(new XPBarModel(), _xpBarView);
            CreatePresenter(new StageDisplayModel(), _stageDisplayView);
            CreatePresenter(new GoldDisplayModel(), _goldDisplayView);
            CreateSkillButtonPresenters();
        }

        private void CreatePresenter(HPBarModel model, HPBarView view)
        {
            var presenter = new HPBarPresenter(model, view);
            presenter.Enable();
            _presenters.Add(presenter);
        }

        private void CreatePresenter(XPBarModel model, XPBarView view)
        {
            var presenter = new XPBarPresenter(model, view);
            presenter.Enable();
            _presenters.Add(presenter);
        }

        private void CreatePresenter(StageDisplayModel model, StageDisplayView view)
        {
            var presenter = new StageDisplayPresenter(model, view);
            presenter.Enable();
            _presenters.Add(presenter);
        }

        private void CreatePresenter(GoldDisplayModel model, GoldDisplayView view)
        {
            var presenter = new GoldDisplayPresenter(model, view);
            presenter.Enable();
            _presenters.Add(presenter);
        }

        private void CreateSkillButtonPresenters()
        {
            for (int i = 0; i < SkillSlotCount; i++)
            {
                if (i >= _skillButtonViews.Length || _skillButtonViews[i] == null)
                {
                    continue;
                }

                var model = new SkillButtonModel(i);
                var presenter = new SkillButtonPresenter(model, _skillButtonViews[i]);
                presenter.Enable();
                _presenters.Add(presenter);
            }
        }

        private void DisposePresenters()
        {
            foreach (IDisposable presenter in _presenters)
            {
                presenter.Dispose();
            }

            _presenters.Clear();
        }
    }
}
