using UnityEngine;

/*
 * Handles the visuals of the rope for the grappling gun
 */
public class GrapplingGun : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float spring;
    [SerializeField] private float mass;
    [SerializeField] private float damp;
    [SerializeField] private LayerMask whatIsGrappleable;

    [Header("References")] public Transform gunTip;
    public new Transform camera;
    public Transform player;
    private SpringJoint _joint;    
    private Vector3 _grapplePoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartGrapple();
        else if (Input.GetKeyUp(KeyCode.Q)) StopGrapple();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            _grapplePoint = hit.point;
            _joint = player.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _grapplePoint;

            var distanceFromPoint = Vector3.Distance(player.position, _grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            _joint.maxDistance = distanceFromPoint * 0f;
            _joint.minDistance = distanceFromPoint * 0f;

            //Adjust these values to fit your game.
            _joint.spring = spring;
            _joint.damper = damp;
            _joint.massScale = mass;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    private void StopGrapple()
    {
        Destroy(_joint);
    }


    public bool IsGrappling()
    {
        return _joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return _grapplePoint;
    }

    public Vector3 GetGunTip()
    {
        return gunTip.position;
    }
}