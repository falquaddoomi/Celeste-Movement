using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollision : MonoBehaviour, ICollision {
    [Header("Layers")]
    public LayerMask groundLayer;
    
    private const float TOLERANCE = Single.Epsilon;

    public bool onGround { get; private set; }
    public bool onWall { get; private set; }
    public bool onRightWall { get; private set; }
    public bool onLeftWall { get; private set; }
    public int wallSide { get; private set; }

    private CapsuleCollider2D checkCollider;
    private ContactFilter2D filter;
    private ContactPoint2D[] contacts;

    private void Awake() {
        checkCollider = GetComponent<CapsuleCollider2D>();

        filter = new ContactFilter2D { layerMask = groundLayer };
        contacts = new ContactPoint2D[12];
    } 

    void FixedUpdate()
    {
        onGround = false;
        onRightWall = false;
        onLeftWall = false;

        var contactCnt = checkCollider.GetContacts(filter, contacts);
        for (var i = 0; i < contactCnt; i++) {
            var c = contacts[i];

            if (Math.Abs(c.normal.y - 1) < TOLERANCE) {
                // it's in contact with the floor, so we're onGround
                onGround = true;
            }
            
            if (Math.Abs(c.normal.x - 1) < TOLERANCE) {
                // it's in contact with the floor, so we're onGround
                onLeftWall = true;
            }
            
            if (Math.Abs(c.normal.x - (-1)) < TOLERANCE) {
                // it's in contact with the floor, so we're onGround
                onRightWall = true;
            }
        }

        onWall = onRightWall || onLeftWall;
        wallSide = onRightWall ? -1 : 1;
    }
}
