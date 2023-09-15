using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinControl : MonoBehaviour
{
    private CreateMap createMap;

    // Start is called before the first frame update
    void Start()
    {
        createMap = GameObject.FindGameObjectWithTag("Maze").GetComponent<CreateMap>();
        transform.position = createMap.end;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Application.LoadLevel(0);
        }
    }
}
