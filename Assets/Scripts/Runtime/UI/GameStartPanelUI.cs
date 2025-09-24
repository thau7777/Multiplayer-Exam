using UnityEngine;
using TMPro;
using System.Collections;

public class GameStartPanelUI : Singleton<GameStartPanelUI>
{
    public TMP_Text gameStartText;
    public float animationDuration = 0.5f;
    public float stayDuration = 1f; // how long it stays visible


    protected override void Awake()
    {
        base.Awake(); // important if your Singleton has logic
        ShowGameStart();
    }
   
    public void ShowGameStart()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleDownCoroutine());
        Debug.LogWarning("did run show game start");
    }

    private IEnumerator ScaleDownCoroutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / animationDuration;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t);
            yield return null;
        }

        transform.localScale = Vector3.one;

        // wait for stay duration
        yield return new WaitForSeconds(stayDuration);

        // hide panel
        gameObject.SetActive(false);
    }
}
