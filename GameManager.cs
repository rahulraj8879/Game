using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerPrefab;
    CursorLockMode cuuu;

    // Start is called before the first frame update
    void Start()
    {

        //cuuu = CursorLockMode.Locked;

        

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PlayerPrefab!= null)
            {
                int Range = Random.Range(-10, 10);
                PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(Range, 0f, Range), Quaternion.identity);
            }
            else
            {
                Debug.Log("Place Prefab");
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            cuuu= CursorLockMode.None;
        }
    }
}
