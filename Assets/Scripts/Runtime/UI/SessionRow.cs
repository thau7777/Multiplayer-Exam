// SessionRow.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SessionRow : MonoBehaviour
{
    public TMP_Text roomNameText;
    public TMP_Text countText;
    public Button selectButton;

    public void Setup(string sessionName, int playerCount, int maxPlayers, UnityAction onClick)
    {
        if (roomNameText != null) roomNameText.text = sessionName;
        if (countText != null) countText.text = $"{playerCount}/{maxPlayers}";
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(onClick);
        }
    }
}
