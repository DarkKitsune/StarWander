using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using VulpineLib.Util;

namespace StarWander.GFX
{
    /// <summary>
    /// A GL shader program object
    /// </summary>
    public class ShaderProgram : IGLObject
    {
        public static ShaderProgram None => throw new InvalidOperationException("");

        public bool Disposed { get; set; }

        /// <summary>
        /// The name of this shader
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The GLHandle pointing to this shader
        /// </summary>
        public GLHandle Handle { get; set; }

        /// <summary>
        /// Get whether this shader program has been linked
        /// </summary>
        public bool IsLinked
        {
            get
            {
                this.Assert();

                GL.GetProgram(Handle.Handle, GetProgramParameterName.LinkStatus, out var linkStatus);
                return linkStatus != 0;
            }
        }

        /// <summary>
        /// Get the info log for this shader program
        /// </summary>
        public string InfoLog
        {
            get
            {
                this.Assert();

                GL.GetProgramInfoLog(Handle.Handle, out var infoLog);
                return infoLog;
            }
        }

        // The shader features supported in this program
        public Shader.ShaderFeatures Features { get; private set; }

        public List<Shader> AttachedShaders { get; private set; } = new List<Shader>();

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private ShaderProgram()
        {
            Name = "";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct a new ShaderProgram
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        private ShaderProgram(string name, int handle)
        {
            Name = name;
            if (State.VersionIsAtLeast(4, 3))
                GL.ObjectLabel(ObjectLabelIdentifier.Program, handle, name.Length, name);
            Handle = new GLHandle(handle);
        }

        /// <summary>
        /// Construct a new ShaderProgram
        /// </summary>
        /// <param name="name">The shader program's name</param>
        /// <returns>The new shader program</returns>
        public static ShaderProgram Create(string name)
        {
            return new ShaderProgram(name, GL.CreateProgram());
        }

        /// <summary>
        /// Called during finalization
        /// </summary>
        ~ShaderProgram()
        {
            Dispose();
        }

        /// <summary>
        /// Attach a shader to this program
        /// </summary>
        /// <param name="shader"></param>
        public void AttachShader(Shader shader)
        {
            this.Assert();

            GL.AttachShader(Handle.Handle, shader.Handle.Handle);
            AttachedShaders.Add(shader);
        }

        /// <summary>
        /// Attach shaders to this program
        /// </summary>
        /// <param name="shaders"></param>
        public void AttachShaders(params Shader[] shaders)
        {
            this.Assert();

            foreach (var shader in shaders)
                AttachShader(shader);
        }

        /// <summary>
        /// Detach a shader from this program
        /// </summary>
        /// <param name="shader"></param>
        public void DetachShader(Shader shader)
        {
            this.Assert();

            GL.DetachShader(Handle.Handle, shader.Handle.Handle);
            AttachedShaders.Add(shader);
        }

        /// <summary>
        /// Detach shaders from this program
        /// </summary>
        /// <param name="shaders"></param>
        public void DetachShaders(params Shader[] shaders)
        {
            this.Assert();

            foreach (var shader in shaders)
                DetachShader(shader);
        }

        /// <summary>
        /// Link the shader program
        /// </summary>
        /// <exception cref="ShaderProgramLinkException"></exception>
        public void Link()
        {
            this.Assert();

            GL.LinkProgram(Handle.Handle);
            var infoLog = InfoLog;
            if (InfoLog.Length > 0)
            {
                if (IsLinked)
                {
                    Log.Info(infoLog, ToString());
                }
                else
                {
                    Log.Error(infoLog, ToString());
                    throw new ShaderProgramLinkException(this, infoLog);
                }
            }
            else if (!IsLinked)
            {
                throw new ShaderProgramLinkException(this, "N/A");
            }
            foreach (var shader in AttachedShaders)
                Features |= shader.Features;
        }

        /// <summary>
        /// Bind this shader program
        /// </summary>
        public void Bind()
        {
            GL.UseProgram(Handle.Handle);
        }

        /// <summary>
        /// Unbind all shader programs
        /// </summary>
        public static void Unbind()
        {
            GL.UseProgram(0);
        }

        /// <summary>
        /// Get a uniform location from the shader program
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <returns>Uniform location</returns>
        public int GetUniformLocation(string name)
        {
            Bind();
            return GL.GetUniformLocation(Handle.Handle, name);
        }

        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, float value)
        {
            Bind();
            GL.Uniform1(location, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, double value)
        {
            Bind();
            GL.Uniform1(location, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, int value)
        {
            Bind();
            GL.Uniform1(location, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, float[] value)
        {
            Bind();
            GL.Uniform1(location, value.Length, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, double[] value)
        {
            Bind();
            GL.Uniform1(location, value.Length, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, int[] value)
        {
            Bind();
            GL.Uniform1(location, value.Length, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, Vector2<double> value)
        {
            Bind();
            GL.Uniform2(location, value.X, value.Y);
        }/// <summary>
         /// Set data in the uniform at the location
         /// </summary>
         /// <param name="location">The uniform location</param>
         /// <param name="value"></param>
         /// <returns>Uniform location</returns>
        public void Uniform(int location, Vector2<float> value)
        {
            Bind();
            GL.Uniform2(location, value.X, value.Y);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, Vector2<int> value)
        {
            Bind();
            GL.Uniform2(location, value.X, value.Y);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, Vector3<double> value)
        {
            Bind();
            GL.Uniform3(location, value.X, value.Y, value.Z);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, Vector3<float> value)
        {
            Bind();
            GL.Uniform3(location, value.X, value.Y, value.Z);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, Vector3<int> value)
        {
            Bind();
            GL.Uniform3(location, value.X, value.Y, value.Z);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, Color value)
        {
            Bind();
            GL.Uniform4(location, value.R, value.G, value.B, value.A);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, ref OpenTK.Matrix3 value)
        {
            Bind();
            GL.UniformMatrix3(location, false, ref value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="location">The uniform location</param>
        /// <param name="value"></param>
        /// <returns>Uniform location</returns>
        public void Uniform(int location, ref Matrix4<float> value)
        {
            Bind();
            var tk = value.ToTKMatrix();
            GL.UniformMatrix4(location, false, ref tk);
        }

        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, double value)
        {
            Bind();
            GL.Uniform1(GetUniformLocation(name), value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, float value)
        {
            Bind();
            GL.Uniform1(GetUniformLocation(name), value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, int value)
        {
            Bind();
            GL.Uniform1(GetUniformLocation(name), value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, double[] value)
        {
            Bind();
            GL.Uniform1(GetUniformLocation(name), value.Length, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, float[] value)
        {
            Bind();
            GL.Uniform1(GetUniformLocation(name), value.Length, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, int[] value)
        {
            Bind();
            GL.Uniform1(GetUniformLocation(name), value.Length, value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, Vector2<double> value)
        {
            Bind();
            GL.Uniform2(GetUniformLocation(name), value.X, value.Y);
        }/// <summary>
         /// Set data in the uniform at the location
         /// </summary>
         /// <param name="name">The uniform name</param>
         /// <param name="value"></param>
        public void Uniform(string name, Vector2<float> value)
        {
            Bind();
            GL.Uniform2(GetUniformLocation(name), value.X, value.Y);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, Vector2<int> value)
        {
            Bind();
            GL.Uniform2(GetUniformLocation(name), value.X, value.Y);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, Vector3<double> value)
        {
            Bind();
            GL.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, Vector3<float> value)
        {
            Bind();
            GL.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, Vector3<int> value)
        {
            Bind();
            GL.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, Color value)
        {
            Bind();
            GL.Uniform4(GetUniformLocation(name), value.R, value.G, value.B, value.A);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, ref OpenTK.Matrix3 value)
        {
            Bind();
            GL.UniformMatrix3(GetUniformLocation(name), false, ref value);
        }
        /// <summary>
        /// Set data in the uniform at the location
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <param name="value"></param>
        public void Uniform(string name, ref Matrix4<float> value)
        {
            Bind();
            var tk = value.ToTKMatrix();
            GL.UniformMatrix4(GetUniformLocation(name), false, ref tk);
        }

        /// <summary>
        /// Get a uniform block index from the shader program
        /// </summary>
        /// <param name="name">The uniform block name</param>
        /// <returns>Uniform block index</returns>
        public int GetUniformBlockIndex(string name)
        {
            Bind();
            return GL.GetUniformBlockIndex(Handle.Handle, name);
        }

        /// <summary>
        /// Set the binding point for a uniform block in the shader program
        /// </summary>
        /// <param name="uniformBlockIndex">The uniform block index</param>
        /// <param name="bindingPoint">The binding point</param>
        public void SetUniformBlockBindingPoint(int uniformBlockIndex, UniformBlockBindingPoint bindingPoint)
        {
            Bind();
            GL.UniformBlockBinding(Handle.Handle, uniformBlockIndex, bindingPoint.Index);
        }

        /// <summary>
        /// Set the binding point for a uniform block in the shader program
        /// </summary>
        /// <param name="uniformBlockName">The uniform block name</param>
        /// <param name="bindingPoint"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetUniformBlockBindingPoint(string uniformBlockName, UniformBlockBindingPoint bindingPoint)
        {
            Bind();
            var index = GL.GetUniformBlockIndex(Handle.Handle, uniformBlockName);
            if (index < 0)
                throw new ArgumentException($"Uniform block with name \"{uniformBlockName}\" not defined in shader program");
            GL.UniformBlockBinding(Handle.Handle, index, bindingPoint.Index);
        }

        /// <summary>
        /// Enable uniform block block_camera
        /// </summary>
        public void EnableBlockCamera()
        {
            AssertHasFeatures(Shader.ShaderFeatures.Camera);
            SetUniformBlockBindingPoint("block_camera", ReservedBindingPoints.Camera);
        }

        /// <summary>
        /// Enable uniform block block_fog
        /// </summary>
        public void EnableFog()
        {
            AssertHasFeatures(Shader.ShaderFeatures.Fog);
            SetUniformBlockBindingPoint("block_fog", ReservedBindingPoints.Fog);
        }

        /// <summary>
        /// Set the texture source coordinates
        /// </summary>
        /// <param name="textureSourceTopLeft"></param>
        /// <param name="textureSourceSize"></param>
        public void SetTextureSource(Vector2<float> textureSourceTopLeft, Vector2<float> textureSourceSize)
        {
            AssertHasFeatures(Shader.ShaderFeatures.TextureSource);
            Uniform("uniform_textureSourceTopLeft", textureSourceTopLeft);
            Uniform("uniform_textureSourceSize", textureSourceSize);
        }

        /// <summary>
        /// Enable using a diffuse texture; usually not necessary
        /// </summary>
        public void EnableDiffuseTexture()
        {
            AssertHasFeatures(Shader.ShaderFeatures.DiffuseTexture);
            Uniform("uniform_diffuseTexture", (int)TextureUnits.Diffuse);
        }

        /// <summary>
        /// Enable using a normal texture
        /// </summary>
        public void EnableNormalTexture()
        {
            AssertHasFeatures(Shader.ShaderFeatures.NormalTexture);
            Uniform("uniform_normalTexture", (int)TextureUnits.Normal);
        }

        /// <summary>
        /// Set the model color
        /// </summary>
        /// <param name="color"></param>
        public void SetModelColor(Color color)
        {
            AssertHasFeatures(Shader.ShaderFeatures.ModelColor);
            Uniform("uniform_modelColor", color);
        }

        /// <summary>
        /// Set the model matrix
        /// </summary>
        /// <param name="mat"></param>
        public void SetModelMatrix(ref Matrix4<float> mat)
        {
            AssertHasFeatures(Shader.ShaderFeatures.ModelMatrix);
            Uniform("uniform_modelMatrix", ref mat);
        }

        /// <summary>
        /// Set the model position & scale
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public void SetPositionScale(Vector3<float> position, Vector3<float> scale)
        {
            if (HasFeatures(Shader.ShaderFeatures.PositionScale))
            {
                Uniform("uniform_modelPosition", position);
                Uniform("uniform_modelScale", scale);
                return;
            }
            if (HasFeatures(Shader.ShaderFeatures.ModelMatrix))
            {
                var mat = Matrix4<float>.CreateScale(scale) *
                    Matrix4<float>.CreateTranslation(position);
                Uniform("uniform_modelMatrix", ref mat);
                return;
            }
            throw new InvalidOperationException(
                    $"ShaderProgram \"{Name}\" must support one of the following features: " +
                    $"{Shader.ShaderFeatures.PositionScale} or {Shader.ShaderFeatures.ModelMatrix}"
                );
        }

        public bool HasFeatures(Shader.ShaderFeatures features)
        {
            return (Features & features) != 0;
        }

        public void AssertHasFeatures(Shader.ShaderFeatures features)
        {
            if ((Features & features) == 0)
                throw new InvalidOperationException($"ShaderProgram \"{Name}\" does not support the feature(s) {features}");
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
                return;

            Handle = GLHandle.Null;
            GL.DeleteProgram(Handle.Handle);
            Disposed = true;
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
