using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Timmer : MonoBehaviour
{
    [SerializeField] TMP_Text timmerText;
    [SerializeField] float timmer = 60f;

    private void Awake()
    {
        timmerText.text = timmer.ToString();
    }

    private void Start()
    {
        StartCoroutine(TimmerRoutine());
    }

    IEnumerator TimmerRoutine()
    {
        while (timmer >= 0)
        {
            yield return new WaitForSeconds(1f);
            timmer -= 1f;
            timmerText.text = "Time Left :"+ timmer.ToString();
        }
        GameManager.instance.GameLostScreen();
    }
}
