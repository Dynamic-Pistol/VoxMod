using OpenTK.Mathematics;
using VoxMod.Rendering.Objects;

namespace VoxMod.Main.ECS;

public struct VoxTransform{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
}

public struct VoxPoint {
    public Vector3 Position;
    public Quaternion Rotation;
}

public struct VoxMeshComponent{
    public VoxMesh Mesh;
}

public struct VoxMaterialComponent {
    public VoxMaterial Material;
}

public struct VoxCamera {
    public Matrix4 Projection;
}

public struct VoxSelected {
    public bool IsSelected;
}

public struct Player;