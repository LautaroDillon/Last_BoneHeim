using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound Library")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;


    [Header("Filters")]
    public AudioLowPassFilter lowPassFilter;

    private void Awake()
    {
        lowPassFilter = musicSource.GetComponent<AudioLowPassFilter>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        PlayMusic("Background Music", 1f);
    }

    #region Audio Tech
    public void PlayMusic(string name, float volumen)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Music Not Found!");
        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = volumen;
            musicSource.Play();
            Debug.Log("Playing " + s.clip);
        }
    }

    public void PauseMenuMusic()
    {
        StopAllCoroutines();
        StartCoroutine(LerpMusicEffects(0.5f, 0.9f, 5000f));
    }

    public void UnPauseMenuMusic()
    {
        StopAllCoroutines();
        StartCoroutine(LerpMusicEffects(1f, 1f, 22000f));
    }

    IEnumerator LerpMusicEffects(float duration, float targetPitch, float targetCutoff)
    {
        float startPitch = musicSource.pitch;
        float startCutoff = lowPassFilter.cutoffFrequency;

        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            musicSource.pitch = Mathf.Lerp(startPitch, targetPitch, t);
            lowPassFilter.cutoffFrequency = Mathf.Lerp(startCutoff, targetCutoff, t);

            yield return null;
        }

        musicSource.pitch = targetPitch;
        lowPassFilter.cutoffFrequency = targetCutoff;
    }

    public void StopMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Music Not Found!");
            return;
        }

        if (musicSource.clip == s.clip && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("Stopping " + s.clip);
        }
        else
        {
            Debug.Log("Music is not currently playing: " + s.clip);
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

    public void PauseSFX()
    {
        if (sfxSource != null)
            sfxSource.Pause();
    }

    public void UnPauseSFX()
    {
        if (sfxSource != null)
            sfxSource.Play();
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
    #endregion

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }
}
