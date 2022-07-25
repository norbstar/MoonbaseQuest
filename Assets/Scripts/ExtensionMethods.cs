using System.Collections.Generic;

using UnityEngine;

// Alias used to differentiate from System.Random;
using UnityRandom = UnityEngine.Random;

namespace ExtensionMethods
{
    public static class AudioExtensions
    {
        public static Object Random(this List<Object> list) => list[UnityRandom.Range(0, list.Count)];

        public static AudioClip Random(this List<AudioClip> list) => list[UnityRandom.Range(0, list.Count)];

        public static void PlayClip(this AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }
    }
}