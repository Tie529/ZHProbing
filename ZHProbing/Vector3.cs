using System;
using NXOpen;


public static class Expand
{
    public static Vector3 ToVector3(this NXOpen.Point3d p3d)
    {
        return new Vector3(p3d.X, p3d.Z, p3d.Y);
    }

    public static Point ToNXPoint(this Vector3 v3)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;
        return workPart.Points.CreatePoint(new Point3d(v3.x, v3.z, v3.y));
    }

    public static Point3d ToNXPoint3d(this Vector3 v3)
    {
        return new Point3d(v3.x, v3.z, v3.y);
    }

    public static Point3d ToABSPoint3d(this Point3d p3)
    {
        return new Point3d(Math.Abs(p3.X), Math.Abs(p3.Y), Math.Abs(p3.Z));
    }

    public static string ToString(this Point3d  v3)
    {
        return $"={v3.X},{v3.Y},{v3.Z}";
    }
}



public struct Vector3
{
    public double x;
    public double y;
    public double z;

   public Vector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3 operator *(Vector3 a, double d)
    {
        return new Vector3(a.x * d, a.y * d, a.z * d);
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3 operator /(Vector3 a, double d)
    {
        return new Vector3(a.x / d, a.y / d, a.z / d);
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public double magnitude
    {
        get
        {
            return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }
    }
    public static Vector3 right
    {
        get
        {
            return new Vector3(1f, 0f, 0f);
        }
    }
    public static Vector3 forward
    {
        get
        {
            return Vector3.forwardVector;
        }
    }

    public static Vector3 zero
    {
        get
        {
            return Vector3.zeroVector;
        }
    }



    public static Vector3 Normalize(Vector3 value)
    {
        double num = Vector3.Magnitude(value);
        bool flag = num > 1E-05f;
        Vector3 result;
        if (flag)
        {
            result = value / num;
        }
        else
        {
            result = Vector3.zero;
        }
        return result;
    }

    public Vector3 normalized
    {
        get
        {
            return Vector3.Normalize(this);
        }
    }

    public static double Magnitude(Vector3 vector)
    {
        return Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }

    public static double Dot(Vector3 lhs, Vector3 rhs)
    {
        return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
    }

    public static double Clamp(double value, double min, double max)
    {
        if (value < min)
        {
            value = min;
        }
        else if (value > max)
        {
            value = max;
        }
        return value;
    }

    public static double Angle(Vector3 from, Vector3 to)
    {
        return System.Math.Acos( Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f) ) * 57.29578f;
    }

    private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);

    private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);
}



