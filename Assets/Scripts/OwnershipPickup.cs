using UnityEngine;
using Unity.Netcode;

public class OwnershipPickup : NetworkBehaviour
{
    [SerializeField] float pickupRadius = 2f;
    [SerializeField] string pickupTag = "Pickup";

    private void Update()
    {
        if(!HasAuthority || !IsSpawned) { return; }

        var nearbyColliders = Physics.OverlapSphere(transform.position, pickupRadius);
        foreach(Collider collider in nearbyColliders)
        {
            if (!collider.CompareTag(pickupTag)) continue;

            NetworkObject networkObject = collider.GetComponent<NetworkObject>();

            if (!networkObject.IsOwner)
            {
                Debug.Log($"Changing ownership of {networkObject} to {NetworkManager.LocalClientId}");
                networkObject.ChangeOwnership(NetworkManager.LocalClientId);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
