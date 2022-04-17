using System.Collections;

using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(FX.ScaleFX))]
    public class StartCubeManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] float fromScale;
        [SerializeField] float toScale;

        private FX.ScaleFX scaleFX;
        private Coroutine coroutine;

        // Start is called before the first frame update
        void OnStart()
        {
            ResolveDependencies();
        }

        void OnEnable()
        {
            coroutine = StartCoroutine(scaleFX.Apply(new FX.ScaleFX.Config
            {
                fromValue = fromScale,
                toValue = toScale,
                applyToZAxis = true
            }));
        }

        void OnDisable() => StopCoroutine(coroutine);

        private void ResolveDependencies()
        {
            scaleFX = GetComponent<FX.ScaleFX>() as FX.ScaleFX;
        }
    }
}