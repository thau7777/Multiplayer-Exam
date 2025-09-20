using Fusion;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterController _controller;
    private Transform _camera;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private NetworkButtons _buttons;
    private void Awake()
    {
        _controller = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority) return;

        _camera = Camera.main.transform;
        CinemachineCamera cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = transform;
            cinemachineCamera.LookAt = transform;
        }
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

            // Jump via NetworkButtons
            if (data.buttons.WasPressed(_buttons, InputButton.Jump) && _controller.Grounded)
            {
                _controller.Jump();
            }
            _buttons = data.buttons; // Update the last known button state
        }
    }
}
