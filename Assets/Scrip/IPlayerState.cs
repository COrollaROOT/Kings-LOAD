public interface IPlayerState
{
    void Enter();
    void Exit();
    void Tick();
    void FixedTick();
}