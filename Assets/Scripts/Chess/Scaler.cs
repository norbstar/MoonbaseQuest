using UnityEngine;

namespace Chess
{
    public class Scaler : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] [Range(0f, 100f)] float scale = 1f;

        // Update is called once per frame
        void Update()
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}