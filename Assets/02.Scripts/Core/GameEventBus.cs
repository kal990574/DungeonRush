using System;
using DungeonRush.Data;

namespace DungeonRush.Core
{
    public static class GameEventBus
    {
        // HP 이벤트.
        public static event Action<float, float> OnPlayerHPChanged;
        public static event Action<float> OnPlayerDamaged;
        public static event Action<float> OnPlayerHealed;

        // XP 이벤트.
        public static event Action<float, float> OnPlayerXPChanged;
        public static event Action<int> OnPlayerLevelUp;

        // 스테이지 이벤트.
        public static event Action<int, int> OnWaveStart;
        public static event Action<int, int> OnBossWaveStart;
        public static event Action<int> OnChapterStart;

        // 스킬 이벤트.
        public static event Action<int, SkillData> OnSkillEquipped;
        public static event Action<int, SkillData> OnSkillUnequipped;
        public static event Action<int, float, float> OnSkillCooldownUpdate;

        // 골드 이벤트.
        public static event Action<int, int> OnGoldChanged;

        // 카드 선택 이벤트.
        public static event Action<CardData[]> OnCardChoicesReady;
        public static event Action<CardData> OnCardSelected;
        public static event Action<int> OnCardRerollRequested;
        public static event Action<int, CardData> OnCardRerolled;

        // 게임 오버 이벤트.
        public static event Action<int, int, int, float, int> OnGameOver;

        // 시너지 이벤트.
        public static event Action<SynergyData, SynergyTier> OnSynergyActivated;
        public static event Action<SynergyData> OnSynergyDeactivated;

        // 게임 상태 이벤트.
        public static event Action<GameState> OnGameStateChanged;

        public static void PublishPlayerHPChanged(float currentHP, float maxHP)
        {
            OnPlayerHPChanged?.Invoke(currentHP, maxHP);
        }

        public static void PublishPlayerDamaged(float damage)
        {
            OnPlayerDamaged?.Invoke(damage);
        }

        public static void PublishPlayerHealed(float amount)
        {
            OnPlayerHealed?.Invoke(amount);
        }

        public static void PublishPlayerXPChanged(float currentXP, float requiredXP)
        {
            OnPlayerXPChanged?.Invoke(currentXP, requiredXP);
        }

        public static void PublishPlayerLevelUp(int newLevel)
        {
            OnPlayerLevelUp?.Invoke(newLevel);
        }

        public static void PublishWaveStart(int chapter, int wave)
        {
            OnWaveStart?.Invoke(chapter, wave);
        }

        public static void PublishBossWaveStart(int chapter, int wave)
        {
            OnBossWaveStart?.Invoke(chapter, wave);
        }

        public static void PublishChapterStart(int chapter)
        {
            OnChapterStart?.Invoke(chapter);
        }

        public static void PublishSkillEquipped(int slotIndex, SkillData skillData)
        {
            OnSkillEquipped?.Invoke(slotIndex, skillData);
        }

        public static void PublishSkillUnequipped(int slotIndex, SkillData skillData)
        {
            OnSkillUnequipped?.Invoke(slotIndex, skillData);
        }

        public static void PublishSkillCooldownUpdate(int slotIndex, float remaining, float total)
        {
            OnSkillCooldownUpdate?.Invoke(slotIndex, remaining, total);
        }

        public static void PublishGoldChanged(int currentGold, int delta)
        {
            OnGoldChanged?.Invoke(currentGold, delta);
        }

        public static void PublishCardChoicesReady(CardData[] cards)
        {
            OnCardChoicesReady?.Invoke(cards);
        }

        public static void PublishCardSelected(CardData card)
        {
            OnCardSelected?.Invoke(card);
        }

        public static void PublishCardRerollRequested(int cardIndex)
        {
            OnCardRerollRequested?.Invoke(cardIndex);
        }

        public static void PublishCardRerolled(int cardIndex, CardData newCard)
        {
            OnCardRerolled?.Invoke(cardIndex, newCard);
        }

        public static void PublishGameOver(int chapter, int wave, int kills, float time, int score)
        {
            OnGameOver?.Invoke(chapter, wave, kills, time, score);
        }

        public static void PublishSynergyActivated(SynergyData synergyData, SynergyTier tier)
        {
            OnSynergyActivated?.Invoke(synergyData, tier);
        }

        public static void PublishSynergyDeactivated(SynergyData synergyData)
        {
            OnSynergyDeactivated?.Invoke(synergyData);
        }

        public static void PublishGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        public static void Clear()
        {
            OnPlayerHPChanged = null;
            OnPlayerDamaged = null;
            OnPlayerHealed = null;
            OnPlayerXPChanged = null;
            OnPlayerLevelUp = null;
            OnWaveStart = null;
            OnBossWaveStart = null;
            OnChapterStart = null;
            OnSkillEquipped = null;
            OnSkillUnequipped = null;
            OnSkillCooldownUpdate = null;
            OnGoldChanged = null;
            OnCardChoicesReady = null;
            OnCardSelected = null;
            OnCardRerollRequested = null;
            OnCardRerolled = null;
            OnGameOver = null;
            OnSynergyActivated = null;
            OnSynergyDeactivated = null;
            OnGameStateChanged = null;
        }
    }
}
