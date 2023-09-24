using System.IO;
using UnityEngine;

public class GrapplingGun : MonoBehaviour {


    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    public float maxDistance = 100f;
    private SpringJoint joint;
    public float spring;
    public float mass;
    public float damp;



    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            StartGrapple();
        }
        else if (Input.GetKeyUp(KeyCode.E)) {
            StopGrapple();
        }
    }



    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0f;
            joint.minDistance = distanceFromPoint * 0f;
            Debug.Log(joint.currentForce);

            //Adjust these values to fit your game.
            joint.spring = spring;
            joint.damper = damp;
            joint.massScale = mass;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        Destroy(joint);
    }


    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
