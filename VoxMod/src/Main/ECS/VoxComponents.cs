using OpenTK.Mathematics;
using VoxMod.Rendering.Objects;

namespace VoxMod.Main.ECS;

public record struct VoxTransform{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
}

public record struct VoxPoint {
    public Vector3 Position;
    public Quaternion Rotation;
}

public record struct VoxMeshComponent{
    public VoxMesh Mesh;
}

public record struct VoxMaterialComponent {
    public VoxMaterial Material;
}

public record struct VoxCamera {
    public Matrix4 Projection;
    public Vector3 CameraUp;
    public Matrix4 View;
    public Vector3 CameraForward;
    public float Fov;
    public float Yaw;
    public float Pitch;
}

public record struct VoxPlayer;