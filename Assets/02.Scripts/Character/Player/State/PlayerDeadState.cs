public class PlayerDeadState : IState
{
    private readonly PlayerController _player;

    public PlayerDeadState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.AutoMove.Stop();
        _player.AnimHandler.PlayDeath();
    }

    public void Execute() { }

    public void Exit() { }
}