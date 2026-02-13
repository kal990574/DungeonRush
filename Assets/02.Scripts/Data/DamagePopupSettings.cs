using UnityEngine;

[CreateAssetMenu(fileName = "DamagePopupSettings", menuName = "DungeonRush/Damage Popup Settings")]
public class DamagePopupSettings : ScriptableObject
{
    [Header("텍스트")]
    public float fontSize = 5f;
    public Color normalColor = Color.white;

    [Header("애니메이션")]
    public float floatHeight = 1.2f;
    public float duration = 0.8f;
    public Vector3 offset = new Vector3(0f, 0.5f, 0f);
    public float randomOffsetX = 0.3f;

    [Header("스케일 펀치")]
    public float punchScale = 1.3f;
    public float punchDuration = 0.15f;

    [Header("풀링")]
    public int defaultCapacity = 20;
    public int maxSize = 50;
}