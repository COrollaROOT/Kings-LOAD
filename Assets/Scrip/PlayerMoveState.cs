using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    readonly PlayerController controller;
    readonly PlayerStateMachine stateMachine;

    public PlayerMoveState(PlayerController controller, PlayerStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        // 여기서 Walk/Run 애니메이션 파라미터 처리 가능
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        if (!controller.HasMoveInput)
            controller.ChangeToIdle();
    }

    public void FixedTick()
    {
        Vector3 dir = controller.MoveDirection;
        if (dir.sqrMagnitude <= 0.0001f)
            return;

        Move(dir);
        Rotate(dir);
    }

    void Move(Vector3 dir)
    {
        Rigidbody rb = controller.PlayerRigidbody;
        Vector3 next = rb.position + (dir * controller.MoveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);
    }

    void Rotate(Vector3 dir)
    {
        Rigidbody rb = controller.PlayerRigidbody;

        Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion next = Quaternion.Slerp(rb.rotation, target, controller.RotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(next);
    }
}