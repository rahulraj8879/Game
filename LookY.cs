using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookY : MonoBehaviour
{
    [SerializeField]
    private float Sensitivity = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float Mousey = Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(transform.eulerAngles.x - (Mousey * Sensitivity), transform.eulerAngles.y , transform.eulerAngles.z);
    }
}
