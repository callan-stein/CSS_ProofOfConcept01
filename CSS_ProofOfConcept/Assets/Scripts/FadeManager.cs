using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour {

    public float FlickerDuration = 0.1f;
    public float FadeDuration = 2.0f;

    [HideInInspector]
    public bool Loading = true;

    public void FadeInOnLoad()
    {
        Loading = true;
        SteamVR_Fade.Start(Color.black, 0.0f);
        SteamVR_Fade.Start(Color.clear, FadeDuration);
        Invoke("SetLoadingFalse", FadeDuration / 2);
    }

    private void FadeToBlack(float duration)
    {
        //set start color
        SteamVR_Fade.Start(Color.clear, 0f);
        //set and start fade to
        SteamVR_Fade.Start(Color.black, duration);
    }

    private void FlickerFromBlack()
    {
        //set start color
        SteamVR_Fade.Start(Color.black, 0f);
        //set and start fade to
        SteamVR_Fade.Start(Color.clear, FlickerDuration);
    }

    private void FadeFromBlack()
    {
        //set start color
        SteamVR_Fade.Start(Color.black, 0f);
        //set and start fade to
        SteamVR_Fade.Start(Color.clear, FadeDuration);
    }

    private void SetLoadingFalse()
    {
        Loading = false;
    }

    public void Flicker()
    {
        FadeToBlack(FlickerDuration);
        Invoke("FlickerFromBlack", FlickerDuration);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetScene()
    {
        Loading = true;
        FadeToBlack(FadeDuration);
        Invoke("ReloadScene", FadeDuration);
    }
}
