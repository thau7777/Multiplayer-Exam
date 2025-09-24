using UnityEngine;
using System.Collections;

public class MinimapUI : Singleton<MinimapUI>
{

    [SerializeField] private GameObject panel;
    [SerializeField] private Vector3 startScale = new Vector3(2f, 2f, 2f);
    [SerializeField] private float shrinkDuration = 0.4f;
    [SerializeField] private float holdTime = 2f;

    protected override void Awake()
    {
        panel.SetActive(false);
    }

    public void ShowMinimap()
    {
        panel.SetActive(true);
        panel.transform.localScale = startScale;

        StopAllCoroutines();
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        // Step 1: Shrink down to Vector3.one
        Vector3 targetScale = Vector3.one;
        Vector3 initialScale = panel.transform.localScale;

        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            panel.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }
        panel.transform.localScale = targetScale;

        // Step 2: Hold for 2 seconds
        yield return new WaitForSeconds(holdTime);

        // Step 3: Hide
        panel.SetActive(false);
    }
}
