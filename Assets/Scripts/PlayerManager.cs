using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject uiButtons;
    [SerializeField] LayerMask npcLayer;

    [SerializeField] GameObject[] stars;

    [SerializeField] float speed = 3f;
    [SerializeField] float gravity = -9.81f;

    CharacterController characterController;

    private Vector3 velocity;

    public int strikes = 0;
    [SerializeField] TMP_Text strikesText;

    public static PlayerManager instance;

    public Action OnStrike;

    [SerializeField] Sprite myPortrait;
    [SerializeField] Sprite[] npcPortraits;

    [SerializeField] string[] conversations;
    private Flock prevFlock;

    private void Awake()
    {
        instance = this;
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        if (characterController.isGrounded)
        {
            velocity = Vector3.zero;
        }
        characterController.Move(velocity * Time.deltaTime);


        if (Physics.Raycast(transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, 2f, npcLayer))
        {
            uiButtons.SetActive(true);
            if (!ConversationManager.instance.dialogueIsPlaying)
            {
                if (!hitInfo.collider.gameObject.GetComponent<Flock>().isTheif)
                {
                    if (!hitInfo.collider.gameObject.GetComponent<Flock>().isTheif)
                    {
                        if (prevFlock != null)
                        {
                            prevFlock.ResumeFlock();
                        }
                        prevFlock = hitInfo.collider.gameObject.GetComponent<Flock>();
                        prevFlock.PauseFlock();
                    }
                }
                else
                {
                    hitInfo.collider.gameObject.GetComponent<Flock>().PauseFlock(1f);
                    float maxSpeed = hitInfo.collider.gameObject.GetComponent<Flock>().flockParameters.speed;
                    maxSpeed += 0.2f;
                    maxSpeed = Mathf.Clamp(maxSpeed, 1f, 5f);
                    hitInfo.collider.gameObject.GetComponent<Flock>().flockParameters.speed = maxSpeed;
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hitInfo.collider.gameObject.GetComponent<Flock>().isTheif && 
                    !hitInfo.collider.gameObject.GetComponent<Flock>().hasInteracted)
                {
                    Investigate(hitInfo.collider.gameObject);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (ConversationManager.instance.dialogueIsPlaying)
                {
                    return;
                }
                CatchTheif(hitInfo.collider.gameObject);
            }
        }
        else
        {
            uiButtons.SetActive(false);
        }
    }

    private bool canInvestigate = true;

    IEnumerator ResetInvestigate()
    {
        canInvestigate = false;
        yield return new WaitForSeconds(0.2f);
        canInvestigate = true;
    }

    public void Investigate(GameObject npc)
    {
        if (!canInvestigate) return;

        if (UI_Hints.count >= 3)
        {
            return;
        }

        if (ConversationManager.instance.dialogueIsPlaying)
        {
            return;
        }
        Flock npcFlock = npc.GetComponent<Flock>();
        ConversationManager.instance.EnterDialogueMode(conversations, npcFlock.face, "Inspector Krishna", myPortrait, npcFlock.myName);
        ConversationManager.instance.OnDialogeExit += AfterConversation;
        ConversationManager.instance.OnDialogeExit += ()=>
        {
            npc.GetComponent<Flock>().ResumeFlock();
            npc.GetComponent<Flock>().hasInteracted = true;
        };
    }

    private void AfterConversation()
    {
        UI_Hints.instance.RevealHint();
        StartCoroutine(ResetInvestigate());
        ConversationManager.instance.OnDialogeExit -= AfterConversation;
    }

    public void CatchTheif(GameObject npc)
    {
        if (npc.GetComponent<Theif>())
        {
            GameManager.instance.GameWonScreen(strikes);
        }
        else
        {
            OnStrike?.Invoke();
            strikes++;
            strikesText.text = "Strikes : " + strikes.ToString();
            if (strikes >= 3)
            {
                GameManager.instance.GameLostScreen();
            }
        }
    }
}
