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
    public Vector3 CameraUp;
    public Matrix4 View;
    public Vector3 CameraForward;
    public float Fov;
}

public struct VoxSelected {
    public bool IsSelected;
}

public struct VoxPlayer;