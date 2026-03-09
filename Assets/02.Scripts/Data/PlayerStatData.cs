using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStat", menuName = "DungeonRush/Player Stat Data")]
public class PlayerStatData : ScriptableObject
{
    [Header("체력")]
    public float maxHp = 200f;

    [Header("이동")]
    public float moveSpeed = 4f;

    [Header("탐지")]
    public float detectRange = 15f;
}