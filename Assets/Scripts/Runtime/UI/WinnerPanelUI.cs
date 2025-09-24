using TMPro;
using UnityEngine;

public class WinnerPanelUI : Singleton<WinnerPanelUI>
{

    public TMP_Text winnerNameText;
    public TMP_Text countdownText;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void ShowWinner(string winnerName, float countdownTime)
    {
        gameObject.SetActive(true);

        if (winnerNameText == null)
        {
            Debug.LogError("winnerNameText is NULL! Check WinnerPanelUI instance: " + gameObject.name);
            return;
        }

        winnerNameText.text = $"Winner: {winnerName}";
        StartCoroutine(CountdownCoroutine(countdownTime));
    }


    private System.Collections.IEnumerator CountdownCoroutine(float time)
    {
        for (int i = (int)time; i >= 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
    }
}
