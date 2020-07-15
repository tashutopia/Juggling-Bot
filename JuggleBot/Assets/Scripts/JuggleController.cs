using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleController : MonoBehaviour
{
    public Transform LeftTarget;
    public Transform RightTarget;

    public Transform Ball1;
    public Transform Ball2;
    public Transform Ball3;

    public Rigidbody Ball1RB;
    public Rigidbody Ball2RB;
    public Rigidbody Ball3RB;

    public Vector3 ThrowingPower_left = new Vector3();
    public Vector3 ThrowingPower_right = new Vector3();
    public float time;

    private int totalCount = 1;     //which catch we are currently on
    private Transform BallBeingThrown;
    private Rigidbody BallBeingThrown_RB;
    private int targetArm;
    private bool ballThrown = false;

    bool BallInLeftHand = true;
    bool BallInRightHand = true;

    // Start is called before the first frame update
    void Start()
    {
        BallBeingThrown = Ball1;
        BallBeingThrown_RB = Ball1RB;

        Ball1RB.useGravity = false;
        Ball2RB.useGravity = false;
        Ball3RB.useGravity = false;
        ballThrown = true;
        StartCoroutine(Throw_One());
    }

    // Update is called once per frame
    void Update()
    {
        print(totalCount);
        findTargetArm();
        findIfBallCaught();

        if (targetArm == 1 && !BallInRightHand)
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
        else if (targetArm == 0 && !BallInLeftHand)
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

    void findTargetArm()
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

    void findBallBeingThrown()
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

    void findIfBallCaught()
    {
        float speedOfBall = BallBeingThrown_RB.velocity.magnitude;

        if (ballThrown && System.Math.Abs(speedOfBall - 0)<.5)
        {
            totalCount++;

            if (targetArm == 1)  //sets ball as a child of the target to attach it to the hand when caught
            {
                BallBeingThrown.parent = RightTarget;
                BallInRightHand = true;
                print("Right Target Parent");
            }
            else
            {
                BallBeingThrown.parent = LeftTarget;
                BallInLeftHand = true;
                print("Left Target Parent");
            }

            StartCoroutine(ThrowWhenCaught(BallBeingThrown_RB));
            findBallBeingThrown();
        }
    }


    IEnumerator ThrowWhenCaught(Rigidbody objectCaught)
    {
        yield return new WaitForSeconds(.2f);
        if (targetArm == 0)
        {
            StartCoroutine(MoveTowardsPoint(RightTarget, 4f, RightTarget.position.y,-2.3f));
            objectCaught.AddForce(ThrowingPower_right, ForceMode.Impulse);
            BallInRightHand = false;
        }
        else
        {
            StartCoroutine(MoveTowardsPoint(LeftTarget, 4f, LeftTarget.position.y, 0f));
            objectCaught.AddForce(ThrowingPower_left, ForceMode.Impulse);
            BallInLeftHand = false;
        }
        objectCaught.gameObject.transform.parent = null;
    }


    IEnumerator MoveTowardsBall(Transform Target, Transform Ball)
    {
       
        Vector3 ballLocation = new Vector3(Ball.position.x, -3.04f, Ball.position.z);

        Target.position = Vector3.MoveTowards(Target.position, ballLocation, .5f);

        yield return null;
    }
    IEnumerator MoveTowardsPoint(Transform Target,float x, float y, float z)
    {
        Vector3 GoalPoint = new Vector3(x, y, z);

        Target.position = Vector3.MoveTowards(Target.position, GoalPoint, 0.2f);

        yield return null;
    }

    IEnumerator Throw_One()
    {
        Ball1RB.useGravity = true;
        Ball1RB.AddForce(ThrowingPower_left, ForceMode.Impulse);
        yield return new WaitForSeconds(time);
        //Throws the purple ball towards the right hand and waits for 0.5 seconds
        Ball2RB.useGravity = true;
        Ball2RB.AddForce(ThrowingPower_right, ForceMode.Impulse);
        BallInRightHand = false;
        yield return new WaitForSeconds(time * 3 / 2);
        //Throws the green ball towards the left hand and waits for 0.75 seconds
        Ball3RB.useGravity = true;
        Ball3RB.AddForce(ThrowingPower_left, ForceMode.Impulse);
        BallInLeftHand = false;
        //Throws the purple ball towards the left hand
    }

}
