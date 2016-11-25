using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour {

    [SerializeField]//Makes speed show up in the inspector even though the component below is private. In this case, speed
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    [SerializeField]
    private float thrusterForce = 1000f;
    [Header("Spring settings:")]
//    [SerializeField]
    //private JointDriveMode jointMode = JointDriveMode.Position;  
    [SerializeField]
    private float jointSpring=20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();    
        SetJointSteeings(jointSpring);
    }


    void Update()
    {
        //Do not move if the pause menu is being displayed
        if (PauseMenu.isOn)
        {


           
            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero); ;
            motor.RotateCamera(0f);
            return;
        }
        
        //if(Cursor.lockState!=CursorLockMode.Locked)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
       
        


        //Calculate movement and velocity as a 3D vector
        //We use GetAxisRaw here because we want to add lerping to the movement. If we 
        //use just GetAxis,Unity is going to perform some opertaions on the values before returning them to us. GetAxisRaw gives us full control.
        float xmove = Input.GetAxis("Horizontal");//-1 to 1
        float zmove = Input.GetAxis("Vertical");//-1 to 1

        Vector3 moveHorizontal = transform.right * xmove;//Using transform instead of Vector also takes into consideration the current rotation
        Vector3 moveVertical = transform.forward * zmove;

        //Calculate final movement vector
        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", zmove);

        //Apply movement    
        motor.Move(velocity);

        //Calculate player rotation as a 3D vector
        float yrot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yrot, 0) * lookSensitivity;

        //Apply rotation
        motor.Rotate(rotation);


        //Calculate camera rotation as a 3D vector
        float xrot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xrot * lookSensitivity;

        //Apply camera rotation
        motor.RotateCamera(cameraRotationX);

        //Calculate thruster force based on player input
        Vector3 _thrusterForce = Vector3.zero;
        
        if(Input.GetButton("Jump"))
        {
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSteeings(0f);
            
        }
        else
        {
            SetJointSteeings(jointSpring);   
        }
        //Apply thruster force
        motor.ApplyThruster(_thrusterForce);

    }

    private void SetJointSteeings(float _jointSpring)
    {
      /*  joint.yDrive = new JointDrive { 
            mode = jointMode, 
            positionSpring = _jointSpring, 
            maximumForce = jointMaxForce
        };*/
    }
}
