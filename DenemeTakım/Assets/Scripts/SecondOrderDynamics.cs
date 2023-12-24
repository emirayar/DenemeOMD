using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamics : MonoBehaviour
{
    private Vector2 xp; // previous input
    private Vector2 y, yd; // state variables
    private float k1, k2, k3; // dynamics constants
    private const float PI = Mathf.PI;

    public void SecondOrderDynamics(float f, float z, float r, Vector2 x0)
    {
        // compute constants
        k1 = z / (PI * f);
        k2 = 1 / ((2 * PI * f) * (2 * PI * f));
        k3 = r * z / (2 * PI * f);
        // initialize variables
        xp = x0;
        y = x0;
        yd = Vector2.zero;
    }
    public Vector2 CustomUpdate(float T, Vector2 x, Vector2 xd = default)
    {
        if (xd == null)
        { // estimate velocity
            xd = (x - xp) / T;
            xp = x;
        }
        y = y + T * yd; //integrate position by velocity
        yd = yd + T * (x + k3 * xd - y - k1 * yd) / k2; ////integrate position by acceleration
        return y;
    }
}