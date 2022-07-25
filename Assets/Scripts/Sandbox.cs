// using System;
using System.Collections.Generic;

// A static directive
// using static System.Random;

// using ExtensionMethods;
using static ExtensionMethods.AudioExtensions;

using UnityEngine;

// Alias used to differentiate from System.Random;
// using UnityRandom = UnityEngine.Random;

public class Sandbox
{
    [SerializeField] AudioSource _audioSource;

    [SerializeField] List<AudioClip> _sounds;

    // Alternate version of PlaySounds that utilises extension methods on the AudioSource class
    // and the array of sounds
    // public void PlaySound() => _audioSource.PlayClip(_sounds[0]);
    public void PlaySound() => _audioSource.PlayClip(_sounds.Random());

    // public void PlaySound()
    // {
    //     int random = UnityRandom.Range(0, _sounds.Count);
    //     _audioSource.clip = _sounds[random];
    //     _audioSource.Play();
    // }
}