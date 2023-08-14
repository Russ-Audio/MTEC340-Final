using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController _controller;
    public MoveCamera moveCamera;

    public float _speed = 6f;

    Vector3 velocity;
    public float _gravity = -9.8f;
    public float _jumpHeight = 3f;

    public Transform groundCheck;
    public float _groundDistance = 0.4f;
    public LayerMask groundMask;

    private AudioSource audioSource;

    bool isGrounded;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);


        if (isGrounded && velocity.y <= 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * x + transform.forward * z;

        movement = Vector3.ClampMagnitude(movement, _speed);

        float horizontalMagnitude = movement.magnitude;

        _controller.Move(movement * _speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        velocity.y += _gravity * Time.deltaTime;

        _controller.Move(velocity * Time.deltaTime);

        if(horizontalMagnitude != 0 && isGrounded)
        {
            moveCamera.StartWalking();
            //Debug.Log("1");
        }
        else
        {
            moveCamera.StopWalking();
            //Debug.Log("2");
        }
    
    }
}
