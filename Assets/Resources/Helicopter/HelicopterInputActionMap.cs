//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Resources/Helicopter/HelicopterInputActionMap.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @HelicopterInputActionMap : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @HelicopterInputActionMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""HelicopterInputActionMap"",
    ""maps"": [
        {
            ""name"": ""Helicopter"",
            ""id"": ""c6901008-8124-4c03-9f0b-8c62a08edd09"",
            ""actions"": [
                {
                    ""name"": ""Engage Power"",
                    ""type"": ""Button"",
                    ""id"": ""4199a8e6-5331-46e9-8a9b-b29c6127dd76"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Cut Power"",
                    ""type"": ""Button"",
                    ""id"": ""f1fd022f-9ab2-4e83-a0cb-9d3a65bf96c2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Stabilise Elevation"",
                    ""type"": ""Button"",
                    ""id"": ""8e48efa2-7cdb-4504-99e2-5d6fbff2667c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Stabilise Descent"",
                    ""type"": ""Button"",
                    ""id"": ""cd3f74ca-d753-4b5e-a519-5fd601632ca9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotar Speed"",
                    ""type"": ""Value"",
                    ""id"": ""151d71f4-a115-4ad6-a262-ee1a8c0a9d4d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Heading"",
                    ""type"": ""Value"",
                    ""id"": ""1cbb8aec-f601-4e9c-a348-fddd54ff36bd"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Value"",
                    ""id"": ""dbdca0c9-01fb-4c54-9854-c21c5e52ae07"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Banking"",
                    ""type"": ""Value"",
                    ""id"": ""377a6a71-0184-4668-8312-89b3abdaf315"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""30376e1c-197f-4317-adf5-51e524077ffc"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Engage Power"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fde360ca-0a28-43b4-83a8-67acc34180d6"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cut Power"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ce8aac55-33aa-4896-a721-3c3c47ee58c8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotar Speed"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""c07e9c81-1e7c-40c2-946b-1463a79c8664"",
                    ""path"": ""<Keyboard>/rightBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotar Speed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""03eef34e-02ab-44ff-9af4-e5a8eaaa9e08"",
                    ""path"": ""<Keyboard>/leftBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotar Speed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""d3a1a093-6068-44b6-ba5c-6b190fb91473"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Heading"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""51c124cc-a077-4eef-85bd-5fee1eac8cf4"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Heading"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9bac0cf5-b6c7-4902-ac0e-cbd68e13043f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Heading"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""97377ce7-e5c6-4f9d-a026-77448ad28454"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""54cea3bc-7f9f-485d-80a3-654f05fcf273"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9986db1a-d963-4183-bceb-4f2a4ae492cc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""7c9e2f4a-399b-4f01-94b5-77938d58235c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Banking"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""3c86d52e-fd2c-43cc-8e4d-84ec3ef0b18a"",
                    ""path"": ""<Keyboard>/rightShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Banking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""482f7278-9085-4421-95bf-105fbb1c3cec"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Banking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""04f8e682-e29e-41b9-8c33-d4ab7ec5ee62"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stabilise Elevation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2471388f-69ed-4b67-a673-60661fe55099"",
                    ""path"": ""<Keyboard>/quote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stabilise Descent"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Helicopter
        m_Helicopter = asset.FindActionMap("Helicopter", throwIfNotFound: true);
        m_Helicopter_EngagePower = m_Helicopter.FindAction("Engage Power", throwIfNotFound: true);
        m_Helicopter_CutPower = m_Helicopter.FindAction("Cut Power", throwIfNotFound: true);
        m_Helicopter_StabiliseElevation = m_Helicopter.FindAction("Stabilise Elevation", throwIfNotFound: true);
        m_Helicopter_StabiliseDescent = m_Helicopter.FindAction("Stabilise Descent", throwIfNotFound: true);
        m_Helicopter_RotarSpeed = m_Helicopter.FindAction("Rotar Speed", throwIfNotFound: true);
        m_Helicopter_Heading = m_Helicopter.FindAction("Heading", throwIfNotFound: true);
        m_Helicopter_Rotation = m_Helicopter.FindAction("Rotation", throwIfNotFound: true);
        m_Helicopter_Banking = m_Helicopter.FindAction("Banking", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Helicopter
    private readonly InputActionMap m_Helicopter;
    private IHelicopterActions m_HelicopterActionsCallbackInterface;
    private readonly InputAction m_Helicopter_EngagePower;
    private readonly InputAction m_Helicopter_CutPower;
    private readonly InputAction m_Helicopter_StabiliseElevation;
    private readonly InputAction m_Helicopter_StabiliseDescent;
    private readonly InputAction m_Helicopter_RotarSpeed;
    private readonly InputAction m_Helicopter_Heading;
    private readonly InputAction m_Helicopter_Rotation;
    private readonly InputAction m_Helicopter_Banking;
    public struct HelicopterActions
    {
        private @HelicopterInputActionMap m_Wrapper;
        public HelicopterActions(@HelicopterInputActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @EngagePower => m_Wrapper.m_Helicopter_EngagePower;
        public InputAction @CutPower => m_Wrapper.m_Helicopter_CutPower;
        public InputAction @StabiliseElevation => m_Wrapper.m_Helicopter_StabiliseElevation;
        public InputAction @StabiliseDescent => m_Wrapper.m_Helicopter_StabiliseDescent;
        public InputAction @RotarSpeed => m_Wrapper.m_Helicopter_RotarSpeed;
        public InputAction @Heading => m_Wrapper.m_Helicopter_Heading;
        public InputAction @Rotation => m_Wrapper.m_Helicopter_Rotation;
        public InputAction @Banking => m_Wrapper.m_Helicopter_Banking;
        public InputActionMap Get() { return m_Wrapper.m_Helicopter; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(HelicopterActions set) { return set.Get(); }
        public void SetCallbacks(IHelicopterActions instance)
        {
            if (m_Wrapper.m_HelicopterActionsCallbackInterface != null)
            {
                @EngagePower.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnEngagePower;
                @EngagePower.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnEngagePower;
                @EngagePower.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnEngagePower;
                @CutPower.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnCutPower;
                @CutPower.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnCutPower;
                @CutPower.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnCutPower;
                @StabiliseElevation.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnStabiliseElevation;
                @StabiliseElevation.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnStabiliseElevation;
                @StabiliseElevation.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnStabiliseElevation;
                @StabiliseDescent.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnStabiliseDescent;
                @StabiliseDescent.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnStabiliseDescent;
                @StabiliseDescent.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnStabiliseDescent;
                @RotarSpeed.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnRotarSpeed;
                @RotarSpeed.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnRotarSpeed;
                @RotarSpeed.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnRotarSpeed;
                @Heading.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnHeading;
                @Heading.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnHeading;
                @Heading.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnHeading;
                @Rotation.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnRotation;
                @Rotation.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnRotation;
                @Rotation.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnRotation;
                @Banking.started -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnBanking;
                @Banking.performed -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnBanking;
                @Banking.canceled -= m_Wrapper.m_HelicopterActionsCallbackInterface.OnBanking;
            }
            m_Wrapper.m_HelicopterActionsCallbackInterface = instance;
            if (instance != null)
            {
                @EngagePower.started += instance.OnEngagePower;
                @EngagePower.performed += instance.OnEngagePower;
                @EngagePower.canceled += instance.OnEngagePower;
                @CutPower.started += instance.OnCutPower;
                @CutPower.performed += instance.OnCutPower;
                @CutPower.canceled += instance.OnCutPower;
                @StabiliseElevation.started += instance.OnStabiliseElevation;
                @StabiliseElevation.performed += instance.OnStabiliseElevation;
                @StabiliseElevation.canceled += instance.OnStabiliseElevation;
                @StabiliseDescent.started += instance.OnStabiliseDescent;
                @StabiliseDescent.performed += instance.OnStabiliseDescent;
                @StabiliseDescent.canceled += instance.OnStabiliseDescent;
                @RotarSpeed.started += instance.OnRotarSpeed;
                @RotarSpeed.performed += instance.OnRotarSpeed;
                @RotarSpeed.canceled += instance.OnRotarSpeed;
                @Heading.started += instance.OnHeading;
                @Heading.performed += instance.OnHeading;
                @Heading.canceled += instance.OnHeading;
                @Rotation.started += instance.OnRotation;
                @Rotation.performed += instance.OnRotation;
                @Rotation.canceled += instance.OnRotation;
                @Banking.started += instance.OnBanking;
                @Banking.performed += instance.OnBanking;
                @Banking.canceled += instance.OnBanking;
            }
        }
    }
    public HelicopterActions @Helicopter => new HelicopterActions(this);
    public interface IHelicopterActions
    {
        void OnEngagePower(InputAction.CallbackContext context);
        void OnCutPower(InputAction.CallbackContext context);
        void OnStabiliseElevation(InputAction.CallbackContext context);
        void OnStabiliseDescent(InputAction.CallbackContext context);
        void OnRotarSpeed(InputAction.CallbackContext context);
        void OnHeading(InputAction.CallbackContext context);
        void OnRotation(InputAction.CallbackContext context);
        void OnBanking(InputAction.CallbackContext context);
    }
}
