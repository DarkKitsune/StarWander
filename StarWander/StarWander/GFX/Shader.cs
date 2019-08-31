using System;
using System.Text.RegularExpressions;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using VulpineLib.Util;

namespace StarWander.GFX
{
    /// <summary>
    /// A GL shader object
    /// </summary>
    public class Shader : IGLObject
    {
        /// <summary>
        /// Features allowed in a shader
        /// </summary>
        [Flags]
        public enum ShaderFeatures
        {
            None = 0,
            Camera = 1,
            Fog = 1 << 1,
            TextureSource = 1 << 2,
            ModelMatrix = 1 << 3,
            PositionScale = 1 << 4,
            ModelColor = 1 << 5,
            DiffuseTexture = 1 << 6,
            NormalTexture = 1 << 7,
        }

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
        /// The type of shader this is
        /// </summary>
        public ShaderType ShaderType { get; private set; }

        /// <summary>
        /// Get whether this shader has been compiled
        /// </summary>
        public bool IsCompiled
        {
            get
            {
                this.Assert();

                GL.GetShader(Handle.Handle, ShaderParameter.CompileStatus, out var compileStatus);
                return compileStatus != 0;
            }
        }

        /// <summary>
        /// Get the info log for this shader
        /// </summary>
        public string InfoLog
        {
            get
            {
                this.Assert();

                GL.GetShaderInfoLog(Handle.Handle, out var infoLog);
                return infoLog;
            }
        }

        public ShaderFeatures Features { get; private set; }

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private Shader()
        {
            Name = "";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct a shader
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="handle"></param>
        private Shader(string name, ShaderType type, int handle)
        {
            Name = name;
            ShaderType = type;
            if (State.VersionIsAtLeast(4, 3))
                GL.ObjectLabel(ObjectLabelIdentifier.Shader, handle, name.Length, name);
            Handle = new GLHandle(handle);
        }

        /// <summary>
        /// Construct a shader
        /// </summary>
        /// <param name="name">The shader name</param>
        /// <param name="type">The type of shader</param>
        /// <returns>The new shader</returns>
        public static Shader Create(string name, ShaderType type)
        {
            return new Shader(name, type, GL.CreateShader(type.ToGLShaderType()));
        }

        /// <summary>
        /// Called during finalization
        /// </summary>
        ~Shader()
        {
            Dispose();
        }

        /// <summary>
        /// Set the shader source
        /// </summary>
        /// <param name="source"></param>
        public void SetSource(string source)
        {
            this.Assert();

            source = PreProcess(source);
            GL.ShaderSource(Handle.Handle, source);
        }

        /// <summary>
        /// Set the shader sources
        /// </summary>
        /// <param name="sources"></param>
        public void SetSource(params string[] sources)
        {
            this.Assert();

            unsafe
            {
                var lengths = stackalloc int[sources.Length];
                for (var i = 0; i < sources.Length; i++)
                {
                    sources[i] = PreProcess(sources[i]);
                    lengths[i] = sources[i].Length;
                }
                GL.ShaderSource(Handle.Handle, sources.Length, sources, lengths);
            }
        }

        private static Regex PPRegexFeature = new Regex(@"#feature\((.*)\)", RegexOptions.Compiled);
        private string PreProcess(string source)
        {
            return PPRegexFeature.Replace(
                    source,
                    (match) =>
                    {
                        var feature = match.Groups[1].Value;
                        switch (feature)
                        {
                            case "Camera":
                                Features |= ShaderFeatures.Camera;
                                return
                                        // uniforms
                                        "layout(std140) uniform block_camera" +
                                        "{" +
                                            "mat4 uniform_projection;" +
                                            "mat4 uniform_view;" +
                                            "vec3 uniform_eyePosition;" +
                                        "};" +
                                        // functions
                                        "vec3 eye()" +
                                        "{" +
                                            "return uniform_eyePosition;" +
                                        "}" +
                                        "mat4 view()" +
                                        "{" +
                                            "return uniform_view;" +
                                        "}" +
                                        "mat4 view(mat4 x)" +
                                        "{" +
                                            "return uniform_view * x;" +
                                        "}" +
                                        "vec4 view(vec4 x)" +
                                        "{" +
                                            "return uniform_view * x;" +
                                        "}" +
                                        "mat4 projection()" +
                                        "{" +
                                            "return uniform_projection;" +
                                        "}" +
                                        "mat4 projection(mat4 x)" +
                                        "{" +
                                            "return uniform_projection * x;" +
                                        "}" +
                                        "vec4 projection(vec4 x)" +
                                        "{" +
                                            "return uniform_projection * x;" +
                                        "}";

                            case "Fog":
                                Features |= ShaderFeatures.Fog;
                                return
                                        // uniforms
                                        "layout(std140) uniform block_fog" +
                                        "{" +
                                            "float uniform_fogStart;" +
                                            "float uniform_fogEnd;" +
                                            "vec4 uniform_fogColor;" +
                                        "};" +
                                        // functions
                                        "float fog(float z)" +
                                        "{" +
                                            "return clamp((z - fogStart) / (fogEnd - fogStart), 0.0, 1.0)" +
                                        "}" +
                                        "vec4 mixFog(vec4 baseColor, float z)" +
                                        "{" +
                                            "return mix(baseColor, uniform_fogColor, fog(z));" +
                                        "}" +
                                        "vec3 mixFog(vec3 baseColor, float z)" +
                                        "{" +
                                            "return mix(vec4(baseColor, 1.0), uniform_fogColor, fog(z)).xyz;" +
                                        "}";

                            case "TextureSource":
                                Features |= ShaderFeatures.TextureSource;
                                return
                                        // uniforms
                                        "uniform vec2 uniform_textureSourceTopLeft;" +
                                        "uniform vec2 uniform_textureSourceSize;" +
                                        // functions
                                        "vec2 textureSourceCoords(vec2 texcoords)" +
                                        "{" +
                                            "return uniform_textureSourceTopLeft + texcoords * uniform_textureSourceSize;" +
                                        "}";

                            case "ModelMatrix":
                                Features |= ShaderFeatures.ModelMatrix;
                                return
                                        // uniforms
                                        "uniform mat4 uniform_modelMatrix;" +
                                        // functions
                                        "mat4 model()" +
                                        "{" +
                                            "return uniform_modelMatrix;" +
                                        "}" +
                                        "vec4 model(vec4 baseVec)" +
                                        "{" +
                                            "return uniform_modelMatrix * baseVec;" +
                                        "}" +
                                        "mat4 model(mat4 baseMat)" +
                                        "{" +
                                            "return uniform_modelMatrix * baseMat;" +
                                        "}";

                            case "PositionScale":
                                Features |= ShaderFeatures.PositionScale;
                                return
                                        // uniforms
                                        "uniform vec3 uniform_position;" +
                                        "uniform vec3 uniform_scale;" +
                                        // functions
                                        "vec3 position()" +
                                        "{" +
                                            "return uniform_position;" +
                                        "}" +
                                        "vec3 scale()" +
                                        "{" +
                                            "return uniform_scale;" +
                                        "}" +
                                        "vec3 scale(vec3 baseVec)" +
                                        "{" +
                                            "return uniform_scale * baseVec;" +
                                        "}" +
                                        "vec3 positionScale(vec3 baseVec)" +
                                        "{" +
                                            "return baseVec * uniform_scale + uniform_position;" +
                                        "}" +
                                        "vec4 positionScale(vec4 baseVec)" +
                                        "{" +
                                            "return vec4(baseVec.xyz * uniform_scale + uniform_position, baseVec.w);" +
                                        "}";

                            case "ModelColor":
                                Features |= ShaderFeatures.ModelColor;
                                return
                                        // uniforms
                                        "uniform vec4 uniform_modelColor;" +
                                        // functions
                                        "vec4 modelColor()" +
                                        "{" +
                                            "return uniform_modelColor;" +
                                        "}" +
                                        "vec4 modelColor(vec4 baseColor)" +
                                        "{" +
                                            "return uniform_modelColor * baseColor;" +
                                        "}";

                            case "DiffuseTexture":
                                Features |= ShaderFeatures.DiffuseTexture;
                                return
                                        // uniforms
                                        "uniform sampler2D uniform_diffuseTexture;" +
                                        // functions
                                        "vec4 diffuse(vec2 texcoords)" +
                                        "{" +
                                            "return texture(uniform_diffuseTexture, texcoords);" +
                                        "}";

                            case "NormalTexture":
                                Features |= ShaderFeatures.NormalTexture;
                                return
                                        // uniforms
                                        "uniform sampler2D uniform_normalTexture;" +
                                        // functions
                                        "vec3 normal(vec2 texcoords)" +
                                        "{" +
                                            "return texture(uniform_normalTexture, texcoords).xyz;" +
                                        "}";

                            default:
                                Log.Error(
                                        $"Unknown feature \"{match.Groups[0].Value}\" while preprocessing shader \"{Name}\"", "Shader Preprocesser"
                                    );
                                return match.Groups[0].Value;
                        }
                    }
                );
        }

        /// <summary>
        /// Set the shader source
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void SetSourceFile(string path)
        {
            this.Assert();

            if (!File.Exists(path))
                throw new FileNotFoundException($"No shader file found at path \"{path}\"");
            SetSource(File.ReadAllText(path));
        }

        /// <summary>
        /// Compile the shader source into this shader
        /// </summary>
        /// <exception cref="ShaderCompileException"></exception>
        public void Compile()
        {
            this.Assert();

            GL.CompileShader(Handle.Handle);
            var infoLog = InfoLog;
            if (InfoLog.Length > 0)
            {
                if (IsCompiled)
                {
                    Log.Info(infoLog, ToString());
                }
                else
                {
                    Log.Error(infoLog, ToString());
                    throw new ShaderCompileException(this, infoLog);
                }
            }
            else if (!IsCompiled)
            {
                throw new ShaderCompileException(this, "N/A");
            }
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
                return;

            Handle = GLHandle.Null;
            GL.DeleteShader(Handle.Handle);
            Disposed = true;
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
