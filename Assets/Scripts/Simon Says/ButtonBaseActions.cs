using UnityEngine;

namespace SimonSays
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ButtonBaseActions : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] Material onMaterial;
        [SerializeField] Material offMaterial;

        [Header("Audio")]
        [SerializeField] AudioClip clickClip;

        private new MeshRenderer renderer;
        private bool isOn = false;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
        }

        public void OnFlipButton()
        {
            isOn = !isOn;
            AudioSource.PlayClipAtPoint(clickClip, transform.position, 1.0f);
            renderer.material = (isOn) ? onMaterial : offMaterial;
        }
    }
}