using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleController : CatchDetector
{
    public Transform LeftTarget;
    public Transform RightTarget;

    public Transform Ball1;
    public Transform Ball2;
    public Transform Ball3;

    public Rigidbody Ball1RB;
    public Rigidbody Ball2RB;
    public Rigidbody Ball3RB;

    public Vector3 ThrowingPowerRightwards = new Vector3();
    public Vector3 ThrowingPowerLeftwards = new Vector3();
    public float time;


    private int totalCount = 1;     // which catch we are currently on
    private int targetArm;          // which arm are we throwing to (0 is left, 1 is right)
    private bool ballInLeftHand = true;
    private bool ballInRightHand = true;
    private bool firstCycleDone;
    private Transform BallBeingThrown;
    private Rigidbody BallBeingThrown_RB;

    // Start is called before the first frame update
    void Start()
    {
        BallBeingThrown = Ball1;
        BallBeingThrown_RB = Ball1RB;

        Ball1RB.useGravity = false;
        Ball2RB.useGravity = false;
        Ball3RB.useGravity = false;

        firstCycleDone = false;
        StartCoroutine(ThrowOne());
    }

    // Update is called once per frame
    void Update()
    {
        // print(totalCount);
        if(CatchDetector.BallIsInHand)
            print("Catch Detected");
        FindTargetArm();
        FindIfBallCaught();

        if (targetArm == 1 && !ballInRightHand)
        {
            if (BallBeingThrown.position.x != RightTarget.position.x || BallBeingThrown.position.z != RightTarget.position.z)
            {
                StartCoroutine(MoveTowardsBall(RightTarget, BallBeingThrown));
            }
            else
            {
                StopCoroutine(MoveTowardsBall(RightTarget, BallBeingThrown));
            }
        }
        else if (targetArm == 0 && !ballInLeftHand)
        {
            if (BallBeingThrown.position != LeftTarget.position)
            {
                StartCoroutine(MoveTowardsBall(LeftTarget, BallBeingThrown));
            }
            else
            {
                StopCoroutine(MoveTowardsBall(LeftTarget, BallBeingThrown));
            }
        }
    }

    void FindTargetArm()
    {
        if (totalCount % 2 == 1)  // odd number throw
        {
            targetArm = 1; // target is right arm
        }
        else if (totalCount % 2 == 0)  // even number throw
        {
            targetArm = 0; // target is left arm
        }
    }

    public void FindBallBeingThrown()   //sets BallBeingThrown (Target) and BallBeingThrown_RB (Rigidbody)
    {
        if (BallBeingThrown == Ball1)
        {

            BallBeingThrown = Ball2;
            BallBeingThrown_RB = Ball2RB;
        }
        else if (BallBeingThrown == Ball2)
        {
            BallBeingThrown = Ball3;
            BallBeingThrown_RB = Ball3RB;
        }
        else
        {
            BallBeingThrown = Ball1;
            BallBeingThrown_RB = Ball1RB;
        }
    }

    void FindIfBallCaught()
    {
        if (firstCycleDone && CatchDetector.BallIsInHand)
        {
            totalCount++;

            if (targetArm == 1)  //sets ball as a child of the target to attach it to the hand when caught
            {
                BallBeingThrown.parent = RightTarget;
                ballInRightHand = true;
                StartCoroutine(MoveTowardsPoint(RightTarget, 6f, RightTarget.position.y, -3.5f));
            }
            else
            {
                BallBeingThrown.parent = LeftTarget;
                ballInLeftHand = true;
                StartCoroutine(MoveTowardsPoint(LeftTarget, 6f, LeftTarget.position.y, 3.5f));
            }

            StartCoroutine(ThrowWhenCaught(BallBeingThrown_RB));
            FindBallBeingThrown();
            CatchDetector.BallIsInHand = false;
        }
    }

    IEnumerator MoveTowardsBall(Transform Target, Transform Ball)
    {
        Vector3 BallLocation = new Vector3(Ball.position.x, -3.04f, Ball.position.z);

        Target.position = Vector3.MoveTowards(Target.position, BallLocation, .5f);

        yield return null;
    }

    public IEnumerator MoveTowardsPoint(Transform Target, float x, float y, float z)
    {
        Vector3 GoalPoint = new Vector3(x, y, z);

        while(Target.position != GoalPoint)
        {
            Target.position = Vector3.MoveTowards(Target.position, GoalPoint, 0.2f);
            yield return null;
        }

        yield return null;
    }

    IEnumerator ThrowOne() // First Cycle
    {
        Ball1RB.useGravity = true;
        Ball1RB.AddForce(ThrowingPowerRightwards, ForceMode.Impulse);
        yield return new WaitForSeconds(time);
        //Throws Ball 1 towards the right hand and waits for <time> seconds
        Ball2RB.useGravity = true;
        Ball2RB.AddForce(ThrowingPowerLeftwards, ForceMode.Impulse);
        ballInRightHand = false;
        yield return new WaitForSeconds(time * 3 / 2);
        //Throws Ball 2 towards the left hand and waits for <time * 3 / 2> seconds
        Ball3RB.useGravity = true;
        Ball3RB.AddForce(ThrowingPowerRightwards, ForceMode.Impulse);
        ballInLeftHand = false;
        //Throws Ball 3 towards the right hand
        firstCycleDone = true;
    }

    public IEnumerator ThrowWhenCaught(Rigidbody objectCaught)
    {
        yield return new WaitForSeconds(.2f);
        if (targetArm == 0)
        {
            objectCaught.AddForce(ThrowingPowerLeftwards, ForceMode.Impulse);
            ballInRightHand = false;
        }
        else
        {
            objectCaught.AddForce(ThrowingPowerRightwards, ForceMode.Impulse);
            ballInLeftHand = false;
        }
        objectCaught.gameObject.transform.parent = null;
    }
}
