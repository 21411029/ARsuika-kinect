using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class HandDirection
{
    Vector3 tipPos, handPos, wristPos, elbowPos;
    int status = 0;

    Vector3 lastPos;
    Vector3 lastDir;
    Vector3 currentDir;

    bool isRight;

    public HandDirection()
    {
        isRight = true;
        status = 0;
        lastPos = Vector3.zero;
        lastDir = Vector3.forward;
        currentDir = Vector3.zero;
    }
    public HandDirection(bool isForRight)
    {
        isRight = isForRight;
        status = 0;
        lastPos = Vector3.zero;
        lastDir = Vector3.forward;
        currentDir = Vector3.zero;
    }

    public void reset()
    {
        status = 0;
    }

    public Vector3 getPosition()
    {
        Vector3 result;
        if ((status | 0x02) != 0)
            result = handPos;
        else if ((status | 0x01) != 0)
            result = tipPos;
        else if ((status | 0x04) != 0)
            result = wristPos;
        else
            result = lastPos;

        lastPos = result;
        return result;
    }

    public Vector3 getDirection()
    {
        Vector3 result;
        if ((status | 0x03) != 0)
            result = tipPos - handPos;
        else if ((status | 0x05) != 0)
            result = tipPos - wristPos;
        else if ((status | 0x06) != 0)
            result = handPos - wristPos;
        else if ((status | 0x0a) != 0)
            result = handPos - elbowPos;
        else
            result = lastDir;

        lastDir = result;
        result = result * 0.2f + currentDir * 0.8f;
        currentDir = result;
        return result;
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
