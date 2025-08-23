using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endingCheck : MonoBehaviour
{
    private globalReferences _globalReferences;

    // Start is called before the first frame update
    void Start()
    {
        _globalReferences = GameObject.Find("GlobalReferences").GetComponent<globalReferences>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 && _globalReferences.generatorActivated >= _globalReferences.generatorCounter)
        {
            _globalReferences.playerWon = true;
            _globalReferences.FadeinOutAnimator.SetTrigger("Fadeout");
            Debug.Log("You Won");
            StartCoroutine(GameWin());
        }
        else if (other.gameObject.layer == 10)
        {
            _globalReferences.GeneratorCanvasAnimator.SetTrigger("Remind");
            Debug.Log("You are missing generators");
        }
    }

    IEnumerator GameWin()
    {
        _globalReferences.FadeinOutAnimator.SetTrigger("Fadeout");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(2);
    }
}
