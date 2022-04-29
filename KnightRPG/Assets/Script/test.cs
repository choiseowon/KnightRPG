using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
  
    public GameObject root = null;
    Ray ray;
    RaycastHit hit;
    Vector3 pos = Vector3.zero;
    Vector3 rot = Vector3.zero;
    int rayerNumber = -1;
    public GameObject grass = null;
    void Start()
    {
        rayerNumber = 1 << LayerMask.NameToLayer("Ground");
        Debug.Log(rayerNumber);

        for(int ii = 0; ii < 50; ii++)
        {
            pos = RandomPosition(0, 30, root.transform);
            this.transform.LookAt(pos);

            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, Mathf.Infinity, rayerNumber))
            {
                Debug.Log(hit.point);
                Instantiate(grass, hit.point, Quaternion.identity);
                Debug.Log(hit.collider.name);
                Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * hit.distance, Color.red);
            }
        }
    }


    void Update()
    {
    }
    Vector3 RandomPosition(float min_Pos, float max_Pos, Transform center_Tr)
    {
        Vector3 rand_Pos = Random.insideUnitCircle.normalized;
        rand_Pos.z = rand_Pos.y;
        rand_Pos.y = 0.0f;
        float radius = Random.Range(min_Pos, max_Pos);

        return (rand_Pos * radius) + center_Tr.position;
    }
}
