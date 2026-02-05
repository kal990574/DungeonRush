#if UNITY_EDITOR
using UnityEngine;
using DungeonRush.Core;
using DungeonRush.Data;

namespace DungeonRush.Debug
{
    public class PopupTestController : MonoBehaviour
    {
        private int _killCount = 42;
        private float _playTime;

        private void Update()
        {
            _playTime += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ShowCardSelect();
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ShowGameOver();
            }
        }

        private void ShowCardSelect()
        {
            var cards = new CardData[3];

            for (int i = 0; i < 3; i++)
            {
                cards[i] = CreateTestCard(i);
            }

            GameEventBus.PublishCardChoicesReady(cards);
        }

        private void ShowGameOver()
        {
            int score = _killCount * 100 + 1500;
            GameEventBus.PublishGameOver(3, 7, _killCount, _playTime, score);
        }

        private CardData CreateTestCard(int index)
        {
            var card = ScriptableObject.CreateInstance<CardData>();

            switch (index)
            {
                case 0:
                    card.cardName = "HP Boost";
                    card.description = "Max HP +20";
                    card.rarity = CardRarity.Common;
                    card.effectType = CardEffectType.StatBoost;
                    card.targetStat = StatType.MaxHP;
                    card.effectValue = 20f;
                    break;
                case 1:
                    card.cardName = "Attack Up";
                    card.description = "Attack +10";
                    card.rarity = CardRarity.Uncommon;
                    card.effectType = CardEffectType.StatBoost;
                    card.targetStat = StatType.Attack;
                    card.effectValue = 10f;
                    break;
                case 2:
                    card.cardName = "Heal";
                    card.description = "HP 30% Recovery";
                    card.rarity = CardRarity.Rare;
                    card.effectType = CardEffectType.Heal;
                    card.effectValue = 0.3f;
                    break;
            }

            return card;
        }
    }
}
#endif
