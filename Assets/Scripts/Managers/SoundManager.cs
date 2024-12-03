using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundObject;
    public static SoundManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void PlaySound(AudioClip audioClip, Transform spawnTransform, float volume, bool looping)
    {
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = looping;
        audioSource.pitch = Random.Range(0.9f, 1.2f);
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
