using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowAutomationTest : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject arrowPrefab;

    [Header("Config")]
    [SerializeField] Transform vectorTransform;
    [SerializeField] InputAction fireAction;
    [SerializeField] float speed = 0.1f;

    private Arrow arrow;

    // Start is called before the first frame update
    void Start() => Instantiate();

    void OnEnable() => fireAction.Enable();

    void OnDisable() => fireAction.Disable();

    // void OnEnable() => HandController.InputChangeEventReceived += OnActuation;

    // void OnDisable() => HandController.InputChangeEventReceived -= OnActuation;

    // private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    // {
    //     if ((int) characteristics == (int) HandController.LeftHand)
    //     {
    //         if (actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus))
    //         {
    //             InstantiateAndFire();
    //         }
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        if (fireAction.triggered)
        {
            Fire();
        }
    }


    private void Instantiate()
    {
        var instance = Instantiate(arrowPrefab);
        instance.transform.position = vectorTransform.transform.position;
        instance.transform.rotation = vectorTransform.transform.rotation;
        arrow = instance.GetComponent<Arrow>();
    }

    public void Fire()
    {
        arrow.Fire(0.25f);
        Instantiate();
    }
}
