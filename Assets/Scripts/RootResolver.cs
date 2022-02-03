using UnityEngine;

public class RootResolver : MonoBehaviour
{
    [SerializeField] GameObject root;

    public GameObject Root { get { return root; } }
}