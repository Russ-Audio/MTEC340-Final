using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cam;
    public Vector3 camOffset = Vector3.zero;

    public Animator camAnim;

    private Vector3 initialPosition;

    private int FootIndex;
    private int LastIndex = -1;

    [SerializeField] AudioClip[] FootstepSFX;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = cam.localPosition;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = initialPosition + camOffset;
        cam.localPosition = Vector3.Slerp(cam.localPosition, targetPosition, 4.0f * Time.deltaTime);
    }

    public void StartWalking()
    {
        camAnim.SetBool("isMoving", true);
        //Debug.Log("CALLED");
    }

    public void StopWalking()
    {
        camAnim.SetBool("isMoving", false);
        camOffset = Vector3.zero;
    }

    public void PlayFootstep()
    {
        //Debug.Log("THOMP");
        //PlayOneShot Footstep Sound
        
        do
        {
            FootIndex = Random.Range(0, 4);
        }
        while (FootIndex == LastIndex);
        

        //FootIndex = Random.Range(0, 4);

        float randPitch = Random.Range(0.95f, 1.1f);
        float randVol = Random.Range(0.6f, 0.8f);
        audioSource.pitch = randPitch;
        audioSource.volume = randVol;
        audioSource.PlayOneShot(FootstepSFX[FootIndex], 0.4f);
        LastIndex = FootIndex;
    }
}
