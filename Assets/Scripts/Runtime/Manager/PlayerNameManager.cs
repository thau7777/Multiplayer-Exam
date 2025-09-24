using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you’re using TextMeshPro for UI text

public class PlayerNameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject nameInputPanel; // Small window for entering name
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Text playerNameDisplay;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button renameButton;

    private const string PlayerNameKey = "PlayerName";

    private void Start()
    {
        string savedName = PlayerPrefs.GetString(PlayerNameKey, string.Empty);

        if (string.IsNullOrEmpty(savedName))
        {
            // No name yet → show input panel
            nameInputPanel.SetActive(true);
            playerNameDisplay.gameObject.SetActive(false);
        }
        else
        {
            // Already has a name → show display
            nameInputPanel.SetActive(false);
            playerNameDisplay.gameObject.SetActive(true);
            playerNameDisplay.text = savedName;
        }

        confirmButton.onClick.AddListener(ConfirmName);
        renameButton.onClick.AddListener(Rename);
    }

    private void ConfirmName()
    {
        string newName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(newName))
            newName = $"Player#{Random.Range(1000, 9999)}";

        // Save locally
        PlayerPrefs.SetString(PlayerNameKey, newName);
        PlayerPrefs.Save();

        // Update UI
        playerNameDisplay.text = newName;
        playerNameDisplay.gameObject.SetActive(true);
        nameInputPanel.SetActive(false);
    }

    private void Rename()
    {
        // Show input panel again
        nameInputField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        nameInputPanel.SetActive(true);
        playerNameDisplay.gameObject.SetActive(false);
    }

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString(PlayerNameKey, "Unknown");
    }
}
