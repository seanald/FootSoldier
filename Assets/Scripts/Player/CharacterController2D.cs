﻿using UnityEngine;
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
    private bool lastInput;
    private float jumpPressedTime;

    // Raycasting vars
    private int horizontalRays = 6;
    private int verticalRays = 4;
    private int marginPercent = 20;
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

            RaycastHit2D[] hitInfoArray = new RaycastHit2D[verticalRays];

            float smallFraction = Mathf.Infinity;
            int indexUsed = 0;

            float distance = box.height / 2 + (grounded ? margin : Mathf.Abs(velocity.y * Time.deltaTime));

            bool connected = false;

            for(int i = 0; i < verticalRays; i++)
            {
                float lerpAmmount = (float) i / (float) (verticalRays - 1);
                Vector2 origin = Vector2.Lerp(startPoint, endPoint, lerpAmmount);

                hitInfoArray[i] = Physics2D.Raycast(origin, Vector2.down, distance);
                connected = hitInfoArray[i].fraction > 0;

                Debug.DrawRay(origin, Vector2.down, Color.red);
                if (connected)
                {
                    if(hitInfoArray[i].fraction < smallFraction)
                    {
                        indexUsed = i;
                        smallFraction = hitInfoArray[i].fraction;
                    }
                }
            }

            if (connected)
            {
                grounded = true;
                falling = false;
                transform.Translate(Vector2.down * (hitInfoArray[indexUsed].fraction * distance - box.height / 2));
                velocity = new Vector2(velocity.x, 0);
            }
            else
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

            Vector2 startPoint = new Vector2(box.center.x, box.yMin);
            Vector2 endPoint = new Vector2(box.center.x, box.yMax);

            RaycastHit2D[] hitInfoArray = new RaycastHit2D[horizontalRays];
            int ammountConnected = 0;
            float lastFraction = 0;

            float sideRayLength = box.width/2 + Mathf.Abs(velocity.x * Time.deltaTime);
            Vector2 direction = velocity.x > 0 ? Vector2.right : Vector2.left;

            bool connected = false;

            for (int i = 0; i < horizontalRays; i++)
            {
                float lerpAmmount = (float)i / (float) (horizontalRays - 1);
                Vector2 origin = Vector2.Lerp(startPoint, endPoint, lerpAmmount);

                hitInfoArray[i] = Physics2D.Raycast(origin, direction, sideRayLength);
                connected = hitInfoArray[i].fraction > 0;
                Debug.DrawRay(origin, direction, Color.green);
                if (connected)
                {
                    if(lastFraction > 0)
                    {
                        float angle = Vector2.Angle(hitInfoArray[i].point - hitInfoArray[i - 1].point, Vector2.right);
                        Debug.Log("angle=" + angle);
                        if(Mathf.Abs(angle - 90) < 40)
                        {
                            transform.Translate(direction * (hitInfoArray[i].fraction * sideRayLength - box.width / 2));
                            velocity = new Vector2(0, velocity.y);
                            break;
                        }
                    }

                    ammountConnected++;
                    lastFraction = hitInfoArray[i].fraction;
                }
            }
        }

    }
    private void LateUpdate()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    void Update()
    {
        bool input = Input.GetButton("Jump");
        if (input && !lastInput)
        {
            jumpPressedTime = Time.time;
        }
        else if (!input)
        {
            jumpPressedTime = 0;
        }

        if (grounded && Time.time - jumpPressedTime < 0.1)
        {
            grounded = false;
            velocity = new Vector2(velocity.x, jumpHeight);
            jumpPressedTime = 0;
        }

        lastInput = input;
    }
}
