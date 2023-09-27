using UnityEngine;

/*
 * Handles the visuals of the rope for the grappling gun
 */
public class GrapplingRope : MonoBehaviour
{
    [Header("References")]
    public GrapplingGun grapplingGun;
    
    [Space(10)]
    [Header("Settings")]
    [SerializeField] private int quality;
    [SerializeField] private float damper;
    [SerializeField] private float strength;
    [SerializeField] private float velocity;
    [SerializeField] private float waveCount;
    [SerializeField] private float waveHeight;
    [SerializeField] private AnimationCurve affectCurve;

    private CustomSpring _spring;
    private LineRenderer _lineRenderer;
    private Vector3 _currentGrapplePosition = Vector3.zero;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _spring = new CustomSpring();
        _spring.RestLength = 0;
    }

    //Called after Update
    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!grapplingGun.IsGrappling())
        {
            _currentGrapplePosition = grapplingGun.gunTip.position;
            _spring.Reset();
            _lineRenderer.positionCount = 0;
            return;
        }

        //Init points for drawing if empty
        if (_lineRenderer.positionCount == 0)
        {
            _spring.Velocity = velocity;
            _spring.DampAmount = damper;
            _spring.Stiffness = strength;
            
            _lineRenderer.positionCount = quality;
        }

        
        _spring.Update(Time.deltaTime);

        //points from the grapple gun
        Vector3 grapplePoint = grapplingGun.GetGrapplePoint();
        Vector3 gunTipPosition = grapplingGun.GetGunTip();
        
        //directions for the grapple rope
        Vector3 grappleDirection = (grapplePoint - gunTipPosition).normalized;
        Vector3 waveUp = Quaternion.LookRotation(grappleDirection) * Vector3.up;

        //The point of the end of the rope
        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        //Update each point along the rope
        Vector3[] points = new Vector3[_lineRenderer.positionCount];
        for (var i = 0; i < points.Length; i++)
            points[i] = GetRopePoint(gunTipPosition, _currentGrapplePosition, waveUp, i);

        //Update all the rope points
        _lineRenderer.SetPositions(points);
    }

    private Vector3 GetRopePoint(Vector3 gunTipPosition, Vector3 currentGrapplePosition, Vector3 waveUp, int index)
    {
        //How far through the rope this point is
        float percent = index / (float)(quality - 1);
        //The raw value from the sine wave
        float waveFactor = Mathf.Sin(percent * waveCount * 2 * Mathf.PI);
        //The height of the sine wave
        float waveAmplitude = waveHeight * _spring.Length * affectCurve.Evaluate(percent);
        //The vector offset of the rope point from line
        Vector3 offset = waveFactor * waveAmplitude * waveUp;
        //The point on line + offset
        return Vector3.Lerp(gunTipPosition, currentGrapplePosition, percent) + offset;
    }
}