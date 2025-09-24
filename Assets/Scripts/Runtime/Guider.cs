using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Guider : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _exitTarget;
    private Transform _player;
    private int _runCount = 0;
    private int _maxRuns = 3;

    [Header("Settings")]
    public float speed = 20f;   // very fast
    public float arriveDistance = 1f;

    public void Initialize(Transform player, Transform exitTarget)
    {
        _agent = GetComponent<NavMeshAgent>();
        _exitTarget = exitTarget;
        _player = player;

        _agent.speed = speed;

        StartCoroutine(RunCycle());
    }

    private IEnumerator RunCycle()
    {
        while (_runCount < _maxRuns)
        {
            while(_player == null)
                yield return null;
            // Move guider back to player position instantly
            transform.position = _player.position;
            _agent.Warp(_player.position);

            // Set destination to exit
            _agent.SetDestination(_exitTarget.position);

            // Wait until reached
            while (_agent.pathPending || _agent.remainingDistance > arriveDistance)
                yield return null;

            _runCount++;

            // Tiny pause before next run
            yield return new WaitForSeconds(0.3f);
        }

        Destroy(gameObject);
    }
}
