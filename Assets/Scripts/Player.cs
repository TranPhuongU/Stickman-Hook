using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Sprites Player")]
    [SerializeField] Sprite ballSprite;
    [SerializeField] Sprite stopSprite;
    [SerializeField] Sprite goSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite winSprite;

    [Header("Components")]
    private HingeJoint2D hJoint;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;

    [Header("Anchor")]
    [SerializeField] private GameObject anchor;

    [Header("Variable Private")]
    private int lastBestPosJoint;
    private int lastBestPosSelected;
    private int touches;
    private int bestPos;
    private float bestDistance;
    private Vector3 actualJoinPos;

    [Header("Public variables")]
    [SerializeField] private float gravityRope = 2f;
    [SerializeField] private float gravityAir = 0.5f;
    [SerializeField] private float factorX = 1.2f;
    [SerializeField] private float factorY = 1;

    [Header("Bool")]
    private bool sticked = false;
    private bool won = false;

    private void Start()
    {
        hJoint = GetComponent<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        lastBestPosJoint = 0;
        lastBestPosSelected = 0;
        touches = 0;

        won = false;

        anchor.transform.GetChild(lastBestPosJoint).gameObject.GetComponent<JointAnchor>().Selected();
    }

    private void Update()
    {
        bestPos = 0;
        bestDistance = float.MaxValue;

        for (int i = 0; i < anchor.transform.childCount; i++)
        {
            float actualDistance = Vector2.Distance(gameObject.transform.position, anchor.transform.GetChild(i).transform.position);
            if(actualDistance < bestDistance)
            {
                bestPos = i;
                bestDistance = actualDistance;
            }
        }
        if(!won)
            CheckInput();

        if (sticked)
        {
            lineRenderer.SetPosition(0, gameObject.transform.position);
            lineRenderer.SetPosition(1, actualJoinPos);

            ChangeSprite();
        }

        if(lastBestPosSelected != bestPos)
        {
            anchor.transform.GetChild(lastBestPosSelected).gameObject.GetComponent<JointAnchor>().Unselected();
            anchor.transform.GetChild(bestPos).gameObject.GetComponent<JointAnchor>().Selected();
        }
            lastBestPosSelected = bestPos;
    }

    private void CheckInput()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)|| ((Input.touchCount > 0) && (touches == 0)))
        {
            lineRenderer.enabled = true;
            hJoint.enabled = true;
            rb.gravityScale = gravityRope;

            hJoint.connectedBody = anchor.transform.GetChild(bestPos).transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>();
            actualJoinPos = anchor.transform.GetChild(bestPos).gameObject.transform.position;
            anchor.transform.GetChild(bestPos).gameObject.GetComponent<JointAnchor>().SetSticked();
            anchor.transform.GetChild(bestPos).gameObject.GetComponent<JointAnchor>().Unselected();

            lastBestPosJoint = bestPos;
            rb.angularVelocity = 0;
            sticked = !sticked;

        }

        if(Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) ||((Input.touchCount == 0) && (touches > 0)))
        {
            lineRenderer.enabled = false;
            hJoint.enabled = false;
            rb.velocity = new Vector2(rb.velocity.x * factorX, rb.velocity.y + factorY);
            rb.gravityScale = gravityAir;

            anchor.transform.GetChild(lastBestPosJoint).gameObject.GetComponent<JointAnchor>().SetUnsticked();

            if(bestPos == lastBestPosJoint)
            {
                anchor.transform.GetChild(bestPos).gameObject.GetComponent<JointAnchor>().Selected();
                anchor.transform.GetChild(lastBestPosJoint).gameObject.GetComponent<JointAnchor>().Unselected();
            }

            spriteRenderer.sprite = ballSprite;
            rb.AddTorque(-rb.velocity.magnitude);
            sticked = !sticked;
        }
        touches = Input.touchCount;
    }

    private void ChangeSprite()
    {
        if(rb.velocity.x  > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        //if(rb.velocity.x < 0.7f && rb.velocity.x > -0.7f && gameObject.transform.position.y < actualJoinPos.y)
        //{
        //    spriteRenderer.sprite = stopSprite;
        //}
        if (rb.velocity.x == 0)
        {
            spriteRenderer.sprite = stopSprite;
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                spriteRenderer.sprite = goSprite;
            }
            else
                spriteRenderer.sprite = backSprite;
        }

        gameObject.transform.eulerAngles = lookAt2d(actualJoinPos - gameObject.transform.position);
    }

    public Vector3 lookAt2d(Vector3 vec)
    {
        return new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, Vector2.SignedAngle(Vector2.up, vec));
    }

    public bool GetSticked()
    {
        return sticked;
    }

    public void ResetGame(Vector3 innitPos)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        gameObject.transform.position = innitPos;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void Win(float speedWin)
    {
        won = true;
        spriteRenderer.flipX = false;
        rb.gravityScale = 0;
        gameObject.transform.eulerAngles = lookAt2d(rb.velocity);
        rb.velocity = rb.velocity.normalized * speedWin;
        rb.angularVelocity = 0f;
        spriteRenderer.sprite = winSprite;
    }
}
