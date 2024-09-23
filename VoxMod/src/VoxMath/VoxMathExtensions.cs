using OpenTK.Mathematics;

namespace VoxMod.VoxMath;

public static class VoxMathExtensions
{
    public static Quaternion ToQuaternion(this Vector3 v)
    {

        float cy = MathF.Cos(v.Z * 0.5f);
        float sy = MathF.Sin(v.Z * 0.5f);
        float cp = MathF.Cos(v.Y * 0.5f);
        float sp = MathF.Sin(v.Y * 0.5f);
        float cr = MathF.Cos(v.X * 0.5f);
        float sr = MathF.Sin(v.X * 0.5f);

        return new Quaternion
        {
            W = (cr * cp * cy + sr * sp * sy),
            X = (sr * cp * cy - cr * sp * sy),
            Y = (cr * sp * cy + sr * cp * sy),
            Z = (cr * cp * sy - sr * sp * cy)
        };

    }

    public static Vector3 ToEulerAngles(this Quaternion q)
    {
        Vector3 angles = new();

        // roll / x
        float sinrCosp = 2 * (q.W * q.X + q.Y * q.Z);
        float cosrCosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        angles.X = MathF.Atan2(sinrCosp, cosrCosp);

        // pitch / y
        float sinp = 2 * (q.W * q.Y - q.Z * q.X);
        if (MathF.Abs(sinp) >= 1)
        {
            angles.Y = MathF.CopySign(MathF.PI / 2, sinp);
        }
        else
        {
            angles.Y = MathF.Asin(sinp);
        }

        // yaw / z
        float sinyCosp = 2 * (q.W * q.Z + q.X * q.Y);
        float cosyCosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        angles.Z = MathF.Atan2(sinyCosp, cosyCosp);

        return angles;
    }
    
    public static System.Numerics.Vector3 FromSys2OpenTK(this Vector3 original)
    {
        return new(original.X, original.Y, original.Z);
    }

    public static Vector3 FromOpentk2Sys(this System.Numerics.Vector3 original)
    {
        return new(original.X, original.Y, original.Z);
    }
}
