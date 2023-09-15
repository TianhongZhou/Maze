using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rBody;

    void Start() 
    {
        rBody = GetComponent<Rigidbody>();
    }

    void Update() 
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal, 0, vertical);

        if (dir != Vector3.zero)
        {
            rBody.velocity = dir * 3;
        }
    }
}
