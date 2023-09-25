using UnityEngine;

public class RotateGun : MonoBehaviour
{
    public GrapplingGun grappling;

    private Quaternion _desiredRotation;
    private float _rotationSpeed = 5f;

    private void Update()
    {
        if (!grappling.IsGrappling())
            _desiredRotation = transform.parent.rotation;
        else
            _desiredRotation = Quaternion.LookRotation(grappling.GetGrapplePoint() - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, Time.deltaTime * _rotationSpeed);
    }
}