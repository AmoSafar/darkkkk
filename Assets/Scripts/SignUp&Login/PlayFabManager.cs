using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_Text errorText;

    public void Signup()
    {
        var request = new RegisterPlayFabUserRequest {
            Email = email.text,
            Password = password.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnSignupSuccess, OnError);
    }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest {
            Email = email.text,
            Password = password.text,
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void RecoverPassword()
    {
        var request = new SendAccountRecoveryEmailRequest {
            Email = email.text,
            TitleId = "1D6567"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnError);
    }

    void OnSignupSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Signup Successful");
    // ست کردن شناسه‌ی اکانت برای فایل‌های سیو
    if (!string.IsNullOrEmpty(result.PlayFabId)) {
        PlayerPrefs.SetString("LoggedInPlayerID", result.PlayFabId);
        PlayerPrefs.Save();
    }
    // رفتن به اولین سین گیم‌پلی (که GameManager دارد)
    SceneManager.LoadScene(6, LoadSceneMode.Single);
    }

    void OnLoginSuccess(LoginResult result)
    {
          // ست کردن شناسه‌ی اکانت برای فایل‌های سیو
    if (!string.IsNullOrEmpty(result.PlayFabId)) {
        PlayerPrefs.SetString("LoggedInPlayerID", result.PlayFabId);
        PlayerPrefs.Save();
    }
    // رفتن به اولین سین گیم‌پلی (که GameManager دارد)
    SceneManager.LoadScene(6, LoadSceneMode.Single);
    }

    void OnRecoverySuccess(SendAccountRecoveryEmailResult result){
        Debug.Log("Email Sent!");
    }

    void OnError(PlayFabError error){
        errorText.text = error.GenerateErrorReport();
    }
}