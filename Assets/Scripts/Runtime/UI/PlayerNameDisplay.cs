using TMPro;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    public void SetName(string playerName)
    {
        if (nameText != null)
            nameText.text = playerName;
    }

    private void LateUpdate()
    {
        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam != null)
        {
            // Make text face the camera
            transform.LookAt(transform.position + mainCam.transform.forward);
        }
    }
}
