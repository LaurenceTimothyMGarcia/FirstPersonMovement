using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("Reference")]
    private Movement pm;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask whatIsGrapple;
    [SerializeField] LineRenderer lr;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    [SerializeField] float grapplingCD;
    private float grapplingCDTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse0;

    private bool grappling;

    private void Start()
    {
        pm = GetComponent<Movement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCDTimer > 0)
        {
            return;
        }

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrapple))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else 
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;
        grapplingCDTimer = grapplingCD;
        lr.enabled = false;
    }

}
