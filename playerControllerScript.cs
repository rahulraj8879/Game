using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class playerControllerScript : MonoBehaviour
{
    public Joystick joystick;
    private RigidbodyFirstPersonController RigidbodyFirstPersonController;
    public FixedTouchField FixedTouchField;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        RigidbodyFirstPersonController = GetComponent<RigidbodyFirstPersonController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        RigidbodyFirstPersonController.joystickInputAxis.x = joystick.Horizontal;
        RigidbodyFirstPersonController.joystickInputAxis.y = joystick.Vertical;
        RigidbodyFirstPersonController.mouseLook.lookInputAxis = FixedTouchField.TouchDist;

        animator.SetFloat("Horizontal", joystick.Horizontal);
        animator.SetFloat("Vertical", joystick.Vertical);

        if (Mathf.Abs(joystick.Horizontal) > 0.9f || Mathf.Abs(joystick.Vertical) > 0.9f)
        {
            RigidbodyFirstPersonController.movementSettings.ForwardSpeed = 8;
            animator.SetBool("isRunning", true);
        }
        else
        {
            RigidbodyFirstPersonController.movementSettings.ForwardSpeed = 4;
            animator.SetBool("isRunning", false);
        }
    }
}
