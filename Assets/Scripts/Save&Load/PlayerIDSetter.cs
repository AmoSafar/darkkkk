using PlayFab;
using UnityEngine;

public class PlayerIDSetter : MonoBehaviour {
    private void Start() {
        var pfId = PlayFabSettings.staticPlayer?.PlayFabId;
        if (string.IsNullOrEmpty(pfId)) {
            Debug.LogWarning("PlayerIDSetter: PlayFabId خالی است. اول باید Login انجام شود.");
            return;
        }

        // ذخیره PlayerID در SaveSystem
        PlayerPrefs.SetString("LoggedInPlayerID", pfId);
        PlayerPrefs.Save();

        // پیدا کردن همه Player ها
        var players = Object.FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (var p in players) {
            // اگر Slot = 2 بود میشه P2، در غیر این صورت P1
            p.playerID = pfId + (p.slot == 2 ? "_P2" : "_P1");

            // روش جایگزین اگر خواستی فقط ۱ و ۲ باشه:
            // p.playerID = pfId + "_P" + Mathf.Clamp(p.slot, 1, 2);
        }
    }
}
