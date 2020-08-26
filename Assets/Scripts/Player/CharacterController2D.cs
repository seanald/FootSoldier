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
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

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
    private int margin = 2;

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

        if(!grounded)
        {
            velocity = new Vector2(velocity.x, Mathf.Max(velocity.y - gravity, -maxFall));
        }

        if(velocity.y < 0)
        {
            falling = true;
        }

        if(grounded || falling)
        {
            Vector3 startPoint = new Vector3(box.xMin + margin, box.center.y, transform.position.z);
            Vector3 endPoint = new Vector3(box.xMin - margin, box.center.y, transform.position.z);

            RaycastHit hitInfo;

            float distance = box.height / 2 + (grounded ? margin : Mathf.Abs(velocity.y * Time.deltaTime));

            bool connected = false;

            for(int i = 0; i < verticalRays; i++)
            {
                float lerpAmmount = (float) i / (float) verticalRays - 1;
                Vector3 origin = Vector3.Lerp(startPoint, endPoint, lerpAmmount);

                Ray ray = new Ray(origin, Vector3.down);

                connected = Physics.Raycast(ray, out hitInfo, distance, layerMask);

                if(connected)
                {
                    grounded = true;
                    falling = false;
                    transform.Translate(Vector3.down * (hitInfo.distance - box.height / 2));
                    velocity = new Vector2(velocity.x, 0);
                }
            }

            if(!connected)
            {
                grounded = false;
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
