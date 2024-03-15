using OpenTK.Mathematics;

namespace VoxMod.Rendering.Objects;

public struct VoxSimpleVertexTextured
{
    public Vector3 Position;
    public Vector2 UVs;

    public VoxSimpleVertexTextured(float x, float y, float z, float s, float t)
    {
        Position = new Vector3(x, y, z);
        UVs = new Vector2(s, t);
    }

    public static implicit operator VoxSimpleVertexTextured((float x, float y, float z, float s, float t) srcTuple)
    {
        return new VoxSimpleVertexTextured(srcTuple.x, srcTuple.y, srcTuple.z, srcTuple.s, srcTuple.t);
    }
}