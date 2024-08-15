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

    public static int strikes = 0;
    [SerializeField] TMP_Text strikesText;

    public static PlayerManager instance;

    public Action OnStrike;

    private void Awake()
    {
        instance = this;
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                Investigate(hitInfo.collider.gameObject);
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CatchTheif(hitInfo.collider.gameObject);
            }
        }
        else
        {
            uiButtons.SetActive(false);
        }
    }

    public void Investigate(GameObject npc)
    {
        UI_Hints.instance.RevealHint();
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
