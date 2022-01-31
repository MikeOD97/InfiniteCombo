
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour
{
    public LayerMask collisionMask;

    public float skinwidth = .015f;
    const float distanceBetweenRays = .25f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    float maxClimbAngle = 80;
    float maxDescendAngle = 80;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    [HideInInspector]
    public Vector2 playerInput;

    BoxCollider2D coll;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        calculateRaySpacing();
        collisions.faceDir = 1;
    }

    public void Move(Vector2 velocity, Vector2 input)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        playerInput = input;

        if(velocity.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }

        if(velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(velocity.x) + skinwidth;

        if (Mathf.Abs(velocity.x) < skinwidth)
            rayLength = 2 * skinwidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                    continue;
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(i == 0 && slopeAngle <= maxClimbAngle)
                {
                    float distToSlope = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distToSlope = hit.distance - skinwidth;
                        velocity.x -= distToSlope * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distToSlope * directionX;
                }

                if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinwidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }

            }
        }
    }

    void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinwidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "SoftPlatform")
                {
                    if (directionY == 1 || hit.distance == 0)
                        continue;
                    if (playerInput.y == -1)
                    {
                        continue;
                    }

                }


                velocity.y = (hit.distance - skinwidth) * directionY;
                rayLength = hit.distance;

                if(collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinwidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinwidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 vel, float angle)
    {
        float moveDist = Mathf.Abs(vel.x);
        float climbVelY = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDist;
        if (vel.y <= climbVelY)
        {
            vel.y = climbVelY;
            vel.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(vel.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = angle;
        }
    }

    void DescendSlope(ref Vector2 vel)
    {
        float directionX = Mathf.Sign(vel.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == directionX)
                {
                    if(hit.distance - skinwidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(vel.x))
                    {
                        float moveDistance = Mathf.Abs(vel.x);
                        float descendVelY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        vel.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(vel.x);
                        vel.y -= descendVelY;
                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinwidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }
    void calculateRaySpacing()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinwidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
        Debug.Log(boundsHeight);
        Debug.Log(horizontalRayCount);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }


    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public int faceDir;
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}