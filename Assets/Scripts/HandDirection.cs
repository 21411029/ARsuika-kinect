using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class HandDirection
{
    Vector3 tipPos, handPos, wristPos, elbowPos;
    int status = 0;

    Vector3 lastPos;
    Vector3 position;

    Vector3 lastDir;
    Vector3 currentDir;

    float movement;
    public bool isSwinging;

    bool isRight;

    public void Update(bool isDoubleHanded)
    {
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
        movement = movement * 0.96f + move.magnitude;

        float threshold;
        bool wasSwinging = isSwinging;
        if (move.magnitude > 0)
        {
            if (isSwinging)
                isSwinging = (move.magnitude > 0.01f);
            else
                isSwinging = (move.magnitude > 0.07f && Vector3.Dot(move.normalized, Vector3.down) > 0.5f);

            if (wasSwinging && !isSwinging)
                Debug.Log("stop swing: " + move.magnitude + " : " + Vector3.Dot(move.normalized, Vector3.down));
        }

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
        tempDir = tempDir * 0.15f + currentDir * 0.85f;
        currentDir = tempDir;
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
        // isSwinging = false;
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
