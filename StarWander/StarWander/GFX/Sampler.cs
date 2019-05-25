using VulpineLib.Util;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    internal class Sampler : IGLObject
    {
        public bool Disposed { get; set; }

        /// <summary>
        /// The name of this sampler
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The GLHandle pointing to this sampler
        /// </summary>
        public GLHandle Handle { get; set; }

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private Sampler()
        {
            Name = "";
        }

        public TextureMinFilter MinFilter
        {
            get
            {
                GetParameter(SamplerParameterName.TextureMinFilter, out int value);
                return (TextureMinFilter)value;
            }
            set
            {
                SetParameter(SamplerParameterName.TextureMinFilter, (int)value);
            }
        }

        public TextureMagFilter MagFilter
        {
            get
            {
                GetParameter(SamplerParameterName.TextureMagFilter, out int value);
                return (TextureMagFilter)value;
            }
            set
            {
                SetParameter(SamplerParameterName.TextureMagFilter, (int)value);
            }
        }

        public TextureWrapMode WrapS
        {
            get
            {
                GetParameter(SamplerParameterName.TextureWrapS, out int value);
                return (TextureWrapMode)value;
            }
            set
            {
                SetParameter(SamplerParameterName.TextureWrapS, (int)value);
            }
        }

        public TextureWrapMode WrapT
        {
            get
            {
                GetParameter(SamplerParameterName.TextureWrapT, out int value);
                return (TextureWrapMode)value;
            }
            set
            {
                SetParameter(SamplerParameterName.TextureWrapT, (int)value);
            }
        }

        /// <summary>
        /// Construct a Sampler object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        private Sampler(string name, int handle)
        {
            Name = name;
            GL.BindSampler(0, handle);
            GL.ObjectLabel(ObjectLabelIdentifier.Sampler, handle, name.Length, name);
            Handle = new GLHandle(handle);
        }

        /// <summary>
        /// Called when finalizing
        /// </summary>
        ~Sampler()
        {
            Dispose();
        }

        /// <summary>
        /// Construct a Sampler object
        /// </summary>
        /// <param name="name"></param>
        public static Sampler Create(string name)
        {
            return new Sampler(name, GL.GenSampler());
        }

        /// <summary>
        /// Bind this object to a texture unit in the state
        /// </summary>
        /// <param name="textureUnit"></param>
        public void Bind(int textureUnit = 0)
        {
            this.Assert();

            GL.BindSampler(textureUnit, Handle.Handle);
        }

        /// <summary>
        /// Bind this object to the diffuse texture unit
        /// </summary>
        public void BindDiffuse()
        {
            this.Assert();

            GL.BindSampler((int)TextureUnits.Diffuse, Handle.Handle);
        }

        /// <summary>
        /// Bind this object to the normal texture unit
        /// </summary>
        public void BindNormal()
        {
            this.Assert();

            GL.BindSampler((int)TextureUnits.Normal, Handle.Handle);
        }

        /// <summary>
        /// Unbind any Sampler from a texture unit
        /// </summary>
        /// <param name="textureUnit"></param>
        public static void Unbind(int textureUnit = 0)
        {
            GL.BindSampler(textureUnit, 0);
        }

        /// <summary>
        /// Unbind any Sampler from the diffuse texture unit
        /// </summary>
        public static void UnbindDiffuse()
        {
            GL.BindSampler((int)TextureUnits.Diffuse, 0);
        }

        /// <summary>
        /// Unbind any Sampler from the normal texture unit
        /// </summary>
        public static void UnbindNormal()
        {
            GL.BindSampler((int)TextureUnits.Normal, 0);
        }

        /// <summary>
        /// Set a sampler parameter
        /// </summary>
        /// <param name="param">The parameter</param>
        /// <param name="value">The value</param>
        public void SetParameter(SamplerParameterName param, int value)
        {
            GL.SamplerParameter(Handle.Handle, param, value);
        }

        /// <summary>
        /// Set a sampler parameter
        /// </summary>
        /// <param name="param">The parameter</param>
        /// <param name="value">The value</param>
        public void SetParameter(SamplerParameterName param, float value)
        {
            GL.SamplerParameter(Handle.Handle, param, value);
        }

        /// <summary>
        /// Get a sampler parameter
        /// </summary>
        /// <param name="param">The parameter</param>
        /// <param name="value">The value</param>
        public void GetParameter(SamplerParameterName param, out int value)
        {
            GL.GetSamplerParameter(Handle.Handle, param, out value);
        }

        /// <summary>
        /// Get a sampler parameter
        /// </summary>
        /// <param name="param">The parameter</param>
        /// <param name="value">The value</param>
        public void GetParameter(SamplerParameterName param, out float value)
        {
            GL.GetSamplerParameter(Handle.Handle, param, out value);
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
                return;

            Handle = GLHandle.Null;
            GL.DeleteSampler(Handle.Handle);
            Disposed = true;
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
