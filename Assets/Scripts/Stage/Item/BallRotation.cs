using UnityEngine;

public class BallRotation : MonoBehaviour
{
    [SerializeField]
    Rigidbody _rigidBody;
    [SerializeField]
    private float _rotateSpeed = 5;
    private void FixedUpdate()
    {
        var velocity = _rigidBody.velocity;
        var spinAxis = Vector3.Cross(velocity, -Vector3.up).normalized;
        transform.Rotate(spinAxis * _rotateSpeed);
    }
}
