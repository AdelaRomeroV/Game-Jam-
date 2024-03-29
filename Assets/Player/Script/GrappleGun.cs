using UnityEngine;

public class GrappleGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrappleRope grappleRope;
    public playerMov mov;

    [Header("Layers Settings:")]
    [SerializeField] LayerMask grappleLayers;
    [SerializeField] LayerMask collisionLayers;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Push&Pull")]
    [SerializeField] bool pullObject;
    [SerializeField] bool pullPlayer;
    [SerializeField] Transform Object;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;
    float CurrentGravity;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)][SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        CurrentGravity = m_rigidbody.gravityScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !mov.isGrabing)
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse1) && !mov.isGrabing)
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (pullPlayer)
            {
                if (launchToPoint && grappleRope.isGrappling)
                {
                    if (launchType == LaunchType.Transform_Launch)
                    {
                        mov.canJump = false;
                        Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                        Vector2 targetPos = grapplePoint - firePointDistnace;
                        gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                    }
                }
            }
            else if (pullObject)
            {
                Vector2 distanceVector = Object.position - gunPivot.position;
                RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, maxDistnace, grappleLayers);
                Rigidbody2D rgb = Object.GetComponent<Rigidbody2D>();
                
                if (launchToPoint && grappleRope.isGrappling)
                {
                    grapplePoint = _hit.point;

                    if(Vector2.Distance(firePoint.position, Object.position) >= 2)
                    {
                        Vector2 firePointDistnace = Object.position - gunHolder.localPosition;
                        Vector2 targetPos = (Vector2)firePoint.position - firePointDistnace;
                        Object.position = Vector2.Lerp(Object.position, targetPos, Time.deltaTime * launchSpeed / (rgb.mass * 1.5f));

                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = CurrentGravity;
            pullPlayer = false;
            pullObject = false;
            mov.canJump = true;
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, maxDistnace, collisionLayers);
            if(_hit.collider != null)
            {
                if (Physics2D.Raycast(firePoint.position, distanceVector.normalized, maxDistnace, grappleLayers))
                {
                    if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
                    {
                        pullObject = true;
                        Object = _hit.collider.gameObject.transform;

                        grapplePoint = _hit.point;
                        grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                        grappleRope.enabled = true;
                    }
                }
                else
                {
                    if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
                    {
                        pullPlayer = true;
                        grapplePoint = _hit.point;
                        grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                        grappleRope.enabled = true;

                    }
                }
            } 
        }
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    Vector2 distanceVector = firePoint.position - gunHolder.position;

                    m_springJoint2D.distance = distanceVector.magnitude;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:
                    if(pullPlayer)
                    {
                        m_rigidbody.gravityScale = 0;
                        m_rigidbody.velocity = Vector2.zero;
                    }
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }

}

