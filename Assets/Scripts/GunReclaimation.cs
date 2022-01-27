using UnityEngine;

public class GunReclaimation : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        var trigger = collider.gameObject;
        // Debug.Log($"{trigger.name}");
        
        if (trigger.CompareTag("Gun"))
        {
            var manager = trigger.GetComponent<DockableGunInteractableManager>() as DockableGunInteractableManager;

            if (manager != null)
            {
                manager.RestoreCachedGunState();
            }
        }
        // else
        // {
        //     trigger.transform.position += new Vector3 (0f, 5f, 0f);
        //     trigger.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // }
    }
}