using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour, ICollision
{

    [Header("Layers")]
    public LayerMask groundLayer;

    public bool onGround { get; private set; }
    public bool onWall { get; private set; }
    public bool onRightWall { get; private set; }
    public bool onLeftWall { get; private set; }
    public int wallSide { get; private set; }

    [Space]
    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        var position = transform.position;
        onGround = Physics2D.OverlapCircle((Vector2)position + bottomOffset, collisionRadius, groundLayer);
        onRightWall = Physics2D.OverlapCircle((Vector2)position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)position + leftOffset, collisionRadius, groundLayer);
        onWall = onRightWall || onLeftWall;

        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        var position = transform.position;
        Gizmos.DrawWireSphere((Vector2)position  + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)position + leftOffset, collisionRadius);
    }
}
