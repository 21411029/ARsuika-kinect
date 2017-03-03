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

    public void Update()
    {
        leftHand.Update(isDoubleHanded);
        rightHand.Update(isDoubleHanded);
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

    public bool isSwinging()
    {
        return getWieldingHand().isSwinging;
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
        {
            Vector3 doubleHandDirFromElbow = leftHand.getDirection() + rightHand.getDirection();
            Vector3 doubleHandDirFromHandDiff = rightHand.getPosition() - leftHand.getPosition();
            float handDist = Vector3.Distance(leftHand.getPosition(), rightHand.getPosition());
            handDist = handDist > 0.15f ? 0.15f : (handDist < 0 ? 0 : handDist);

            // result = Vector3.Lerp(doubleHandDirFromElbow, doubleHandDirFromHandDiff, handDist / 0.15f);
            result = doubleHandDirFromElbow;
        }
        else
            result = getWieldingHand().getDirection();
        
        return result;
    }

    private bool closeHands()
    {
        float dist;

        dist = Vector3.SqrMagnitude(leftHand.getPosition() - rightHand.getPosition());
        return (dist < 0.3f * 0.3f);    // double-handed = less than 20cm
    }

    private HandDirection getWieldingHand()
    {
        float moveR = rightHand.getMovement();
        float moveL = leftHand.getMovement();
        float threshold = 1.0f;                 // hysteresis factor for changing hands 

        if ( !(moveR < 0.5f && moveL < 0.5f) )  // Do not change the hand when standing still
        {
            if (isRightHanded)
                isRightHanded = (moveR * threshold > moveL);
            else // if leftHanded
                isRightHanded = (moveR > moveL * threshold);
        }
        if (isRightHanded)
            return rightHand;
        else
            return leftHand;
    }
}
