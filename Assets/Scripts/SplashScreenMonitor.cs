using System.Collections;

using UnityEngine;
using UnityEngine.Rendering;

public class SplashScreenMonitor : MonoBehaviour
{
    [SerializeField] FadeOutImage fader;

    IEnumerator Start()
    {
        while (!SplashScreen.isFinished)
        {
            yield return null;
        }

        fader.Go();
    }
}