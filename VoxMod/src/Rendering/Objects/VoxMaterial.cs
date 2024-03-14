using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxMod.Main;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace VoxMod.Rendering.Objects;

public class VoxMaterial
{
    private readonly int _program;
    private readonly Dictionary<string, int> _uniformLocations;
    
    public VoxMaterial(Dictionary<ShaderType, string> sources)
    {
        _program = GL.CreateProgram();
        List<int> shaders = new List<int>();
        _uniformLocations = new Dictionary<string, int>();
        
        foreach (var source in sources)
        {
            var shader = CompileShader(source);
            shaders.Add(shader);
        
            GL.AttachShader(_program, shader);
        }
        
        GL.LinkProgram(_program);
        GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_program);
            Console.WriteLine(infoLog);
            throw new Exception("Failed to link program!");
        }


        foreach (var shader in shaders)
        {
            GL.DetachShader(_program, shader);
            GL.DeleteShader(shader);
        }
    }

    public void Use()
    {
        GL.UseProgram(_program);
    }
    
    private int CompileShader(KeyValuePair<ShaderType, string> shaderSource)
    {
        
        int shaderID = GL.CreateShader(shaderSource.Key);
        
        GL.ShaderSource(shaderID, shaderSource.Value);
        
        GL.CompileShader(shaderID);
        
        GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            VoxLogger.LogError("Error compiling shader of type {0}: {1}", shaderSource.Key, GL.GetShaderInfoLog(shaderID));
            throw new Exception($"Failed to compile shader of type {shaderSource.Key}");
        }
        

        return shaderID;
    }

    public void SetIntUniform(string name, int value)
    {
        if (_uniformLocations.TryGetValue(name, out var location))
            GL.Uniform1(location, value);
        else
        {
            var newLocation = GL.GetUniformLocation(_program, name);
            _uniformLocations.Add(name, newLocation);
            GL.Uniform1(newLocation, value);
        }
    }
    
    public void SetFloatUniform(string name, float value)
    {
        if (_uniformLocations.TryGetValue(name, out var location))
            GL.Uniform1(location, value);
        else
        {
            var newLocation = GL.GetUniformLocation(_program, name);
            _uniformLocations.Add(name, newLocation);
            GL.Uniform1(newLocation, value);
        }
    }

    public void SetVector2Uniform(string name, Vector2 value)
    {
        if (_uniformLocations.TryGetValue(name, out var location))
            GL.Uniform2(location, value.X, value.Y);
        else
        {
            var newLocation = GL.GetUniformLocation(_program, name);
            _uniformLocations.Add(name, newLocation);
            GL.Uniform2(newLocation, value.X, value.Y);
        }
    }

    public void SetVector3Uniform(string name, Vector3 value)
    {
        if (_uniformLocations.TryGetValue(name, out var location))
            GL.Uniform3(location, value.X, value.Y, value.Z);
        else
        {
            var newLocation = GL.GetUniformLocation(_program, name);
            _uniformLocations.Add(name, newLocation);
            GL.Uniform3(newLocation, value.X, value.Y, value.Z);
        }
    }

    public void SetVector4Uniform(string name, Vector4 value)
    {
        if (_uniformLocations.TryGetValue(name, out var location))
            GL.Uniform4(location, value.X, value.Y, value.Z, value.W);
        else
        {
            var newLocation = GL.GetUniformLocation(_program, name);
            _uniformLocations.Add(name, newLocation);
            GL.Uniform4(newLocation, value.X, value.Y, value.Z, value.W);
        }
    }

    public void SetMatrix4Uniform(string name, Matrix4 value, bool transpose = true)
    {
        if (_uniformLocations.TryGetValue(name, out var location))
            GL.UniformMatrix4(location, transpose, ref value);
        else
        {
            var newLocation = GL.GetUniformLocation(_program, name);
            _uniformLocations.Add(name, newLocation);
            GL.UniformMatrix4(newLocation, transpose, ref value);
        }
    }
}