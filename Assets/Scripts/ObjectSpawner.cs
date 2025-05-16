using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectSpawner : NetworkBehaviour
{
    public GameObject prefab;
    public int numberOfPrefab = 10;

    private void Start()
    {
        if (!HasAuthority) { return; }
        if(!NetworkManager.LocalClient.IsSessionOwner) { return; }

        List<Vector3> randomPoints = new List<Vector3>();
        for(int i = 0; i < numberOfPrefab; i++)
        {
            randomPoints.Add(RandomPointInCircleEdge(5));
        }

        for(int i = 0; i < numberOfPrefab; i++)
        {
            var instance = Instantiate(prefab);
            var networkObject = instance.GetComponent<NetworkObject>();
            instance.transform.position = randomPoints[i];
            networkObject.Spawn();
        }
    }
    private Vector3 RandomPointInCircleEdge(float radius)
    {
        Vector2 point2D = Random.insideUnitCircle * radius; // Random.insideUnitCircle.normalized for spawning objects on the edge of the circle
        return new Vector3(point2D.x, 0, point2D.y);
    }
}
