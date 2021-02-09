using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject[] FpsGameobjects;
    [SerializeField]
    GameObject[] soldierGameObjects;
            
    public GameObject playerUiPrefab;
    public Camera fpsCamera;
    private playerControllerScript playerControllerScript;
    private Animator animator;
    private Shooter shooter;
  

    // Start is called before the first frame update
    void Start()
    {
        shooter= GetComponent<Shooter>();    
        animator = GetComponent<Animator>();
        playerControllerScript = GetComponent<playerControllerScript>();
        
        if (photonView.IsMine)
        {
            foreach (GameObject gameObject in FpsGameobjects)
            {
                gameObject.SetActive(true);
            }
            foreach (GameObject gameObject in soldierGameObjects)
            {
                gameObject.SetActive(false);
            }
            animator.SetBool("isSoldier", false);
            GameObject gameObject1 = Instantiate(playerUiPrefab);
           
            playerControllerScript.joystick = gameObject1.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            playerControllerScript.FixedTouchField = gameObject1.transform.Find("RotateTouchField").GetComponent<FixedTouchField>();
            
            gameObject1.transform.Find("ShootButton").GetComponent<Button>().onClick.AddListener(()=>shooter.Fire());
            
            fpsCamera.enabled = true;
        }
        else
        {
            foreach (GameObject gameObject in FpsGameobjects)
            {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in soldierGameObjects)
            {
                gameObject.SetActive(true);
            }
            animator.SetBool("isSoldier", true);
            playerControllerScript.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
