using UnityEngine;

public class SPUMAnimationHandler : MonoBehaviour
{
    private SPUM_Prefabs _spumPrefabs;

    private void Awake()
    {
        _spumPrefabs = GetComponentInChildren<SPUM_Prefabs>();
        _spumPrefabs.OverrideControllerInit();
    }

    public void PlayIdle()
    {
        _spumPrefabs.PlayAnimation(PlayerState.IDLE, 0);
    }

    public void PlayMove()
    {
        _spumPrefabs.PlayAnimation(PlayerState.MOVE, 0);
    }

    public void PlayAttack()
    {
        _spumPrefabs.PlayAnimation(PlayerState.ATTACK, 0);
    }

    public void PlayDamaged()
    {
        _spumPrefabs.PlayAnimation(PlayerState.DAMAGED, 0);
    }

    public void PlayDeath()
    {
        _spumPrefabs.PlayAnimation(PlayerState.DEATH, 0);
    }
}