using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveInput;
    public bool jumpPressed;
}

[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterController _controller;
    private Transform _camera;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private void Awake()
    {
        _controller = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
            _camera = Camera.main.transform;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector3 move = Vector3.zero;
            if (_camera != null)
            {
                Vector3 forward = _camera.forward;
                Vector3 right = _camera.right;
                forward.y = 0f;
                right.y = 0f;
                move = forward.normalized * data.moveInput.y + right.normalized * data.moveInput.x;
            }
            else
            {
                move = new Vector3(data.moveInput.x, 0, data.moveInput.y);
            }

            _controller.Move(move * moveSpeed);

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Runner.DeltaTime);
            }

            if (data.jumpPressed && _controller.Grounded)
            {
                _controller.Jump();
            }
        }
    }

}
