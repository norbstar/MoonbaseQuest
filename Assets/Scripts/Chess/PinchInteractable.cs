using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

namespace Chess
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PinchInteractable : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] OnPrimitiveTriggerHandler hoverZone;

        [Header("Controllers")]
        [SerializeField] HandController leftHandController;
        [SerializeField] HandController rightHandController;

        [Header("Config")]
        [SerializeField] Transform leftHandAttachTransform;
        [SerializeField] Transform rightHandAttachTransform;

        private MeshCollider meshCollider;
        private BoxCollider boxCollider;
        private new Rigidbody rigidbody;
        private InputDeviceCharacteristics? isHoveringCharacteristics;
        private InputDeviceCharacteristics? isHolldingCharacteristics;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            meshCollider = GetComponent<MeshCollider>() as MeshCollider;
            boxCollider = GetComponent<BoxCollider>() as BoxCollider;
            rigidbody = GetComponent<Rigidbody>() as Rigidbody;
        }

        void OnEnable()
        {
            hoverZone.EventReceived += OnHover;
            HandController.ActuationEventReceived += OnActuation;
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
        }

        void Update()
        {
            if (!isHolldingCharacteristics.HasValue) return;

            if ((int) isHolldingCharacteristics == (int) HandController.LeftHand)
            {
                transform.position = leftHandController.transform.position + leftHandAttachTransform.localPosition;
            }
            else if ((int) isHolldingCharacteristics == (int) HandController.RightHand)
            {
                transform.position = rightHandController.transform.position + rightHandAttachTransform.localPosition;
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            var trigger = collider.gameObject;
            
            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Hand"))
                {
                    InputDeviceCharacteristics characteristics = rootGameObject.GetComponent<HybridHandController>().Characteristics;
                    isHoveringCharacteristics = characteristics;
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger = collider.gameObject;

            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Hand"))
                {
                    InputDeviceCharacteristics characteristics = rootGameObject.GetComponent<HybridHandController>().Characteristics;
                    isHoveringCharacteristics = null;
                }
            }
        }

        private void OnHover(OnPrimitiveTriggerHandler.EventType type, GameObject gameObject)
        {
            Debug.Log($"OnHover Game Object : {gameObject.name} Type : {type}");

            switch (type)
            {
                case OnPrimitiveTriggerHandler.EventType.OnTriggerEnter:
                    break;

                case OnPrimitiveTriggerHandler.EventType.OnTriggerExit:
                    break;
            }
        }

        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            Debug.Log($"OnActuation Actuation : {actuation}");

            if (actuation.HasFlag(Actuation.Trigger))
            {
                if (characteristics == isHoveringCharacteristics)
                {
                    PickupObject(characteristics);
                }
            }
            else
            {
                if (characteristics == isHoveringCharacteristics)
                {
                    DropObject();
                }
            }
        }

        private void PickupObject(InputDeviceCharacteristics characteristics)
        {
            Debug.Log("PickupObject");
            
            rigidbody.isKinematic = true;
            meshCollider.enabled = false;
            boxCollider.enabled = true;
            isHolldingCharacteristics = characteristics;
        }

        private void DropObject()
        {
            Debug.Log("DropObject");

            boxCollider.enabled = false;
            meshCollider.enabled = true;
            rigidbody.isKinematic = false;
            isHolldingCharacteristics = null;
        }
    }
}