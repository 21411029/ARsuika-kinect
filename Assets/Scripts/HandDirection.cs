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

    public void Update()
    {


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
        if (isSwinging)
            threshold = 0.02f;
        else
            threshold = 0.05f;

        Debug.Log(Vector3.Dot(move, Vector3.down));

        if (move.magnitude > threshold && Vector3.Dot(move.normalized, Vector3.down) > 0.5f)
            isSwinging = true;
        else
            isSwinging = false;

        Debug.Log(isSwinging);

        lastPos = position;
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


    public Vector3 getDirection(bool fromElbow = false)
    {
        Vector3 result;
        if( fromElbow)
            result = handPos - elbowPos;
        else if ((status & 0x03) == 0x03)
            result = tipPos - handPos;
        else if ((status & 0x05) == 0x05)
            result = tipPos - wristPos;
        else if ((status & 0x06) == 0x06)
            result = handPos - wristPos;
        else if ((status & 0x0a) == 0x0a)
            result = handPos - elbowPos;
        else
            result = lastDir;

        lastDir = result;
        result = result * 0.2f + currentDir * 0.8f;
        currentDir = result;
        return result;
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
