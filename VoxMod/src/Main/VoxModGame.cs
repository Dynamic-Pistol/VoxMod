using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Arch.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpGLTF.Schema2;
using StbImageSharp;
using VoxMod.GUI;
using VoxMod.Main.ECS;
using VoxMod.Rendering.Objects;
using VoxMod.VoxMath;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace VoxMod.Main;

public class VoxModGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private readonly World _world = World.Create();
    private QueryDescription _queryDescription = new QueryDescription().WithAll<VoxPoint, VoxCamera>();

    //private ImGuiController _controller = default!;

    private readonly VoxSimpleVertexTextured[] _vertices =
    [

        (-0.5f, -0.5f, -0.5f, 0.0f, 0.0f),
        (0.5f, -0.5f, -0.5f, 1.0f, 0.0f),
        (0.5f, 0.5f, -0.5f, 1.0f, 1.0f),
        (0.5f, 0.5f, -0.5f, 1.0f, 1.0f),
        (-0.5f, 0.5f, -0.5f, 0.0f, 1.0f),
        (-0.5f, -0.5f, -0.5f, 0.0f, 0.0f),

        (-0.5f, -0.5f, 0.5f, 0.0f, 0.0f),
        (0.5f, -0.5f, 0.5f, 1.0f, 0.0f),
        (0.5f, 0.5f, 0.5f, 1.0f, 1.0f),
        (0.5f, 0.5f, 0.5f, 1.0f, 1.0f),
        (-0.5f, 0.5f, 0.5f, 0.0f, 1.0f),
        (-0.5f, -0.5f, 0.5f, 0.0f, 0.0f),

        (-0.5f, 0.5f, 0.5f, 1.0f, 0.0f),
        (-0.5f, 0.5f, -0.5f, 1.0f, 1.0f),
        (-0.5f, -0.5f, -0.5f, 0.0f, 1.0f),
        (-0.5f, -0.5f, -0.5f, 0.0f, 1.0f),
        (-0.5f, -0.5f, 0.5f, 0.0f, 0.0f),
        (-0.5f, 0.5f, 0.5f, 1.0f, 0.0f),

        (0.5f, 0.5f, 0.5f, 1.0f, 0.0f),
        (0.5f, 0.5f, -0.5f, 1.0f, 1.0f),
        (0.5f, -0.5f, -0.5f, 0.0f, 1.0f),
        (0.5f, -0.5f, -0.5f, 0.0f, 1.0f),
        (0.5f, -0.5f, 0.5f, 0.0f, 0.0f),
        (0.5f, 0.5f, 0.5f, 1.0f, 0.0f),

        (-0.5f, -0.5f, -0.5f, 0.0f, 1.0f),
        (0.5f, -0.5f, -0.5f, 1.0f, 1.0f),
        (0.5f, -0.5f, 0.5f, 1.0f, 0.0f),
        (0.5f, -0.5f, 0.5f, 1.0f, 0.0f),
        (-0.5f, -0.5f, 0.5f, 0.0f, 0.0f),
        (-0.5f, -0.5f, -0.5f, 0.0f, 1.0f),

        (-0.5f, 0.5f, -0.5f, 0.0f, 1.0f),
        (0.5f, 0.5f, -0.5f, 1.0f, 1.0f),
        (0.5f, 0.5f, 0.5f, 1.0f, 0.0f),
        (0.5f, 0.5f, 0.5f, 1.0f, 0.0f),
        (-0.5f, 0.5f, 0.5f, 0.0f, 0.0f),
        (-0.5f, 0.5f, -0.5f, 0.0f, 1.0f)
    ];
    
    private readonly Vector3[] _cubePositions =
    [
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(2.0f, 5.0f, -15.0f),
        new Vector3(-1.5f, -2.2f, -2.5f),
        new Vector3(-3.8f, -2.0f, -12.3f),
        new Vector3(2.4f, -0.4f, -3.5f),
        new Vector3(-1.7f, 3.0f, -7.5f),
        new Vector3(1.3f, -2.0f, -2.5f),
        new Vector3(1.5f, 2.0f, -2.5f),
        new Vector3(1.5f, 0.2f, -1.5f),
        new Vector3(-1.3f, 1.0f, -1.5f)
    ];

    private const string VertexShader = """
            #version 330 core 
            layout (location = 0) in vec3 aPosition;
            layout (location = 1) in vec2 aTexCoord;
            out vec2 TexCoord;
            
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 proj;
    
            void main()
            {
                gl_Position = vec4(aPosition, 1.0) * model * view * proj;
                TexCoord = aTexCoord;
            }
    """;

    private const string FragmentShader = """
        #version 330 core
        out vec4 FragColor;
        
        in vec2 TexCoord;
        
        uniform sampler2D texture0;
        uniform sampler2D texture1;
        
        void main()
        {
            FragColor = mix(texture(texture0, TexCoord), texture(texture1, TexCoord), 0.2);
        }
        """;
    private uint _vao;
    private uint _vbo;
    // private uint _buffer;
    private int _texture1, _texture2;
    private VoxMaterial _mat = default!;
    private uint[] _indices = [];
    
    protected override void OnLoad()
    {
        
        base.OnLoad();
        //CursorState = CursorState.Grabbed;
        //_controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        
        GL.DebugMessageCallback(DebugMessages, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.Enable(EnableCap.DepthTest);
        
        _mat = new VoxMaterial(new Dictionary<ShaderType, string>
        {
            {ShaderType.VertexShader, VertexShader},
            {ShaderType.FragmentShader, FragmentShader}
            
        });
        
        GL.GenVertexArrays(1, out _vao);
        GL.BindVertexArray(_vao);
        
        GL.GenBuffers(1, out _vbo);
        GL.BindBuffer(BufferTarget.ArrayBuffer ,_vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf<VoxSimpleVertexTextured>() * _vertices.Length, _vertices, BufferUsageHint.StaticDraw);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<VoxSimpleVertexTextured>(), 0);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<VoxSimpleVertexTextured>(), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        
        /*
        //old exercise
        GL.CreateBuffers(1, out _buffer);
        GL.NamedBufferStorage(_buffer, sizeI + sizeV, 0, BufferStorageFlags.DynamicStorageBit);
        GL.NamedBufferSubData(_buffer, 0, sizeI, _indices);
        GL.NamedBufferSubData(_buffer, sizeI, sizeV, _vertices);
        
        GL.CreateVertexArrays(1, out _vao);
        
        GL.VertexArrayElementBuffer(_vao, _buffer);
        GL.VertexArrayVertexBuffer(_vao, 0, _buffer, sizeI, Marshal.SizeOf<VoxSimpleVertexTextured>());
        GL.EnableVertexArrayAttrib(_vao, 0);
        GL.EnableVertexArrayAttrib(_vao, 1);
        GL.VertexArrayAttribFormat(_vao, 0,  3, VertexAttribType.Float, false, 
            (uint)Marshal.OffsetOf<VoxSimpleVertexTextured>(nameof(VoxSimpleVertexTextured.Position)));
        GL.VertexArrayAttribFormat(_vao, 1, 2, VertexAttribType.Float, false, 
            (uint)Marshal.OffsetOf<VoxSimpleVertexTextured>(nameof(VoxSimpleVertexTextured.UVs)));
        GL.VertexArrayAttribBinding(_vao, 0, 0);
        GL.VertexArrayAttribBindinwog(_vao, 1, 0);
        */
        //StbImage.stbi_set_flip_vertically_on_load(1);

        _texture1 = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _texture1);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        
        using (var stream = File.OpenRead(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Assets/container.jpg"))
        {
            var result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, result.Width, result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, result.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        
        
        _texture2 = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _texture2);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        using (var stream = File.OpenRead(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Assets/awesomeface.png"))
        {
            var result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, result.Width, result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, result.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        _mat.Use();
        _mat.SetIntUniform("texture0", 0);
        _mat.SetIntUniform("texture1", 1);
    }

    private Vector2 _moveInput;
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _moveInput.X = Convert.ToInt32(KeyboardState[Keys.D]) - Convert.ToInt32(KeyboardState[Keys.A]);
        _moveInput.Y = Convert.ToInt32(KeyboardState[Keys.W]) - Convert.ToInt32(KeyboardState[Keys.S]);
        const float speed = 6;
           
        _world.Query(in _queryDescription, (ref VoxCamera camera, ref VoxPoint point) =>
        {
            point.Position += Vector3.Cross(camera.CameraForward, camera.CameraUp).Normalized() * _moveInput.X * speed * (float)args.Time;
            point.Position += camera.CameraForward * _moveInput.Y * speed * (float)args.Time;
            camera.Fov = Math.Clamp(camera.Fov - MouseState.Scroll.Y, 1, 45);
            camera.Projection = Matrix4.Identity *
                                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.Fov), 800.0f / 600.0f,
                                    0.1f, 100f);
            camera.View = Matrix4.Identity * Matrix4.LookAt(point.Position, point.Position + camera.CameraForward, camera.CameraUp); 
        });
        
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        const float sensitivity = 0.04f;
        float moveX = e.DeltaX * sensitivity;
        float moveY = e.DeltaY * sensitivity;
        
        _world.Query(in _queryDescription, (ref VoxPoint point) =>
        {
            Vector3 front;


            var dir = point.Rotation.Xyz;
            dir.Y += moveX;
            dir.X -= moveY;
        
            dir.X = Math.Clamp(dir.X, -89f, 89f);
            front.X = MathF.Cos(MathHelper.DegreesToRadians(dir.Y)) * MathF.Cos(MathHelper.DegreesToRadians(dir.X));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(dir.X));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(dir.Y)) * MathF.Cos(MathHelper.DegreesToRadians(dir.X));
            point.Rotation = front.Normalized().ToQuaternion();
        });
    }
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.ClearColor(Color.Aqua);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _texture1);
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _texture2);
        GL.BindVertexArray(_vao);
        _mat.Use();

        _world.Query(in _queryDescription, (ref VoxCamera camera) =>
        {
            _mat.SetMatrix4Uniform("proj", camera.Projection);
            _mat.SetMatrix4Uniform("view", camera.View);
        });

        
        for (int i = 0; i < _cubePositions.Length; i++)
        {
            Matrix4 model = Matrix4.Identity 
                    *Matrix4.CreateTranslation(_cubePositions[i]) 
                    * Matrix4.CreateScale(0.25f) 
                    * Matrix4.CreateFromAxisAngle(new Vector3(1, 0.3f, 0.5f), MathHelper.DegreesToRadians(22 * i))
                ;
            _mat.SetMatrix4Uniform("model", model);
            //GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length);
        }
        /* Matrix4 model = Matrix4.Identity * Matrix4.CreateTranslation(0, 0, 3);
        _mat.SetMatrix4Uniform("model", model, transpose);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length);
        _controller.Update(this, (float)e.Time);*/
        
        
        //_controller.Render();
        
        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        
        //_controller.WindowResized(e.Width, e.Height);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Keys.Escape)
        {
            this.Close();
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        //_controller.MouseScroll(e.Offset);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        //_controller.PressChar((char) e.Unicode);
    }

    private void DebugMessages(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userparam)
    {
        string msg = Marshal.PtrToStringAnsi(message, length);
        
        switch (severity)
        {
            case DebugSeverity.DontCare:
            case DebugSeverity.DebugSeverityNotification:
                VoxLogger.LogInfo("{0}, {1}", type, msg);
                break;
            case DebugSeverity.DebugSeverityHigh:
            case DebugSeverity.DebugSeverityMedium:
            case DebugSeverity.DebugSeverityLow:
                VoxLogger.LogError("{0}, {1}", type, msg);
                break;
        }
    }
}