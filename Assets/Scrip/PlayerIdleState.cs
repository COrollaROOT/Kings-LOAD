public class PlayerIdleState : IPlayerState
{
    readonly PlayerController controller;
    readonly PlayerStateMachine stateMachine;

    public PlayerIdleState(PlayerController controller, PlayerStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        // 여기서 Idle 애니메이션 파라미터 처리 가능
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        if (controller.HasMoveInput)
            controller.ChangeToMove();
    }

    public void FixedTick()
    {
    }
}