using UnityEngine;

namespace DungeonRush.UI.HUD
{
    public interface ISkillButtonView : IView
    {
        void SetIcon(Sprite icon);
        void SetCooldown(float ratio);
        void ShowLockedState();
        void ShowReadyState();
    }
}
