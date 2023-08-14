using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    private float currentHeight;
    private float targetHeight;

    public float animationSpeed = 1.0f;
    public float startingY = 1.11f;

    public AudioSource audioSource;

    public bool Open = false;

    private void Start()
    {
        currentHeight = startingY;
        targetHeight = startingY + 2.0f;
    }

    private void Update()
    {
        if (Open)
        {
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, animationSpeed * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, currentHeight, transform.localPosition.z);
            //Play DoorOpenSoundOneShot
            //Debug.Log(currentHeight);
        }
    }

    public void DoorOpen()
    {
        Open = true;
        audioSource.Play();
    }
}
