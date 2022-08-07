using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilRunner : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float idleTime;

    Vector2 direction;
    float idleTimeCounter;
    float currentDirection; // player를 지나쳤는지 감지하기 위해.
    bool isFacingLeft = true;
    Rigidbody2D theRB;
    Animator anim;

    enum enemyState { idle, run, turn}
    enemyState currentState;

    private void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentState = enemyState.run;
        CheckingPlayerPosition();
        currentDirection = direction.x;

    }
    private void Update()
    {
        switch (currentState)
        {
            case enemyState.idle:
                Idle();
                break;
            case enemyState.run:
                CheckingPlayerPosition();
                Run();
                FacingDirection();
                Flip();
                break;
            case enemyState.turn:
                Turn();
                break;
        }
    }

    bool PastPlayer()
    {
        if (currentDirection != direction.x)
            return true;
        return false;
    }

    void CheckingPlayerPosition()
    {
        direction =
            (PlayerController.instance.transform.position - transform.position);
        if (direction.x > 0)
        {
            direction = new Vector2(1, 0);
            return;
        }
        direction = new Vector2(-1, 0);
    }
    void FacingDirection()
    {
        if (theRB.velocity.x < 0)
        {
            isFacingLeft = true;
        }
        else if (theRB.velocity.x > 0)
        {
            isFacingLeft = false;
        }
    }
    void Flip()
    {
        if (isFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180f, 0);
        }
    }
    void Run()
    {
        if (IsPlayingAnimation("Run") == false) // Enter Run State
        {
            currentDirection = direction.x;
            anim.Play("Run");
        }
        if (PastPlayer()) // Exit Run State
        {
            currentState = enemyState.idle;
        }
        theRB.velocity = new Vector2(currentDirection * moveSpeed, theRB.velocity.y);
    }
    void Idle()
    {
        if (IsPlayingAnimation("Idle") == false)  // Enter Idle State
        {
            anim.Play("Idle");
            idleTimeCounter = idleTime;
        }
        if (idleTimeCounter > 0)
        {
            idleTimeCounter -= Time.deltaTime;
        }
        else
        {
            currentState = enemyState.turn;  // Exit Idle State
        }
        theRB.velocity = new Vector2(0, theRB.velocity.y);
    }

    void Turn()
    {
        if (IsPlayingAnimation("Turn") == false) // Enter Turn State
        {
            anim.Play("Turn");
        }
        if (IsPlayingAnimation("Run")) // Exit Turn State, turn 애니가 끝나면 Run으로 Transition.
        {

            currentState = enemyState.run;
            currentDirection = direction.x;
        }
    }
    
    bool IsPlayingAnimation(string _animation)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(_animation))
            return true;
        return false;
    }

}
