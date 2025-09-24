using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUI : Singleton<MainMenuUI>
{
    [Header("UI - MainMenu")]
    public GameObject mainMenuContainer;
    public GameObject panelHostCreate;
    public TMP_InputField inputRoomName;
    public GameObject panelJoin;
    public Transform sessionListParent;
    public GameObject sessionRowPrefab;
    public GameObject loadingMenu;
    public GameObject refreshingPanel;

    private RunnerManager _runnerManager;

    private void Start()
    {
        AudioManager.Instance.PlayMusic("MainMenu");
        _runnerManager = FindAnyObjectByType<RunnerManager>();
        SetCursorVisible(true);
    }
    public void ModifyMainMenuScreen(bool isLoadingScreenOn)
    {
        if (isLoadingScreenOn)
        {

            mainMenuContainer.SetActive(false);
            loadingMenu.SetActive(true);
        }
        else
        {
            mainMenuContainer.SetActive(true);
            loadingMenu.SetActive(false);
        }
    }
    public void OnHostButton() => panelHostCreate?.SetActive(true);
    public void OnHostCreateConfirm()
    {
        string roomName = !string.IsNullOrEmpty(inputRoomName?.text) ?
                          inputRoomName.text : $"Room_{UnityEngine.Random.Range(1000, 9999)}";
        FindAnyObjectByType<RunnerManager>().CreateRoom(roomName);
    }
    public void OnHostCancel() => panelHostCreate?.SetActive(false);
    public void OnJoinButton() => panelJoin?.SetActive(true);
    public void OnJoinClose() => panelJoin?.SetActive(false);
    public void OnQuitButton() => Application.Quit();

    public void UpdateSessionList(List<SessionInfo> sessionList)
    {
        if (sessionListParent == null || sessionRowPrefab == null) return;

        for (int i = sessionListParent.childCount - 1; i >= 0; i--)
            Destroy(sessionListParent.GetChild(i).gameObject);

        foreach (var s in sessionList)
        {
            if (!s.IsValid) continue;

            var go = Instantiate(sessionRowPrefab, sessionListParent);
            var row = go.GetComponent<SessionRow>();
            if (row != null)
                row.Setup(s.Name, s.PlayerCount, s.MaxPlayers, () => _runnerManager.JoinRoomByName(s.Name));
        }
    }
    public void OnRefreshButton()
    {
        refreshingPanel.SetActive(true);
        _runnerManager?.RefreshSessionList(OnRefreshDone);
    }
    public void OnRefreshDone()
    {
        refreshingPanel.SetActive(false);
    }
    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
