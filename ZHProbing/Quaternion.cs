using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public struct Quaternion
{

    public float x;

    public float y;

    public float z;

    public float w;

    public Quaternion(float x,float y,float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public static Quaternion Euler(float x, float y, float z)
    {
        return get_quaternion_from_euler(x * 0.017453292f,y * 0.017453292f, z * 0.017453292f);
    }

    static Quaternion get_quaternion_from_euler(float roll, float pitch, float yaw)
    {

        double x, y, z, w;

        x = System.Math.Sin(roll / 2) * System.Math.Cos(pitch / 2) * System.Math.Cos(yaw / 2) 
            - System.Math.Cos(roll / 2) * System.Math.Sin(pitch / 2) * System.Math.Sin(yaw / 2);

        y = System.Math.Cos(roll / 2) * System.Math.Sin(pitch / 2) * System.Math.Cos(yaw / 2) 
            + System.Math.Sin(roll / 2) * System.Math.Cos(pitch / 2) * System.Math.Sin(yaw / 2);

        z = System.Math.Cos(roll / 2) * System.Math.Cos(pitch / 2) * System.Math.Sin(yaw / 2) 
            - System.Math.Sin(roll / 2) * System.Math.Sin(pitch / 2) * System.Math.Cos(yaw / 2);

        w = System.Math.Cos(roll / 2) * System.Math.Cos(pitch / 2) * System.Math.Cos(yaw / 2) 
            + System.Math.Sin(roll / 2) * System.Math.Sin(pitch / 2) * System.Math.Sin(yaw / 2);

        return new Quaternion((float)x, (float)y, (float)z, (float)w);
    }

    public static Vector3 operator *(Quaternion rotation, Vector3 point)
    {
        float num = rotation.x * 2f;
        float num2 = rotation.y * 2f;
        float num3 = rotation.z * 2f;
        float num4 = rotation.x * num;
        float num5 = rotation.y * num2;
        float num6 = rotation.z * num3;
        float num7 = rotation.x * num2;
        float num8 = rotation.x * num3;
        float num9 = rotation.y * num3;
        float num10 = rotation.w * num;
        float num11 = rotation.w * num2;
        float num12 = rotation.w * num3;
        Vector3 result;
        result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
        result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
        result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
        return result;
    }
}

