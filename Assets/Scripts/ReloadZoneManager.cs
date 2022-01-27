using UnityEngine;

public class ReloadZoneManager : MonoBehaviour
{
    [SerializeField] AudioClip reloadClip;

    public void OnTriggerEnter(Collider collider)
    {
        // Debug.Log($"{gameObject.name}:On Trigger Enter : {collider.name}");
        
        if (collider.gameObject.CompareTag("Gun"))
        {
            var parent = collider.transform.parent.parent;
            var manager = parent.GetComponent<DockableGunInteractableManager>() as DockableGunInteractableManager;
            
            if ((manager != null) && (manager.IsHeld))
            {
                manager.RestoreAmmoCount();
                AudioSource.PlayClipAtPoint(reloadClip, transform.position, 1.0f);
            }
        }
    }
}