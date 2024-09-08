using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public enum SignInMethod
{
    GooglePlayGames,Google,Facebook
}
public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI UIDText;

    public SignInMethod signInMethod;
    void Start()
    {
        if (Debug.isDebugBuild)
        {
            UIDText.text = 00000001.ToString();
        }
        
        var cts = this.GetCancellationTokenOnDestroy();
        OnStartLoading(cts).Forget();
        OnLoadingTextAnim(cts).Forget();
    }

    void OnSignInCheck()
    {
        // switch (signInMethod)
        // {
        //     case SignInMethod.GooglePlayGames:
        //         PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        //         Debug.Log("PlayGames SignIn");
        //         break;
        //     default:
        //         Debug.Log("Sign-In Checking");
        //         break;
        // }
    }

    // private void ProcessAuthentication(SignInStatus status) {
    //     if (status == SignInStatus.Success)
    //     {
    //         UIDText.gameObject.SetActive(true);
    //         UIDText.text = PlayGamesPlatform.Instance.GetUserId();
    //     } else {
    //         // Disable your integration with Play Games Services or show a login button
    //         // to ask users to sign-in. Clicking it should call
    //         // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
    //     }
    // }
    
    async UniTask OnLoadingTextAnim(CancellationToken cancellationToken)
    {
        int dotCount = 0;
        string[] loadingTextVariations = { "Loading", "Loading.", "Loading..", "Loading..." };
        
        while (true)
        {
            loadingText.text = loadingTextVariations[dotCount];
            dotCount = (dotCount + 1) % loadingTextVariations.Length;

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f),cancellationToken:cancellationToken);
            //TODO disable after check login
        }
    }
    async UniTask OnStartLoading(CancellationToken cancellationToken)
    {
        float transitTime = Transitioner.Instance._transitionTime;
        await UniTask.Delay(TimeSpan.FromSeconds(transitTime), 
            cancellationToken:cancellationToken);

        //OnSignInCheck();
        loadingObj.SetActive(true);
        
        await UniTask.Delay(TimeSpan.FromSeconds(5f), 
            cancellationToken:cancellationToken);

        loadingObj.SetActive(false);
        Transitioner.Instance.TransitionToScene("Home");
    }
    
}
