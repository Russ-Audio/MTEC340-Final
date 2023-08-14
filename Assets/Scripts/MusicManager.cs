using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource offCombatMusic;
    public AudioSource inCombatMusic;
    public AudioSource ambience;

    public float fadeDuration = 3.0f;

    private float timer;

    public GameBehavior gameBehavior;

    // Start is called before the first frame update
    void Awake()
    {
        gameBehavior = GameObject.Find("GameBehavior").GetComponent<GameBehavior>();
    }

    private void Start()
    {
        offCombatMusic.Play();
        inCombatMusic.Play();
        ambience.Play();

        offCombatMusic.volume = 0.0f;
        inCombatMusic.volume = 0.0f;
        ambience.volume = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameBehavior.isPaused == true)
        {
            offCombatMusic.mute = true;
            inCombatMusic.mute = true;
            ambience.mute = true;
        }
        else
        {
            offCombatMusic.mute = false;
            inCombatMusic.mute = false;
            ambience.mute = false;
        }

        if(gameBehavior.doorOpen.Open == true && gameBehavior.PlayerDead == false)
        {
            //Fade in music 2
            timer = 0f;

            if(timer < fadeDuration && inCombatMusic.volume < 0.95f)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / fadeDuration;
                float targetVolumeFadeIn = Mathf.Lerp(inCombatMusic.volume, 1.0f, normalizedTime);
                inCombatMusic.volume = targetVolumeFadeIn;
            }
        }

        if(gameBehavior.PlayerDead == true)
        {
            //Fade out music 1
            //Fade out music 2
            timer = 0f;

            if(timer < (fadeDuration - 1.2f))
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / (fadeDuration - 1.2f);
                float targetVolumeFadeOut = Mathf.Lerp(inCombatMusic.volume, 0.0f, normalizedTime);
                offCombatMusic.volume = targetVolumeFadeOut;
                inCombatMusic.volume = targetVolumeFadeOut;
                ambience.volume = targetVolumeFadeOut;
            }

        }
        else
        {
            //Fade in music 1
            timer = 0f;

            if(timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / fadeDuration;
                float targetVolumeFadeIn = Mathf.Lerp(offCombatMusic.volume, 1.0f, normalizedTime);
                offCombatMusic.volume = targetVolumeFadeIn;
                ambience.volume = targetVolumeFadeIn;
            }
        }
    }
}
