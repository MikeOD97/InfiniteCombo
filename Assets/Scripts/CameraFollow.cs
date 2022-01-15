using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Controller target; //The player to follow
    public Vector2 focusAreaSize; //the size of the area being focused on

    public float vertOffset; //keep track of target offset from area
    public float lookAheadDistX; //The distance for the camera to look ahead when moving
    public float lookSmoothTimeX;
    public float vertSmoothTime;

    FocusArea focusArea;

    float currentLookAheadX; //The current camera lookAhead
    float targetLookAheadX; //The lookAhead desired
    float lookAheadDirX; //The direction that the camera is looking ahead to
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    void Start()
    {
        focusArea = new FocusArea(target.GetComponent<BoxCollider2D>().bounds, focusAreaSize);
    }

    //Update takes place after other update scripts
    void LateUpdate()
    {
        focusArea.Update(target.GetComponent<BoxCollider2D>().bounds);
        Vector2 focusPosition = focusArea.center + Vector2.up * vertOffset;

        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0) //Basically, only snap to LookAhead if the player is still moving in the correct direction
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDistX;
            }
            else
            {
                if(!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistX - currentLookAheadX) / 4f;
                }
            }
        }
       
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, vertSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10; //Put it forward on the Z Axis
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right, top, bottom;

        //Defines the area to focus on based on the bounds of the target
        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);

        }

        //moves the focus area's position along with the target
        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
                shiftX = targetBounds.min.x - left;
            else if (targetBounds.max.x > right)
                shiftX = targetBounds.max.x - right;
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
                shiftY = targetBounds.min.y - bottom;
            else if (targetBounds.max.y > top)
                shiftY = targetBounds.max.y - top;
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
   
}
