using System;

namespace DungeonRush.UI
{
    public abstract class PresenterBase<TModel, TView> : IDisposable
        where TModel : ViewModelBase
        where TView : class, IView
    {
        protected TModel Model { get; }
        protected TView View { get; }

        private bool _isDisposed;

        protected PresenterBase(TModel model, TView view)
        {
            Model = model;
            View = view;
        }

        public void Enable()
        {
            Model.OnModelChanged += HandleModelChanged;
            SubscribeEvents();
        }

        public void Disable()
        {
            UnsubscribeEvents();
            Model.OnModelChanged -= HandleModelChanged;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            Disable();
            OnDispose();
            _isDisposed = true;
        }

        protected abstract void SubscribeEvents();

        protected abstract void UnsubscribeEvents();

        protected abstract void HandleModelChanged();

        protected virtual void OnDispose() { }
    }
}
