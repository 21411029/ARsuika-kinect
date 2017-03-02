using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class StickDetector {
    HandDirection leftHand, rightHand;

    private float doubleHandTimer;
    private bool isDoubleHanded;
    private bool isRightHanded;

    public StickDetector()
    {
        leftHand = new HandDirection(false);
        rightHand = new HandDirection(true);
        doubleHandTimer = 0;
        isDoubleHanded = false;
        isRightHanded = true;
    }

    public void reset(float deltaTime)
    {
        leftHand.reset();
        rightHand.reset();
        if (closeHands())
        {
            doubleHandTimer += deltaTime;
            if (doubleHandTimer > 0.5f)
                isDoubleHanded = true;
        }
        else
        {
            isDoubleHanded = false;
            doubleHandTimer = 0;
        }
    }

    public void setPos(Vector3 pos, Kinect.JointType joint)
    {
        leftHand.setPos(pos, joint);
        rightHand.setPos(pos, joint);
    }

    public Vector3 getPosition()
    {
        Vector3 result;
        if (isDoubleHanded)
            result = (leftHand.getPosition() + rightHand.getPosition()) / 2.0f;
        else
            result = getWieldingHand().getPosition();
        return result;
    }

    public Vector3 getDirection()
    {
        Vector3 result;

        if (isDoubleHanded)
            result = (leftHand.getDirection(true) + rightHand.getDirection(true));

        else
            result = getWieldingHand().getDirection();
        
        return result;
    }

    private bool closeHands()
    {
        float dist;

        dist = Vector3.SqrMagnitude(leftHand.getPosition() - rightHand.getPosition());
        return (dist < 0.2f * 0.2f);
    }

    private HandDirection getWieldingHand()
    {
        isRightHanded = (rightHand.getMovement() > leftHand.getMovement());
        if (isRightHanded)
            return rightHand;
        else
            return leftHand;
    }
}
