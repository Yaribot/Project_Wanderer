using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;

public class DistributedPing : NetworkBehaviour
{
    public float PingInterval = 30f;
    private Dictionary<ulong, float> pingTable = new();

    public float GetOneWayPing(ulong clientId) => pingTable.GetValueOrDefault(clientId, -1);

    private void Update()
    {
        ulong localId = NetworkManager.LocalClientId;
        foreach(var targetId in pingTable.Keys)
        {
            if(targetId != localId)
            {
                Debug.Log($"RTT {localId} + {targetId}:"); // Use LogWin...
                GetOneWayPing(targetId);
            }
        }
    }
    public override void OnNetworkSpawn()
    {
        if (HasAuthority) StartCoroutine(PingRoutine());
    }

    IEnumerator PingRoutine()
    {
        while(IsSpawned && NetworkManager.IsConnectedClient)
        {
            foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientId == NetworkManager.Singleton.LocalClientId) continue;

                float sentTime = NetworkManager.ServerTime.TimeAsFloat;
                PingRpc(sentTime, RpcTarget.Single(clientId, RpcTargetUse.Temp));
            }

            yield return new WaitForSecondsRealtime(1.0f / PingInterval);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void PingRpc(float sentTime, RpcParams rpcParams = default)
    {
        pingTable[rpcParams.Receive.SenderClientId] = NetworkManager.ServerTime.TimeAsFloat - sentTime;
        PongRpc(sentTime, RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void PongRpc(float sentTime, RpcParams rpcParams = default)
    {
        float rtt = NetworkManager.ServerTime.TimeAsFloat - sentTime;
        ulong senderId = rpcParams.Receive.SenderClientId;
        // TO DO store the RTT in a table...
    }
}
