using UnityEngine;
using UnityEngine.XR;
// using UnityEngine.XR.Interaction.Toolkit;

using static Enum.ControllerEnums;

namespace Chess
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PinchInteractable : /*XRBaseInteractable*/MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] OnTriggerHandler hoverZone;

        [Header("Controllers")]
        [SerializeField] HandController leftHandController;
        [SerializeField] HandController rightHandController;

        [Header("Config")]
        [SerializeField] Transform leftHandAttachTransform;
        [SerializeField] Transform rightHandAttachTransform;

        // private bool leftHandIsHovering = false;
        // private bool rightHandIsHovering = false;

        private MeshCollider meshCollider;
        private SphereCollider sphereCollider;
        private new Rigidbody rigidbody;
        private InputDeviceCharacteristics? isHoveringCharacteristics;
        private InputDeviceCharacteristics? isHolldingCharacteristics;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            meshCollider = GetComponent<MeshCollider>() as MeshCollider;
            sphereCollider = GetComponent<SphereCollider>() as SphereCollider;
            rigidbody = GetComponent<Rigidbody>() as Rigidbody;
        }

        // protected override void OnHoverEntered(HoverEnterEventArgs args)
        // {
        //     Debug.Log("OnHoverEntered");

        //     base.OnHoverEntered(args);

        //     isHovering = true;
            
        //     var interactor = args.interactorObject.transform.gameObject;

        //     if (TryGet.TryGetXRController(interactor, out XRController xrController))
        //     {
        //         // TODO
        //     }
        // }

        // protected override void OnSelectEntered(SelectEnterEventArgs args)
        // {
        //     Debug.Log("OnSelectEntered");

        //     base.OnSelectEntered(args);
            
        //     var interactor = args.interactorObject.transform.gameObject;

        //     if (TryGet.TryGetXRController(interactor, out XRController xrController))
        //     {
        //         // TODO
        //     }
        // }

        // protected override void OnHoverExited(HoverExitEventArgs args)
        // {
        //     Debug.Log("OnHoverExited");

        //     base.OnHoverExited(args);
            
        //     isHovering = false;

        //     var interactor = args.interactorObject.transform.gameObject;

        //     if (TryGet.TryGetXRController(interactor, out XRController xrController))
        //     {
        //         // TODO
        //     }
        // }

        // protected override void OnSelectExited(SelectExitEventArgs args)
        // {
        //     Debug.Log("OnSelectExited");

        //     base.OnSelectExited(args);
            
        //     var interactor = args.interactorObject.transform.gameObject;

        //     if (TryGet.TryGetXRController(interactor, out XRController xrController))
        //     {
        //         // TODO
        //     }
        // }

        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //     HandController.ActuationEventReceived += OnActuation;
        // }

        void OnEnable()
        {
            hoverZone.EventReceived += OnHover;
            HandController.ActuationEventReceived += OnActuation;
        }

        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        //     HandController.ActuationEventReceived -= OnActuation;
        // }

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
                    
                    // if ((int) characteristics == (int) HandController.LeftHand)
                    // {
                    //     Debug.Log("OnTriggerEnter Left Hand Collider : {rootGameObject.name}");
                    //     leftHandIsHovering = true;
                    // }
                    // else if ((int) characteristics == (int) HandController.RightHand)
                    // {
                    //     Debug.Log("OnTriggerEnter Right Hand Collider : {rootGameObject.name}");
                    //     rightHandIsHovering = true;
                    // }
                    
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
                    
                    // if ((int) characteristics == (int) HandController.LeftHand)
                    // {
                    //     Debug.Log("OnTriggerExit Left Hand Collider : {rootGameObject.name}");
                    //     leftHandIsHovering = false;
                    // }
                    // else if ((int) characteristics == (int) HandController.RightHand)
                    // {
                    //     Debug.Log("OnTriggerExit Right Hand Collider : {rootGameObject.name}");
                    //     rightHandIsHovering = false;
                    // }

                    isHoveringCharacteristics = null;
                }
            }
        }

        private void OnHover(OnTriggerHandler.EventType type, GameObject gameObject)
        {
            Debug.Log($"OnHover Type : {type}");

            switch (type)
            {
                case OnTriggerHandler.EventType.OnTriggerEnter:
                    break;

                case OnTriggerHandler.EventType.OnTriggerExit:
                    break;
            }
        }

        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            Debug.Log($"OnActuation Actuation : {actuation}");

            if (actuation.HasFlag(Actuation.Trigger))
            {
                // if ((int) characteristics == (int) HandController.LeftHand && leftHandIsHovering)
                // {
                //     PinchObject(characteristics);
                // }

                if (characteristics == isHoveringCharacteristics)
                {
                    PickupObject(characteristics);
                }
            }
            else
            {
                // if ((int) characteristics == (int) HandController.LeftHand && leftHandIsHovering)
                // {
                //     ReleaseObject();
                // }

                if (characteristics == isHoveringCharacteristics)
                {
                    DropObject();
                }
            }
        }

        // private void PickupObject(InputDeviceCharacteristics characteristics)
        // {
        //     if ((int) characteristics == (int) HandController.LeftHand)
        //     {
        //         Debug.Log("OnTrigger Left Hand Is Hovering : {leftHandIsHovering}");
        //     }
        //     else if ((int) characteristics == (int) HandController.RightHand)
        //     {
        //         Debug.Log("OnTrigger Left Hand Is Hovering : {rightHandIsHovering}");
        //     }
        // }

        private void PickupObject(InputDeviceCharacteristics characteristics)
        {
            Debug.Log("PickupObject");
            
            rigidbody.isKinematic = true;
            meshCollider.isTrigger = true;
            sphereCollider.enabled = false;
            isHolldingCharacteristics = characteristics;
        }

        private void DropObject()
        {
            Debug.Log("DropObject");

            sphereCollider.enabled = true;
            meshCollider.isTrigger = false;
            rigidbody.isKinematic = false;
            isHolldingCharacteristics = null;
        }
    }
}