using System.Collections.Generic;

using UnityEngine;

public class ManualRotationTargetTest : MonoBehaviour
{
    private HandController leftController;
    
    // Start is called before the first frame update
    void Start()
    {
        if (TryGet.TryGetControllers(out List<HandController> controllers))
        {
            foreach (HandController controller in controllers)
            {
                if ((int) controller.Characteristics == (int) HandController.LeftHand)
                {
                    leftController = controller;
                }
            }
        }
    }
    
    // Update is called once per frame
    void Update() => transform.rotation = leftController.transform.rotation;
}