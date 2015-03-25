using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float speed = 6f;

  Vector3 movement;
  Animator anim;
  Rigidbody playerRigidBody;
  int floorMask;
  float camRayLength = 100f;
  public NewWebSocketClient ws;

  
  void Awake ()
  {
    floorMask = LayerMask.GetMask ("Floor");
    anim = GetComponent <Animator> ();
    playerRigidBody = GetComponent <Rigidbody> ();
  }
  
  void FixedUpdate ()
  {
    float h = ws.GetAxis("horizontal");
    float v = ws.GetAxis("vertical");
    
    Move (h, v);
    if ( (h != 0) || (v != 0) )
      Turning (h, v);
    Animating (h, v);
  }
  
  void Move (float h, float v)
  {
    movement.Set (h, 0f, v);
    movement = movement.normalized * speed * Time.deltaTime;
    playerRigidBody.MovePosition (transform.position + movement);
  }
  
  void Turning (float h, float v) 
  {
    Vector3 playerToTarget = new Vector3(Screen.width*h, 0f, Screen.height*v) - transform.position;
    Quaternion newRotation = Quaternion.LookRotation (playerToTarget);
    playerRigidBody.MoveRotation (newRotation);

    //Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
    //RaycastHit floorHit;
    //if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
    //  Vector3 playerToMouse = floorHit.point - transform.position;
    //  playerToMouse.y = 0f;
    //  
    //  Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
    //  playerRigidBody.MoveRotation (newRotation);
    //}
  }
  
  void Animating (float h, float v)
  {
    bool walking = h != 0f || v != 0f;
    anim.SetBool ("IsWalking", walking);
  }
}
