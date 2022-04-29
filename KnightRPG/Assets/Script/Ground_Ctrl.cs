using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ground_Ctrl : MonoBehaviour
{
    public GameObject ray_Obj = null;
    public GameObject obstacle_Root = null;
    public GameObject[] rock_Obj = null;
    public GameObject[] stump_Obj = null;
    public GameObject[] grass_Obj = null;
    Vector3 pos = Vector3.zero;
    int rayerNumber = -1;

    void Start()
    {
        rayerNumber = 1 << LayerMask.NameToLayer("Ground");
        int obstacle_Count = 10;
        for(int ii = 0; ii <= 32; ii += 8)
        {
            obstacle_Count = obstacle_Count + ii;
            ObstacleInstantiate(grass_Obj, obstacle_Count, ii, ii + 8);
        }

        //obstacle_Count = 5;
        //for (int ii = 0; ii <= 30; ii += 10)
        //{
        //    obstacle_Count = obstacle_Count + (ii / 2);
        //    ObstacleInstantiate(rock_Obj, obstacle_Count, ii, ii + 10);
        //}
    }

    void ObstacleInstantiate(GameObject[] array, int count, float pos_Min ,float pos_Max)
    {
        RaycastHit hit;

        for (int ii = 0; ii < count; ii++)
        {
            Vector3 pos = RandomPosition(pos_Min, pos_Max, obstacle_Root.transform);
            ray_Obj.transform.LookAt(pos);
            int rand = Random.Range(0, array.Length);

            if (Physics.Raycast(ray_Obj.transform.position, ray_Obj.transform.forward, out hit, Mathf.Infinity, rayerNumber))
            {
                GameObject obj = Instantiate(array[rand]);
                obj.transform.SetParent(obstacle_Root.transform);
                obj.transform.position = hit.point;
                float randX = Random.Range(1.0f, 2.0f);
                float randY = Random.Range(1.0f, 2.0f);
                float randZ = Random.Range(1.0f, 2.0f);
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * randX,
                                    obj.transform.localScale.y * randY, obj.transform.localScale.z * randZ);
            }
        }
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
