using UnityEngine;
using System.Collections;
using System;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Gravity acting on object")]
    float gravity = 6;

    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float maxSpeed = 9;

    [SerializeField, Tooltip("Max speed, in units per second, that the character falls.")]
    float maxFall = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 4;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 2;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    int layerMask;

    private Rect box;

    private Vector2 velocity;

    // Check vars
    private bool grounded;
    private bool falling;

    // Raycasting vars
    private int horizontalRays = 6;
    private int verticalRays = 4;
    private int marginPercent = 20;

    public LineRenderer lineRenderer;
    private void Start()
    {
        this.layerMask = LayerMask.NameToLayer("NormalCollisions");
    }

    private void FixedUpdate()
    {
        this.box = new Rect(
            GetComponent<Collider2D>().bounds.min.x,
            GetComponent<Collider2D>().bounds.min.y,
            GetComponent<Collider2D>().bounds.size.x,
            GetComponent<Collider2D>().bounds.size.y
            );

        // --------------- GRAVITY --------------- //
        if (!grounded)
        {
            velocity = new Vector2(velocity.x, Mathf.Max(velocity.y - gravity, -maxFall));
        }

        if(velocity.y < 0)
        {
            falling = true;
        }

        if(grounded || falling)
        {
            float margin = ((float)marginPercent / (float)100 * box.height);
            Vector2 startPoint = new Vector2(box.xMin + margin, box.center.y);
            Vector2 endPoint = new Vector2(box.xMax - margin, box.center.y);

            RaycastHit2D hitInfo;

            float distance = box.height / 2 + (grounded ? margin : Mathf.Abs(velocity.y * Time.deltaTime));

            bool connected = false;

            for(int i = 0; i < verticalRays; i++)
            {
                float lerpAmmount = (float) i / (float) (verticalRays - 1);
                Vector2 origin = Vector2.Lerp(startPoint, endPoint, lerpAmmount);

                hitInfo = Physics2D.Raycast(origin, Vector2.down, distance);
                connected = hitInfo.collider != null;

                Debug.DrawRay(origin, Vector2.down, Color.red);
                if (connected)
                {
                    grounded = true;
                    falling = false;
                    transform.Translate(Vector2.down * (hitInfo.distance - box.height / 2));
                    velocity = new Vector2(velocity.x, 0);
                    break;
                }
            }

            if(!connected)
            {
                grounded = false;
            }
        }

        // --------------- LATERAL MOVEMENT --------------- //
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float newVelocityX = velocity.x;

        if(horizontalAxis != 0)
        {
            newVelocityX += walkAcceleration * horizontalAxis;
            newVelocityX = Mathf.Clamp(newVelocityX, -maxSpeed, maxSpeed);
        }
        else if (velocity.x != 0)
        {
            int modifier = velocity.x > 0 ? -1 : 1;
            newVelocityX += groundDeceleration * modifier;
        }

        velocity = new Vector2(newVelocityX, velocity.y);

        if(velocity.x != 0)
        {
            float margin = ((float)marginPercent / (float)100 * box.width);

            Vector2 startPoint = new Vector2(box.center.x, box.yMin + margin);
            Vector2 endPoint = new Vector2(box.center.x, box.yMax - margin);

            RaycastHit2D hitInfo;

            float sideRayLength = margin + Mathf.Abs(velocity.x * Time.deltaTime);
            Vector2 direction = velocity.x > 0 ? Vector2.right : Vector2.left;

            bool connected = false;

            for (int i = 0; i < horizontalRays; i++)
            {
                float lerpAmmount = (float)i / (float) (horizontalRays - 1);
                Vector2 origin = Vector2.Lerp(startPoint, endPoint, lerpAmmount);

                hitInfo = Physics2D.Raycast(origin, direction, sideRayLength);
                connected = hitInfo.collider != null;
                Debug.DrawRay(origin, direction, Color.green);
                if (connected)
                {
                    Debug.Log("Connected");
                    transform.Translate(direction * (hitInfo.distance - box.width / 2));
                    velocity = new Vector2(0, velocity.y);
                    break;
                }
            }
        }

    }
    private void LateUpdate()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

   /* private void Update()
    {
        if (grounded)
        {
            velocity.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        }

        velocity.y += Physics2D.gravity.y * Time.deltaTime;

        float moveInput = Input.GetAxisRaw("Horizontal");
        float acceleration = grounded ? walkAcceleration : airAcceleration;
        float deceleration = grounded ? groundDeceleration : 0;

        // Update the velocity assignment statements to use our selected
        // acceleration and deceleration values.
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * moveInput, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        transform.Translate(velocity * Time.deltaTime);
        *//*Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

        grounded = false;
        foreach (Collider2D hit in hits)
        {
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                {
                    grounded = true;
                }

            }
        }*//*
    }*/
}
