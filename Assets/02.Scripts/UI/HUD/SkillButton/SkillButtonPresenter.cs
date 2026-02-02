using DungeonRush.Core;
using DungeonRush.Data;

namespace DungeonRush.UI.HUD
{
    public class SkillButtonPresenter : PresenterBase<SkillButtonModel, ISkillButtonView>
    {
        public SkillButtonPresenter(SkillButtonModel model, ISkillButtonView view)
            : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnSkillEquipped += HandleSkillEquipped;
            GameEventBus.OnSkillUnequipped += HandleSkillUnequipped;
            GameEventBus.OnSkillCooldownUpdate += HandleSkillCooldownUpdate;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnSkillEquipped -= HandleSkillEquipped;
            GameEventBus.OnSkillUnequipped -= HandleSkillUnequipped;
            GameEventBus.OnSkillCooldownUpdate -= HandleSkillCooldownUpdate;
        }

        protected override void HandleModelChanged()
        {
            if (Model.IsLocked)
            {
                View.ShowLockedState();
                return;
            }

            if (Model.SkillData != null)
            {
                View.SetIcon(Model.SkillData.icon);
            }

            View.SetCooldown(Model.CooldownRatio);

            if (!Model.IsOnCooldown)
            {
                View.ShowReadyState();
            }
        }

        private void HandleSkillEquipped(int slotIndex, SkillData skillData)
        {
            if (slotIndex != Model.SlotIndex)
            {
                return;
            }

            Model.EquipSkill(skillData);
        }

        private void HandleSkillUnequipped(int slotIndex, SkillData skillData)
        {
            if (slotIndex != Model.SlotIndex)
            {
                return;
            }

            Model.UnequipSkill();
        }

        private void HandleSkillCooldownUpdate(int slotIndex, float remaining, float total)
        {
            if (slotIndex != Model.SlotIndex)
            {
                return;
            }

            Model.UpdateCooldown(remaining, total);
        }
    }
}
