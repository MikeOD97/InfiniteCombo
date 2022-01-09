
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller : MonoBehaviour
{
    public LayerMask collisionMask;

    const float skinwidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;


    BoxCollider2D coll;
    RaycastOrigins raycastOrigins;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        calculateRaySpacing();
    }

    public void Move(Vector3 vel)
    {
        UpdateRaycastOrigins();
        if(vel.x != 0)
            HorizontalCollisions(ref vel);
        if (vel.y != 0)
            VerticalCollisions(ref vel);
        transform.Translate(vel);
    }

    void VerticalCollisions(ref Vector3 vel)
    {
        float directionY = Mathf.Sign(vel.y);
        float rayLength = Mathf.Abs(vel.y + skinwidth);
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; //If you're moving down, cast from the bottom, else the top
            rayOrigin += Vector2.right * (verticalRaySpacing * i + vel.x); //Take into account where the object will be
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            //if there was a hit, set y velocity to the amount needed to move to reach the obstacle
            if(hit)
            {
                vel.y = (hit.distance - skinwidth) * directionY;
                rayLength = hit.distance; //in case there are two detections close to the same elevation, reduces clipping
            }
        }
    }

    void HorizontalCollisions(ref Vector3 vel)
    {
        float directionX = Mathf.Sign(vel.x);
        float rayLength = Mathf.Abs(vel.x + skinwidth);
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomLeft; //If you're moving down, cast from the bottom, else the top
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); //Take into account where the object will be
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            //if there was a hit, set y velocity to the amount needed to move to reach the obstacle
            if (hit)
            {
                vel.x = (hit.distance - skinwidth) * directionX;
                rayLength = hit.distance; //in case there are two detections close to the same elevation, reduces clipping
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinwidth * -2); //need a buffer so the rays can still cast when the edges are hitting a surface

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }
    void calculateRaySpacing()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinwidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}