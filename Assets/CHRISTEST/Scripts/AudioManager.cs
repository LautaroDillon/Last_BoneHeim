using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PlayMusic("BackgroundMusic", 0.4f);
    }

    public void PlayMusic(string name, float volumen)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Music Not Found!");
        else
        {
            musicSource.clip = s.clip;
            sfxSource.volume = volumen;
            musicSource.Play();
        }
    }

    public void StopMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Music Not Found!");
        else
        {
            musicSource.clip = s.clip;
            musicSource.Stop();
        }
    }

    public void PlaySFX(string name, float volumen, bool loop)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Sound Not Found!");
        else
        {
            if(loop == true)
            {
                sfxSource.loop = true;
            }
            sfxSource.clip = s.clip;
            sfxSource.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
            sfxSource.volume = volumen;
            sfxSource.Play();
        }
    }

    public void StopSFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Sound Not Found!");
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.Stop();
        }
    }
    public void PlaySFXOneShot(string name, float volumen)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
            Debug.Log("One Shot Sound Not Found!");
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
            sfxSource.volume = volumen;
            sfxSource.PlayOneShot(s.clip, 1f);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }
}
