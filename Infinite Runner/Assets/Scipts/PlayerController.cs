using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const float LANE_DISTANCE = 2.0f;
    private const float TURN_SPEED = 0.05f;

    public bool isRunning = false; 

    private Animator anim;

    private CharacterController controller; 
    private float jumpForce = 5.0f;
    private float gravity = 10.0f;
    private float verticalVelocity;
    private int desiredLane = 1;

    //Speed Modifier
    private float originalSpeed = 7.0f; 
    private float speed = 7.0f;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f; 

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>(); 
	}
	
	// Update is called once per frame
	void Update () {

        if (!isRunning)
            return;

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.instance.UpdateModifier(speed - originalSpeed);
        }


        if (Swipe.instance.SwipeRight)
            MoveLane(false);

        if (Swipe.instance.SwipeLeft)
            MoveLane(true);

        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }
        else if (desiredLane == 2 )
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        if (isGrounded)
        {
            
            verticalVelocity = -0.1f;

            if (Swipe.instance.SwipeUp)
            {
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }

            else if (Swipe.instance.SwipeDown)
            {
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            if (Swipe.instance.SwipeDown)
            {
                verticalVelocity = -jumpForce; 
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        controller.Move(moveVector * Time.deltaTime);

        Vector3 dir = controller.velocity;
        dir.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
	}

    private void MoveLane(bool goingRight)
    {

        desiredLane += (goingRight) ? -1 : 1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z), Vector3.down);

        return (Physics.Raycast(groundRay, 0.2f + 0.1f));

        
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }
    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void Crash()
    {
        anim.SetTrigger("Death");
        isRunning = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;

        }
    }
}
