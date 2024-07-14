using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LeftMenu : UI_Element
{
    public GameObject LeftSubmenu;
    public Image ImageAffected01;
    public Image ImageAffected02;
    public Image ImageAffected03;
    public Image ImageAffected04;
    private IEnumerator coroutine;
    public override void Click()
    {
        if (LeftSubmenu != null)
        {
            if (LeftSubmenu.activeSelf)
            {
                coroutine = MenuDisable(0.4f);
                StartCoroutine(coroutine);
            }
            else
            {
                coroutine = MenuEnable(0.4f);
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator MenuDisable(float waitTime)
    {
        ImageAffected01.CrossFadeAlpha(0f, 0.15f, ignoreTimeScale: false);
        ImageAffected02.CrossFadeAlpha(0f, 0.15f, ignoreTimeScale: false);
        ImageAffected03.CrossFadeAlpha(0f, 0.15f, ignoreTimeScale: false);
        ImageAffected04.CrossFadeAlpha(0f, 0.15f, ignoreTimeScale: false);
        yield return new WaitForSeconds(waitTime);
        LeftSubmenu.SetActive(value: false);
    }

    private IEnumerator MenuEnable(float waitTime)
    {
        LeftSubmenu.SetActive(value: true);
        ImageAffected01.CrossFadeAlpha(1f, 0.15f, ignoreTimeScale: false);
        ImageAffected02.CrossFadeAlpha(1f, 0.15f, ignoreTimeScale: false);
        ImageAffected03.CrossFadeAlpha(1f, 0.15f, ignoreTimeScale: false);
        ImageAffected04.CrossFadeAlpha(1f, 0.15f, ignoreTimeScale: false);
        yield return new WaitForSeconds(waitTime);
    }



}
