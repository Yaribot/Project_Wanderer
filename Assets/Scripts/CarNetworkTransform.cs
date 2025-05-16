using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CarNetworkTransform : NetworkTransform
{
    public GameObject CarVisual; // Child model that will be moved around to simulate prediction

    [Range(0.001f, 2.0f)] public float VelocityThreshold = 0.1f;
    [SerializeField] DistributedPing ping;

    public float OwnerTimeDelta => ping.GetOneWayPing(OwnerClientId);

    NetworkVariable<Vector3> velocity = new();
    NetworkRigidbody networkRigidbody;

    protected override void Awake()
    {
        base.Awake();
        networkRigidbody = GetComponent<NetworkRigidbody>();
    }

    protected override void OnAuthorityPushTransformState(ref NetworkTransformState networkTransformState)
    {
        var carVelocity = networkRigidbody.GetLinearVelocity();
        if(Mathf.Abs(velocity.Value.magnitude - carVelocity.magnitude) >= VelocityThreshold)
        {
            velocity.Value = carVelocity;
        }
        base.OnAuthorityPushTransformState(ref networkTransformState);
    }

    public override void OnFixedUpdate()
    {
        if (IsOwner) return; // owner doesn't need prediction

        var carVelocityDirection = velocity.Value.normalized;
        var carVelocityPredictedmagnitude = velocity.Value.magnitude * OwnerTimeDelta;

        CarVisual.transform.localPosition = carVelocityDirection * carVelocityPredictedmagnitude;

        base.OnFixedUpdate();
    }
}
