// PlayerRow.cs
using UnityEngine;
using TMPro;

public class PlayerRow : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text statusText;

    public void Setup(string playerName, bool isReady)
    {
        if (nameText != null) nameText.text = playerName;
        if (statusText != null) statusText.text = isReady ? "Ready" : "Not Ready";
    }
}
