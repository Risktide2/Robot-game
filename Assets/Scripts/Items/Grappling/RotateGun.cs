using UnityEngine;

namespace Items.Grappling
{
    /// <summary>
    /// This class handles rotating the grappling gun to face the direction of the grapple point (forward if not grappling)
    /// </summary>
    public class RotateGun : MonoBehaviour
    {
        [SerializeField] private GrapplingGun grappling;
        [SerializeField] private float rotationSpeed = 5f;

        private Quaternion _desiredRotation;

        private void Update()
        {
            if (!grappling.IsGrappling())
                _desiredRotation = transform.parent.rotation;
            else
                _desiredRotation = Quaternion.LookRotation(grappling.GetGrapplePoint() - transform.position);

            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, Time.deltaTime * rotationSpeed);
        }
    }
}