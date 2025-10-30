using Fusion;
using UnityEngine;

/// <summary>
/// Synchronizes a spaceship's position and rotation across the network using Photon Fusion.
/// - If this object has State Authority, it writes current transform to networked state.
/// - Otherwise, it interpolates locally towards the received networked state for smooth visuals.
/// Attach this component to your spaceship GameObject. Optionally assign a Rigidbody or Rigidbody2D.
/// </summary>
public class SpaceshipNetworkSync : NetworkBehaviour
{
    [Header("Optional Physics References")]
    [SerializeField] private Rigidbody _rigidbody3D;
    [SerializeField] private Rigidbody2D _rigidbody2D;

    [Header("Smoothing")]
    [SerializeField] private float _positionLerpSpeed = 12f;
    [SerializeField] private float _rotationLerpSpeed = 12f;

    [Networked] private Vector3 NetworkedPosition { get; set; }
    [Networked] private Quaternion NetworkedRotation { get; set; }

    public override void Spawned()
    {
        if (_rigidbody3D == null) _rigidbody3D = GetComponent<Rigidbody>();
        if (_rigidbody2D == null) _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            // Authoritative writer: push transform/rigidbody to networked state
            if (_rigidbody2D)
            {
                NetworkedPosition = _rigidbody2D.position;
                NetworkedRotation = Quaternion.Euler(0f, 0f, _rigidbody2D.rotation);
            }
            else if (_rigidbody3D)
            {
                NetworkedPosition = _rigidbody3D.position;
                NetworkedRotation = _rigidbody3D.rotation;
            }
            else
            {
                NetworkedPosition = transform.position;
                NetworkedRotation = transform.rotation;
            }
        }
        else
        {
            // Non-authoritative readers: move towards the received state
            float dt = Runner.DeltaTime;
            if (_rigidbody2D)
            {
                Vector2 targetPos = NetworkedPosition;
                float targetZ = NetworkedRotation.eulerAngles.z;
                _rigidbody2D.position = Vector2.Lerp(_rigidbody2D.position, targetPos, _positionLerpSpeed * dt);
                _rigidbody2D.rotation = Mathf.LerpAngle(_rigidbody2D.rotation, targetZ, _rotationLerpSpeed * dt);
            }
            else if (_rigidbody3D)
            {
                _rigidbody3D.position = Vector3.Lerp(_rigidbody3D.position, NetworkedPosition, _positionLerpSpeed * dt);
                _rigidbody3D.rotation = Quaternion.Slerp(_rigidbody3D.rotation, NetworkedRotation, _rotationLerpSpeed * dt);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, NetworkedPosition, _positionLerpSpeed * dt);
                transform.rotation = Quaternion.Slerp(transform.rotation, NetworkedRotation, _rotationLerpSpeed * dt);
            }
        }
    }
}


