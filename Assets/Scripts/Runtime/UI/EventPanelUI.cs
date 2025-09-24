using UnityEngine;
using TMPro;
using System.Collections;

public class EventPanelUI : Singleton<EventPanelUI>
{

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text eventText;

    [Header("Pulse Settings")]
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private float pulseSpeed = 2f;

    private Coroutine pulseRoutine;

    protected override void Awake()
    {
        panel.SetActive(false);
    }

    public void ShowChoosing(string message)
    {
        panel.SetActive(true);
        eventText.text = message;

        // Start pulsing
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseEffect());
    }

    public void ShowEvent(string message)
    {
        eventText.text = message;

        // Stop pulsing and reset scale back to Vector3.one
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
        StartCoroutine(SmoothScale(Vector3.one, 0.3f)); // quick settle
        StartCoroutine(Hide(2f)); // hide after 2 seconds
    }

    public IEnumerator Hide(float time)
    {
        yield return new WaitForSeconds(time);
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
        panel.SetActive(false);
    }

    private IEnumerator PulseEffect()
    {
        Transform t = panel.transform;
        while (true)
        {
            // Lerp up and down forever
            yield return SmoothScale(maxScale, 0.5f / pulseSpeed);
            yield return SmoothScale(Vector3.one, 0.5f / pulseSpeed);
        }
    }

    private IEnumerator SmoothScale(Vector3 target, float duration)
    {
        Transform t = panel.transform;
        Vector3 start = t.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t01 = time / duration;
            t.localScale = Vector3.Lerp(start, target, t01);
            yield return null;
        }
        t.localScale = target;
    }
}
