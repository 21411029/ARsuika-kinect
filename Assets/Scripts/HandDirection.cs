using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class HandDirection
{
    // Constants
    float MOVEMENTDECAY = 0.96f;    // ratio of decaying movement accumulation. bigger = longer accumulation
    float DIRECTIONDECAY = 0.8f;    // ratio of smoothing direction. bigger = slow and smooth
    float SWINGKEEPSPEED = 0.04f;   // the speed to keep the swing state
    float SWINGSTARTSPEED = 0.1f;   // the speed to enter the swing state
    float SWINGSTARTANGLECOS = 0.8f;    // the cos(angle) of the swing against DOWN to enter the swing state
    float MAXSWINGPERID = 1.0f;     // max swing duration. after the period the swing is forced to terminate

    Vector3 tipPos, handPos, wristPos, elbowPos;
    int status = 0;

    Vector3 lastPos;
    Vector3 position;

    Vector3 lastDir;
    Vector3 currentDir;

    float movement;
    public bool isSwinging;
    float swingingTimer;

    bool isRight;

    public void Update(bool isDoubleHanded, float elapsedTime)
    {
        Vector3 lastStickPos = lastPos + lastDir.normalized;

        // Position
        if ((status | 0x02) != 0)
            position = handPos;
        else if ((status | 0x01) != 0)
            position = tipPos;
        else if ((status | 0x04) != 0)
            position = wristPos;
        else
            position = lastPos;
        Vector3 move = position - lastPos;
        movement = movement * MOVEMENTDECAY + move.magnitude;
        lastPos = position;

        // Direction
        Vector3 tempDir;
        if (isDoubleHanded)
            tempDir = handPos - elbowPos;
        else if ((status & 0x03) == 0x03)
            tempDir = tipPos - handPos;
        else if ((status & 0x05) == 0x05)
            tempDir = tipPos - wristPos;
        else if ((status & 0x06) == 0x06)
            tempDir = handPos - wristPos;
//        else if ((status & 0x0a) == 0x0a)
//            tempDir = handPos - elbowPos;
        else
            tempDir = lastDir;
        lastDir = currentDir;
        tempDir = tempDir * (1-DIRECTIONDECAY) + currentDir * DIRECTIONDECAY;
        currentDir = tempDir;

        // Swing Check
        Vector3 currentStickPos = position + currentDir.normalized;
        Vector3 stickMove = currentStickPos - lastStickPos;

        bool wasSwinging = isSwinging;
        if (stickMove.magnitude > 0 && status > 0)
        {
            if (isSwinging) // if isSwinging, keep the state while stick is moving to any direction
            {
                isSwinging = (stickMove.magnitude > SWINGKEEPSPEED);
            }
            else  // to enter isSwinging, fast downward swing is required
            {
                isSwinging = (stickMove.magnitude > SWINGSTARTSPEED 
                    && Vector3.Dot(stickMove.normalized, Vector3.down) > SWINGSTARTANGLECOS);
            }
        }
        if (!wasSwinging && isSwinging)
            swingingTimer = MAXSWINGPERID;

        swingingTimer -= elapsedTime;
        if (swingingTimer < 0)
        {
            isSwinging = false;
            Debug.Log("Swing Expired");
        }

        // debug: Change the state of Swing
        if (wasSwinging && !isSwinging)
            Debug.Log("stop swing: " + isRight + stickMove.magnitude + " : " + Vector3.Dot(stickMove.normalized, Vector3.down));
        else if (!wasSwinging && isSwinging)
            Debug.Log("start swing: " + isRight + stickMove.magnitude + " : " + Vector3.Dot(stickMove.normalized, Vector3.down));

    }


    public HandDirection()
    {
        isRight = true;
        status = 0;
        lastPos = Vector3.zero;
        lastDir = Vector3.forward;
        currentDir = Vector3.zero;
        movement = 0;
    }
    public HandDirection(bool isForRight)
    {
        isRight = isForRight;
        status = 0;
        lastPos = Vector3.zero;
        lastDir = Vector3.forward;
        currentDir = Vector3.zero;
        movement = 0;
    }

    public void reset()
    {
        status = 0;
    }

    public Vector3 getPosition()
    {
        return position;
    }


    public Vector3 getDirection()
    {
        return currentDir;
    }

    public float getMovement()
    {
        return movement;
    }

    public void setPos(Vector3 pos, Kinect.JointType joint)
    {
        if (isRight)
        {
            switch (joint)
            {
                case Kinect.JointType.HandTipRight:
                    tipPos = pos;
                    status |= 1;
                    break;
                case Kinect.JointType.HandRight:
                    handPos = pos;
                    status |= 2;
                    break;
                case Kinect.JointType.WristRight:
                    wristPos = pos;
                    status |= 4;
                    break;
                case Kinect.JointType.ElbowRight:
                    elbowPos = pos;
                    status |= 8;
                    break;
            }
        }
        else
        {
            switch (joint)
            {
                case Kinect.JointType.HandTipLeft:
                    tipPos = pos;
                    status |= 1;
                    break;
                case Kinect.JointType.HandLeft:
                    handPos = pos;
                    status |= 2;
                    break;
                case Kinect.JointType.WristLeft:
                    wristPos = pos;
                    status |= 4;
                    break;
                case Kinect.JointType.ElbowLeft:
                    elbowPos = pos;
                    status |= 8;
                    break;
            }
        }
    }
}
