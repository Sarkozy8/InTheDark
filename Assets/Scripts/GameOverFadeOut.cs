using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverFadeOut : MonoBehaviour
{
    public Animator animatorFade;
    public Animator animatorFadeText;


    void Start()
    {
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(15);
        animatorFade.SetTrigger("Fadeout");
        animatorFadeText.SetTrigger("Fadeout");

        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(0);
    }
}
