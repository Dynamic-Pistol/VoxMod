using OpenTK.Mathematics;

namespace VoxMod.Rendering.Objects;

public struct VoxVertex
{
    public Vector3 Position;
    public Vector2 UVs;

    public VoxVertex(float x, float y, float z, float s, float t)
    {
        Position = new Vector3(x, y, z);
        UVs = new Vector2(s, t);
    }

    public static implicit operator VoxVertex((float x, float y, float z, float s, float t) srcTuple)
    {
        return new VoxVertex(srcTuple.x, srcTuple.y, srcTuple.z, srcTuple.s, srcTuple.t);
    }
}