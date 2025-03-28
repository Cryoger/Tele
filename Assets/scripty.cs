using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scripty : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D my_riggy;
    public BoxCollider2D GroundChk;
    public LayerMask ground;
    public float jumpNum = 2;
    public float jumpval = 2;
    public float yVel = 0;
    public float xVel = 0;
    public bool Grounded;
    public float speed = 3;
    public float grav = 2;
    public bool jumpy = false;
    public GameObject Square;
    public Transform playTrans;
    public Transform camTrans;
    public float offH = 1.5f;
    public float followRate = .05f;
    public string color = "blue";
    public float drag = (float).1;
    public bool shifty = false;
    public bool xdown;
    public float tpScale = 2;
    public int dir = 1;
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update() {
        jumpy = (jumpy || Input.GetKeyDown(KeyCode.Space));
        shifty = (shifty || Input.GetKeyDown(KeyCode.RightShift));
        xdown = Input.GetButton("Horizontal");
        camChase();
        

    }
    void FixedUpdate()
    {
        Grounded = (Physics2D.OverlapAreaAll(GroundChk.bounds.min,GroundChk.bounds.max,ground).Length > 0);
        float xInput = Input.GetAxis("Horizontal");
        if (xInput != 0) {
            dir = Mathf.RoundToInt((xInput/Mathf.Abs(xInput)));
        }
        
        float yInput = Input.GetAxis("Vertical");
        if (jumpy) {
            yVel = jumpUpdate();
        } else {
           float[] arr = move(xInput, yInput);
            yVel = arr[1];
            xVel = arr[0];
        }
        if (shifty) {
            Vector3 arry;
            if (xInput == 0 && yInput == 0) {
                arry = new Vector3(playTrans.position[0] + dir * tpScale, playTrans.position[1], 0);
            } else {
                arry = new Vector3((playTrans.position[0] + tpScale * xInput *
                    (1/(Mathf.Sqrt(Mathf.Pow(xInput, 2f) + Mathf.Pow(yInput, 2f))))), 
                    (playTrans.position[1] + tpScale * yInput *
                    (1/(Mathf.Sqrt(Mathf.Pow(xInput, 2f) + Mathf.Pow(yInput, 2f))))),
                    0);
            }
            playTrans.position = arry;
            shifty = false;
        }
        
        
        
        my_riggy.linearVelocity = new Vector2(xVel, yVel);
        
    }
    float[] move(float xInput, float yInput) {
        if (Grounded) {
            yVel = 0;
            jumpNum = 2;
            if (!xdown) {
                xVel = xVel * drag;
            } else {
                xVel = xInput * speed;
                //xdown = Input.GetButtonDown("Horizontal");
            }
                
            color = "red";
            Square.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else {
            
            yVel = yVel - (float)(.0416667)*grav;
            if (xdown) {
                xVel = xInput * speed;
                //xdown = false;
            }
            color = "blue";
            Square.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        float[] arr = {xVel, yVel};
        return arr;
    }
    float jumpUpdate() {
        if (jumpy && jumpNum > 0){
            yVel = jumpval;
            jumpNum -= 1;
            jumpy = false;
            Square.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else {
            jumpy = false;
        }
        return yVel;
    }
    void camChase() {
        Vector3 offset = new Vector3(0, offH, 0);
        Vector3 modPlay = playTrans.position + offset;
        if (camTrans.position != modPlay) {
            Vector3 dist = calc_dist(modPlay, camTrans.position);
            if (Mathf.Abs(dist[0]) < .01f) {
                dist[0] = 0;
                camTrans.position = new Vector3(modPlay[0], camTrans.position[1], camTrans.position[2]);
            }
            if (Mathf.Abs(dist[1]) < .01f) {
                dist[1] = 0;
                camTrans.position = new Vector3(camTrans.position[0], modPlay[1], camTrans.position[2]);
            }
            camTrans.position += new Vector3(dist[0] * followRate, dist[1] * followRate, 0f);
        }
    }
    Vector3 calc_dist(Vector3 play, Vector3 cam) {
        return (new Vector3( play[0]-cam[0], play[1]-cam[1], 0));
    }
}
