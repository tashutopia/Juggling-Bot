using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleController : MonoBehaviour
{
    public GameObject leftArm;
    public GameObject rightArm;

    public Transform LeftTarget;
    public Transform RightTarget;

    public Transform Ball1;
    public Transform Ball2;
    public Transform Ball3;

    public Rigidbody Ball1RB;
    public Rigidbody Ball2RB;
    public Rigidbody Ball3RB;

    private int totalCount = 1;
    private Transform BallBeingThrown;
    private Rigidbody BallBeingThrown_RB;
    private int targetArm;
    private bool ballThrown = false;

    // Start is called before the first frame update
    void Start()
    {
        BallBeingThrown = Ball1;
        BallBeingThrown_RB = Ball1RB;
    }

    // Update is called once per frame
    void Update()
    {
        findTargetArm();
        findTotalCount();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ballThrown = true;
        }

        if (targetArm == 1)
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
        else if (targetArm == 0)
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

    void findTotalCount()
    {
        float speedOfBall = BallBeingThrown_RB.velocity.magnitude;
        if(ballThrown && speedOfBall == 0){
            totalCount++;
            findBallBeingThrown();
        }
    }

    IEnumerator MoveTowardsBall(Transform Target, Transform Ball)
    {
        print (totalCount);
        Vector3 ballLocation = new Vector3(Ball.position.x, -3.04f, Ball.position.z);

        Target.position = Vector3.MoveTowards(Target.position, ballLocation, 0.2f);

        yield return null;
    }

}
