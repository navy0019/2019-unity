using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingEvent : MonoBehaviour
{
    public Animator animator;
    public GameObject LoseUI;
    public GameObject loadingBar;
    public Text loadText;
    public GameObject loadScene;
    public GameObject blackScene;
    public int SceneNum;
    public List<GameObject> switchObject;
    AsyncOperation async;

    private void Start()
    {

    }

    private void Update()
    {
        if (LoseUI.activeInHierarchy == true)
        {
            SceneNum = 1;
        }
    }

    public void StartLoading()
    {
        loadingBar.transform.localScale = new Vector3(0, 1, 1);
        loadScene.SetActive(true);
        //mainUI.SetActive(false);
        SwitchObjects();
        Invoke("CoroutineStart", 1f);
        //StartCoroutine(LoadScreen(SceneNum));
    }

    IEnumerator LoadScreen(int num)
    {
        int displayProgress = 0;
        int toProgress = 0;

        async = SceneManager.LoadSceneAsync(num);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            toProgress = (int)async.progress * 100;
            while (displayProgress < toProgress)
            {
                displayProgress++;
                SetLoading(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            displayProgress++;
            SetLoading(displayProgress);

            yield return new WaitForEndOfFrame();
        }
        if (toProgress == 100)
        {
            animator.SetTrigger("change");
        }


    }
    public void CoroutineStart()
    {
        StartCoroutine(LoadScreen(SceneNum));
    }
    public void ChangeScene()
    {
        async.allowSceneActivation = true;
    }
    private void SetLoading(float num)
    {
        loadingBar.transform.localScale = new Vector3((num * 0.01f), 1, 1);
        loadText.text = num.ToString() + " %";
    }
    public void SwitchObjects()
    {
        for (int i = 0; i < switchObject.Count; i++)
        {
            if (!switchObject[i].activeSelf)
            {
                switchObject[i].SetActive(true);
            }
            else
            {
                switchObject[i].SetActive(false);
            }
        }
    }
}
