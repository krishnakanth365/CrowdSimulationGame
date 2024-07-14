using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float gravity = -9.81f;

    CharacterController characterController;

    private Vector3 velocity;

    private void Awake()
    {
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
    }
}
