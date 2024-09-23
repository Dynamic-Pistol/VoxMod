using OpenTK.Mathematics;

namespace VoxMod.Rendering.Objects;

public struct VoxSimpleVertexTextured(float x, float y, float z, float s, float t)
{
    public Vector3 Position = new Vector3(x, y, z);
    public Vector2 UVs = new Vector2(s, t);

    public static implicit operator VoxSimpleVertexTextured((float x, float y, float z, float s, float t) srcTuple)
    {
        return new VoxSimpleVertexTextured(srcTuple.x, srcTuple.y, srcTuple.z, srcTuple.s, srcTuple.t);
    }
}