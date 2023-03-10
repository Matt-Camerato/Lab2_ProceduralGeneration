using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float gravity = 9.8f;
    public bool canMove = true;

    [Header("Look Settings")]
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookClamp = 45f;

    [Header("References")]
    [SerializeField] private Camera playerCam;

    private Vector3 moveDir = Vector3.zero;
    private float xRot = 0f;

    private CharacterController cc;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        HandleMovement();
        HandleLooking();

        //reset player if they fall off the map
        if(transform.position.y < -10) transform.position = new Vector3(0, 5, 0);
    }

    private void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift); //left shift for sprint

        //get move speed vectors
        float speedX = canMove? (isRunning? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0f;
        float speedZ = canMove? (isRunning? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0f;

        float moveDirY = moveDir.y; //save yMoveDir speed
        moveDir = (transform.right * speedX) + (transform.forward * speedZ);

        //handle jumping
        if(Input.GetButton("Jump") && canMove && cc.isGrounded) moveDir.y = jumpHeight;
        else moveDir.y = moveDirY;

        //handle free falling
        if(!cc.isGrounded) moveDir.y -= gravity * Time.deltaTime;

        //move character controller
        cc.Move(moveDir * Time.deltaTime);
    }

    private void HandleLooking()
    {
        if(!canMove) return;

        xRot -= Input.GetAxis("Mouse Y") * lookSpeed;
        xRot = Mathf.Clamp(xRot, -lookClamp, lookClamp);
        playerCam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if player reached the goal in a level
        if(other.CompareTag("Finish"))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if(currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                //if you won last level
                Debug.Log("YOU WIN");
            }
            else SceneManager.LoadScene(currentSceneIndex + 1); //otherwise load next level
        }
    }
}
