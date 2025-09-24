using Fusion;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    private RunnerManager _runnerManager;
    private NetworkCharacterController _controller;
    private NetworkButtons _previousButtons;
    private Animator _animator;

    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 5f;  // original/default speed
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Animation Settings")]
    public float speedLerpTime = 10f; // higher = snappier, lower = smoother
    private float _currentAnimSpeed;
    private bool isDead;

    private void Awake()
    {
        _controller = GetComponent<NetworkCharacterController>();
        _animator = GetComponent<Animator>();
    }

    public override void Spawned()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
            return;

        _runnerManager = FindFirstObjectByType<RunnerManager>();
        isDead = false;

        // Set default movement speed
        _controller.maxSpeed = baseSpeed;

        if (!Object.HasInputAuthority)
        {
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
            return;
        }

        // Local Cinemachine camera for this player only
        CinemachineCamera cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = transform;
            cinemachineCamera.LookAt = transform;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2 || isDead || !HasInputAuthority)
            return;

        if (transform.position.y < -10f)
        {
            isDead = true;
            // Ask the server to respawn this player
            RPC_RequestRespawn(Object.InputAuthority);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // Use yaw from input to compute movement
            Quaternion camRot = Quaternion.Euler(0, data.cameraYaw, 0);
            Vector3 move = camRot * new Vector3(data.moveInput.x, 0, data.moveInput.y);

            // Fusion handles speed via maxSpeed, just pass normalized direction
            _controller.Move(move);

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRot,
                    rotationSpeed * Runner.DeltaTime
                );
            }

            // Jump via NetworkButtons
            if (data.buttons.WasPressed(_previousButtons, InputButton.Jump) && _controller.Grounded)
            {
                _controller.Jump();
            }

            _previousButtons = data.buttons;

            // --- Smooth Speed for Animator ---
            float targetSpeed = move.magnitude * _controller.maxSpeed;
            _currentAnimSpeed = Mathf.Lerp(
                _currentAnimSpeed,
                targetSpeed,
                Runner.DeltaTime * speedLerpTime
            );
            _animator.SetFloat("Speed", _currentAnimSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Trap") && !isDead && Object.HasInputAuthority)
        {
            isDead = true;
            // Ask the server to respawn this player
            RPC_RequestRespawn(Runner.LocalPlayer);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestRespawn(PlayerRef playerRef)
    {
        if (_runnerManager != null)
            _runnerManager.SpawnPlayer(playerRef);
        else
            Debug.LogWarning("RunnerManager not found in PlayerMovement");
    }

    // --- Speed Boost using maxSpeed ---
    public void ApplySpeedBoost(float newMaxSpeed, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SpeedBoostRoutine(newMaxSpeed, duration));
    }

    private IEnumerator SpeedBoostRoutine(float newMaxSpeed, float duration)
    {
        float originalSpeed = baseSpeed;

        _controller.maxSpeed = newMaxSpeed;

        yield return new WaitForSeconds(duration);

        _controller.maxSpeed = originalSpeed;
    }
}
