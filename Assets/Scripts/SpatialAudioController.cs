using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SpatialAudioController : MonoBehaviour
{
    [Serializable]
    public class SpatialAudio
    {
        public AudioClip clip;
        public Vector3 position;
    }

    [SerializeField] List<SpatialAudio> items;
    [SerializeField] bool loop = true;

    private int index;

    // Start is called before the first frame update
    void Start() => StartCoroutine(TestSpatialAudioCoroutine());

    private IEnumerator TestSpatialAudioCoroutine()
    {
        yield return new WaitForSeconds(1f);

        if ((items != null) && (items.Count > 0))
        {
            PlayAudio(index);
        }
    }

    private void PlayNextClip()
    {
        if (index + 1 < items.Count)
        {
            ++index;
            PlayAudio(index);
        }
        else if (loop)
        {
            index = 0;
            PlayAudio(index);
        }
    }

    private void PlayAudio(int index)
    {
        SpatialAudio item = items[index];
        StartCoroutine(PlayClipCoroutine(item));
    }

    private IEnumerator PlayClipCoroutine(SpatialAudio audio)
    {
        AudioSource.PlayClipAtPoint(audio.clip, audio.position, 1.0f);

        float length = audio.clip.length;
        yield return new WaitForSeconds(length);

        PlayNextClip();
    }
}