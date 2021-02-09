using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shooter : MonoBehaviourPunCallbacks
{
    public GameObject particle;
   
    public Image HealthBar;
    public float startHealth = 100;
    private float health; 
  
    public Camera fpsCAmera;
    private Animator animator;
    private PlayerSetup playerSetup;
   
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerSetup = GetComponent<PlayerSetup>();
        health = startHealth;
        HealthBar.fillAmount = health / startHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fire()
    {

        
        RaycastHit raycastHit;
        Ray ray = fpsCAmera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out raycastHit, 100))
        {
            Debug.Log(raycastHit.collider.gameObject.name);

            photonView.RPC("CreateEffect", RpcTarget.All,raycastHit.point);

            if (raycastHit.collider.gameObject.CompareTag("Player") && !raycastHit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log(" Raycast called");
                raycastHit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
            }
        }

    }
    
    
    [PunRPC]
     public void TakeDamage( float damage)
    {
        health -= damage;
        Debug.Log(health);
        HealthBar.fillAmount = health / startHealth;
        if (health <= 0f) 
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log(" Dead ");
        if (photonView.IsMine)
        {
            animator.SetBool("isDead",true);
            StartCoroutine(respawn());
        }

    }

    IEnumerator respawn()
    {
        GameObject respawnText = GameObject.Find("respawnText");

        float respawnTime = 8.0f;
        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;

            transform.GetComponent<playerControllerScript>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are dead .respawning in " + respawnTime.ToString(".00");
        }
        animator.SetBool("isDead",false);
        respawnText.GetComponent<Text>().text = "";

        int randomPoint = Random.Range(-10,10);
        transform.position = new Vector3(randomPoint,0,randomPoint);
        transform.GetComponent<playerControllerScript>().enabled = true;
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = startHealth;
        HealthBar.fillAmount = health / startHealth;

    }
    [PunRPC]
    public void CreateEffect(Vector3 position)
    {
        GameObject gameoBj = Instantiate(particle, position, Quaternion.identity);

        Destroy(gameoBj, 2f);
    }
}
