using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    public ChangeRange changeRange;
    // Start is called before the first frame update
    void Start()
    {
        changeRange = FindAnyObjectByType<ChangeRange>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            changeRange.Range = 1;
            transform.position = new Vector3(-57.23f, -37.31f, 0);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            changeRange.Range = 2;
            transform.position = new Vector3(-8.27f, -2.78f,0);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            changeRange.Range = 3;
            transform.position = new Vector3(58.82f, -46.75f,0);
        }
    }
}
