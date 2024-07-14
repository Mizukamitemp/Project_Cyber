using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    public Image splashImage;
    public string loadLevel;

    IEnumerator Start()
    {
        splashImage.canvasRenderer.SetAlpha(0.0f);

        FadeIn();
        yield return new WaitForSeconds(2.5f);
        //FadeOut();
        //yield return new WaitForSeconds(2.5f);
        //SceneManager.LoadScene(loadLevel);
    }

    public void PlayTechDemoA()
    {
        SceneManager.LoadScene("_techDemoONE-A");
    }

    public void PlayTechDemoB()
    {
        SceneManager.LoadScene("_techDemoONE-B");
    }

    public void PlayTechDemoC()
    {
        SceneManager.LoadScene("_techDemoONE-C");
    }

    void FadeIn()
    {
        splashImage.CrossFadeAlpha(1.0f, 1.5f, false);

    }

    void FadeOut()
    {
        splashImage.CrossFadeAlpha(0.0f, 2.5f, false);
    }
}
