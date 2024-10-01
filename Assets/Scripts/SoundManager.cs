using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource[] matchSound;//match sound sesi ayarlanýyo 
    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                if (backgroundMusic != null && !backgroundMusic.isPlaying)
                {
                    backgroundMusic.loop = true;
                    backgroundMusic.Play();
                }

            }
        }
        else
        {
            if (backgroundMusic != null && !backgroundMusic.isPlaying)
            {
                backgroundMusic.loop = true;
                backgroundMusic.Play();
            }
        }
    }

    public void PlaySoundEffect()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                int clipToPlay = Random.Range(0, matchSound.Length);
                matchSound[clipToPlay].Play();
               
            }
        }
        else
        {
            int clipToPlay = Random.Range(0, matchSound.Length);
            matchSound[clipToPlay].Play();

        }
    }
}
