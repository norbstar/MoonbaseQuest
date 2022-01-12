using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InConeValidator))]
[RequireComponent(typeof(EyeSocketActuator))]
public class AlertToPlayerManager : MonoBehaviour
{
    [SerializeField] new Camera camera;

    private InConeValidator inConeValidator;
    private EyeSocketActuator eyeSocketActuator;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        inConeValidator = GetComponent<InConeValidator>() as InConeValidator;
        eyeSocketActuator = GetComponent<EyeSocketActuator>() as EyeSocketActuator;
    }

    // Update is called once per frame
    void Update()
    {
        if (inConeValidator.IsInCone(camera.transform.position))
        {
            eyeSocketActuator.SetColor(Color.red);
        }
        else
        {
            eyeSocketActuator.SetColor(Color.white);
        }
    }
}