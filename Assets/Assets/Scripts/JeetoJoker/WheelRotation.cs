using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public static WheelRotation instance;
    public bool spin;
    public float speed;
    private int[] angle = new int[] { 0, 45, 90, 135, 180, 225, 270, 315, 360 };
    public bool isClock;

    private void Start()
    {
        spin = false;
    }
    void Update()
    {
        if (spin && speed > 0)
        {
            if (isClock)
            {

                transform.Rotate(Vector3.forward, Time.deltaTime * speed);
                speed -= 100f * Time.deltaTime;
            }
            else
            {

                transform.Rotate(Vector3.back, Time.deltaTime * speed);
                speed -= 100f * Time.deltaTime;
            }

        }
        if (spin && speed <= 0)//end of speed
        {
            for (int i = 0; i < angle.Length; i++)
            {
                if (transform.rotation.eulerAngles.z == angle[i])
                {
                    transform.Rotate(0, 0, 1);
                    break;
                }
            }
            if (!isClock)
            {

                float z = -1 * (transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Euler(0, 0, z);
            }
            speed = 0;
            spin = false;
           

        }

    }
    public void spin_button()
    {
        spin = true;
    }
}
