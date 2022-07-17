using UnityEngine;

public class PlayAudioAtPoint : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip audioClip;

    // Start is called before the first frame update
    void Start() => AudioSource.PlayClipAtPoint(audioClip, Vector3.zero, 1.0f);
}