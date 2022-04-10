using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Ctrl : MonoBehaviour
{
    public GameObject player_Obj = null;
    public Camera main_Camera = null;
    Vector3 target_Pos = Vector3.zero;
    Vector3 target_Rot = Vector3.zero;
    float rot_Y = 0.0f;
    float move_Speed = 1.0f;
    float rot_Speed = 1.0f;

    void Start()
    {
        //move_Speed = (GlobalData.user_MoveLv * 0.005f) + 1.0f;
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        target_Pos = player_Obj.transform.position;
        target_Rot = this.transform.eulerAngles;

        this.transform.position = Vector3.Lerp(this.transform.position, target_Pos, 
            Time.deltaTime * move_Speed);

        if(Input.GetMouseButton(1))
        {
            rot_Y = Input.GetAxis("Mouse X") * rot_Speed;
            target_Rot.y += rot_Y;

            this.transform.eulerAngles = target_Rot;
            main_Camera.transform.LookAt(player_Obj.transform.position);
        }
    }
}
