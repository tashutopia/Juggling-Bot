using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuggleController : CatchDetector
{
    public Transform LeftTarget;
    public Transform RightTarget;
    public Text countText;

    public Transform Ball1;
    public Transform Ball2;
    public Transform Ball3;

    public Rigidbody Ball1RB;
    public Rigidbody Ball2RB;
    public Rigidbody Ball3RB;

    public Vector3 ThrowingPowerRightwards = new Vector3();
    public Vector3 ThrowingPowerLeftwards = new Vector3();
    public float time = 0.95f;
    public float dwellTime;
    public float startZPosition = 3f;

    public float positionThrowArmSpeed = 0.5f;
    public float velocityThrowArmSpeed = 0.2f;

    public bool limitZPosition = false;
    public bool limitXPosition = false;
    public bool nudgeTest = false;
    public bool varyThrowingPower = false;

    private float timeCounter;
    public float moveBackSpeed = 6;
    public float height = 2.5f;
    public float moveBackMaxDelta = 0.35f;

    public static int totalCount = 1;     // which catch we are currently on
    private int targetArm;                // which arm are we throwing to (0 is left, 1 is right)
    private bool ballInLeftHand;
    private bool ballInRightHand;
    private bool firstCycleDone;

    private Transform BallBeingThrown;
    private Rigidbody BallBeingThrown_RB;
    private Vector3 LeftStartingPoint;
    private Vector3 RightStartingPoint;

    private int count = 0;      // serves as an index for the position & velocity Lists
    private Vector3 CalculatedBallPosition;
    private List<Vector3> positionList = new List<Vector3>();
    private List<Vector3> velocityList = new List<Vector3>();


    // Start is called before the first frame update
    void Start()
    {
        LeftStartingPoint = new Vector3(6f, LeftTarget.position.y, startZPosition);
        RightStartingPoint = new Vector3(6f, RightTarget.position.y, -1 * startZPosition);

        LeftTarget.position = LeftStartingPoint;
        RightTarget.position = RightStartingPoint;

        BallBeingThrown = Ball1;
        BallBeingThrown_RB = Ball1RB;

        ballInLeftHand = true;
        ballInRightHand = true;
        Ball1RB.useGravity = false;
        Ball2RB.useGravity = false;
        Ball3RB.useGravity = false;
        firstCycleDone = false;

        SetCountText();
        StartCoroutine(ThrowOne());

    }

    // Update is called once per frame
    void Update()
    {
        FindTargetArm();
        FindIfBallCaught();

        if (targetArm == 1 && !ballInRightHand)
        {
            if (limitZPosition)
                CalculatePositionOfBall(RightTarget);
            else
                StartCoroutine(MoveTowardsBall(RightTarget, BallBeingThrown));
        }
        else if (targetArm == 0 && !ballInLeftHand)
        {
            if (limitZPosition)
                CalculatePositionOfBall(LeftTarget);
            else
                StartCoroutine(MoveTowardsBall(LeftTarget, BallBeingThrown));
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

    void FindBallBeingThrown()   //sets BallBeingThrown (Target) and BallBeingThrown_RB (Rigidbody)
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
            count++;
            SetCountText();

            BallBeingThrown_RB.velocity = new Vector3(0, 0, 0);
            BallBeingThrown_RB.useGravity = false;

            if (limitZPosition)
            {
                print("Calculated final pos: " + CalculatedBallPosition);
                print("Actual final pos: " + BallBeingThrown.position);
            }

            if (targetArm == 1)  //sets ball as a child of the target to attach it to the hand when caught
            {
                BallBeingThrown.parent = RightTarget;
                ballInRightHand = true;
                // StartCoroutine(MoveTowardsPoint(RightTarget, RightStartingPoint));
                StartCoroutine(MoveTowardsElliptical(RightTarget, RightStartingPoint));
            }
            else
            {
                BallBeingThrown.parent = LeftTarget;
                ballInLeftHand = true;
                // StartCoroutine(MoveTowardsPoint(LeftTarget, LeftStartingPoint));
                StartCoroutine(MoveTowardsElliptical(LeftTarget, LeftStartingPoint));
            }

            StartCoroutine(ThrowWhenCaught(BallBeingThrown_RB));
            FindBallBeingThrown();

            if (nudgeTest)
            {
                int affectedBall = Random.Range(0, 2) + 1;
                print(affectedBall);
                float uporDown = .25f * (Random.Range(0, 2) - 1);
                if (affectedBall == 1)
                {
                    Ball1.position = new Vector3(Ball1.position.x, Ball1.position.y, Ball1.position.z + uporDown);
                }
                if (affectedBall == 2)
                {
                    Ball2.position = new Vector3(Ball2.position.x, Ball2.position.y, Ball2.position.z + uporDown);
                }
                if (affectedBall == 3)
                {
                    Ball3.position = new Vector3(Ball3.position.x, Ball3.position.y, Ball3.position.z + uporDown);
                }
            }

            CatchDetector.BallIsInHand = false;
        }
    }


    void CalculatePositionOfBall(Transform Target)
    {
        Vector3 initialVelocity = velocityList[count];
        Vector3 initialPosition = positionList[count];
        float zVelocity = initialVelocity.z;
        float yVelocity = initialVelocity.y;

        float trajectoryTime = (2.0f * yVelocity) / 9.81f;
        float deltaZ = trajectoryTime * zVelocity;
        float zPosition = deltaZ + initialPosition.z;

        // print("zVelocity: " + zVelocity);
        // print("yVelocity: " + yVelocity);
        // print("initial pos:" + initialPosition);
        // print("delta z: " + deltaZ);
        // print("predicted z: " + zPosition);

        if (limitXPosition)
            CalculatedBallPosition = new Vector3(TriggerSensor.BallPosition.x, Target.position.y, zPosition);
        else
            CalculatedBallPosition = new Vector3(BallBeingThrown.position.x, Target.position.y, zPosition);

        StartCoroutine(MoveTowardsPoint(Target, CalculatedBallPosition));
    }

    IEnumerator MoveTowardsBall(Transform Target, Transform Ball)
    {
        Vector3 BallLocation = new Vector3(Ball.position.x, -3.04f, Ball.position.z);

        while (Target.position != BallLocation)
        {
            Target.position = Vector3.MoveTowards(Target.position, BallLocation, positionThrowArmSpeed);
        }

        yield return null;
    }

    IEnumerator MoveTowardsPoint(Transform Target, Vector3 GoalPoint)
    {
        while (Target.position != GoalPoint)
        {
            Target.position = Vector3.MoveTowards(Target.position, GoalPoint, velocityThrowArmSpeed);
            yield return null;
        }

        yield return null;
    }

    IEnumerator MoveTowardsElliptical(Transform Target, Vector3 GoalPoint)
    {
        timeCounter = 0;

        float initial = Target.position.z;

        float x = GoalPoint.x;
        float y;
        float z = GoalPoint.z;

        while(Target.position.z != GoalPoint.z)
        {
            timeCounter += Time.deltaTime * moveBackSpeed;

            if(timeCounter < 0.1 * moveBackSpeed)
                y = -1 * Mathf.Cos(timeCounter) * height - 3.04f;
            else if((int)Target.position.y != -3)
                y = -1 * Mathf.Cos(timeCounter) * height - 3.04f;
            else
                y = Target.position.y;

            Target.position = Vector3.MoveTowards(Target.position, new Vector3 (x, y, z), moveBackMaxDelta);
            yield return null;
        }

        while(Target.position.y != -3.04f)
        {
            Target.position = Vector3.MoveTowards(Target.position, new Vector3 (Target.position.x, -3.04f, Target.position.z), moveBackMaxDelta);
            yield return null;
        }

        yield return null;
    }

    IEnumerator ThrowOne() // First Cycle
    {
        yield return null;
        yield return null;

        // Records the balls' initial positions
        positionList.Add(Ball1.position);
        positionList.Add(Ball2.position);
        positionList.Add(Ball3.position);

        // Throws Ball 1 towards the right hand and waits for <time> seconds
        Ball1RB.useGravity = true;
        Ball1RB.AddForce(ThrowingPowerRightwards, ForceMode.Impulse);
        yield return null;
        yield return null;
        velocityList.Add(Ball1RB.velocity);     // Records ball's initial velocity
        yield return new WaitForSeconds(time);

        // Throws Ball 2 towards the left hand and waits for <time> seconds
        Ball2RB.useGravity = true;
        Ball2RB.AddForce(ThrowingPowerLeftwards, ForceMode.Impulse);
        ballInRightHand = false;
        yield return null;
        yield return null;
        velocityList.Add(Ball2RB.velocity);     // Records ball's initial velocity
        yield return new WaitForSeconds(time);

        // Throws Ball 3 towards the right hand
        Ball3RB.useGravity = true;
        Ball3RB.AddForce(ThrowingPowerRightwards, ForceMode.Impulse);
        ballInLeftHand = false;
        yield return null;
        yield return null;
        velocityList.Add(Ball3RB.velocity);     // Records ball's initial velocity

        // End of first cycle
        firstCycleDone = true;
    }

    IEnumerator ThrowWhenCaught(Rigidbody objectCaught)
    {
        yield return new WaitForSeconds(dwellTime);
        objectCaught.useGravity = true;
        positionList.Add(objectCaught.gameObject.transform.position);       // Records ball's initial position


        if (!varyThrowingPower)
        {
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
        }
        else
        {
            Vector3 randomVary = .4f * Random.onUnitSphere;
            randomVary.x = 0f;
            randomVary.y = 0f;

            if (targetArm == 0)
            {
                objectCaught.AddForce(ThrowingPowerLeftwards + randomVary, ForceMode.Impulse);
                ballInRightHand = false;
            }
            else
            {
                objectCaught.AddForce(ThrowingPowerRightwards + randomVary, ForceMode.Impulse);
                ballInLeftHand = false;
            }
        }
        yield return null;
        yield return null;

        velocityList.Add(objectCaught.velocity);            // Records ball's initial velocity
        objectCaught.gameObject.transform.parent = null;
    }

    void SetCountText()
    {
        countText.text = "Number of Completed Catches: " + count.ToString();
    }
}
