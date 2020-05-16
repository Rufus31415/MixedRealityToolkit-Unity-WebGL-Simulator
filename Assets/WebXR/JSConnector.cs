using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSConnector : MonoBehaviour
{
    /*
    public void Start()
    {
        setCameraTransform("{\"x\":1.1,\"y\":2,\"z\":-3.2,\"_x\":-0.69,\"_y\":0.51,\"_z\":0.69,\"_w\":0.68}");
    }*/

    public void setCameraTransform(string msgStr)
    {
        var msg = JsonUtility.FromJson<JSCameraTransformMessage>(msgStr);

        Camera.main.transform.position = new Vector3(msg.x, msg.y, msg.z);
      

        var quat = new Quaternion(msg._x, msg._y, msg._z, msg._w);
        var euler = quat.eulerAngles;

        Camera.main.transform.rotation = Quaternion.Euler(msg.crx * euler.x, msg.cry * euler.y, msg.crz * euler.z);

        Camera.main.fieldOfView = msg.fov;
    }


}

public class JSCameraTransformMessage {

    public float x;
    public float y;
    public float z;
    public float _x;
    public float _y;
    public float _z;
    public float _w;

    public float crx;
    public float cry;
    public float crz;

    public float fov;
}