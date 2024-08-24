using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
#if UNITY_EDITOR
using UnityEditor.VersionControl;
#endif
using System.Drawing;
using UnityEngine.U2D;
using Color = UnityEngine.Color;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] Image conversationBox;
    [SerializeField] Image portrait;
    [SerializeField] Image otherPortrait;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text otherNameText;

    public static ConversationManager instance;
    public bool dialogueIsPlaying = false;

    private Sprite portraitA;
    private Sprite portraitB;

    private String nameOfPortraitA;
    private String nameOfPortraitB;

    private float defaultZoomValue;

    public Action OnDialogeFunctionCalled;
    public Action OnDialogeExit;

    private Coroutine currentTypeWriter;

    private int conversationIndex = 0;
    private string[] currentConverstaion;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    public void EnterDialogueMode(String[] converstaion, Sprite portraitA, string nameOfA, Sprite portraitB, string nameOfB)
    {
        if (dialogueIsPlaying)
        {
            return;
        }
        InitailizePortraits();
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        dialogueText.text = converstaion[conversationIndex];
        currentConverstaion = converstaion;
        this.portraitA = portraitA;
        this.portraitB = portraitB;
        this.nameOfPortraitA = nameOfA;
        this.nameOfPortraitB = nameOfB;
        nameText.text = nameOfPortraitA;
        otherNameText.text = nameOfPortraitB;
        SwitchPortrait(portraitA, nameOfA);
        DisplayText(converstaion[conversationIndex]);
        conversationIndex++;
        InputManager.instance.OnInteractPressed += ContinueStory;
        //StartCoroutine(CheckForPlayer());
    }

    private void DisplayText(string line)
    {
        dialogueText.text = line;
    }

    public void ExitDialogueMode()
    {
        InputManager.instance.OnInteractPressed -= ContinueStory;
        OnDialogeExit?.Invoke();
        conversationIndex = 0;
        dialogueIsPlaying = false;
        if (dialoguePanel != null)
        {
            try
            {
                dialoguePanel.SetActive(false);
            }
            catch { }
        }
        dialogueText.text = "";

        //StopCoroutine(CheckForPlayer());
    }


    public void ContinueStory()
    {
        if (conversationIndex >= currentConverstaion.Length)
        {
            ExitDialogueMode();
            return;
        }
        if (conversationIndex % 2 == 0)
        {
            SwitchPortrait(portraitA, nameOfPortraitA);
            DisplayText(currentConverstaion[conversationIndex]);
        }
        else
        {
            SwitchPortrait(portraitB, nameOfPortraitB);
            DisplayText(currentConverstaion[conversationIndex]);
        }
        conversationIndex++;
    }
    private void InitailizePortraits()
    {
        portrait.sprite = portraitA;
        otherPortrait.sprite = portraitB;
    }

    IEnumerator CheckForPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (Vector3.Distance(transform.position, PlayerManager.instance.transform.position) > 5)
            {
                Debug.Log(PlayerManager.instance.gameObject);
                //ExitDialogueMode();
                //StopCoroutine(CheckForPlayer());
                break;
            }

            yield return null;
        }
        yield return null;
    }

    private void SwitchPortrait(Sprite narratorPortrait, string nameOfPortrait)
    {
        InitailizePortraits();
        if (narratorPortrait == portraitA)
        {
            portrait.color = new Color(1, 1, 1, 0.4f);
            otherNameText.fontSize = 30f;
            nameText.fontSize = 46.3f;
            otherPortrait.color = new Color(1, 1, 1, 1f);
        }
        else
        {
            portrait.color = new Color(1, 1, 1, 1f);
            otherNameText.fontSize = 46.3f;
            nameText.fontSize = 30;
            otherPortrait.color = new Color(1, 1, 1, .4f);
        }
    }
}
