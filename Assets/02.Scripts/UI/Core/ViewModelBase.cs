using System;

namespace DungeonRush.UI
{
    public abstract class ViewModelBase
    {
        public event Action OnModelChanged;

        protected void NotifyChanged()
        {
            OnModelChanged?.Invoke();
        }
    }
}
