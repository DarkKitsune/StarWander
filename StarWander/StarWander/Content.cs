﻿using System.Drawing;
using System.IO;

using StarWander.GFX;

namespace StarWander
{
    public static class Content
    {
        /// <summary>
        /// Root path of the content
        /// </summary>
        public static string Root { get; private set; } = Path.Combine("..", "..", "..", "content");

        /// <summary>
        /// Static constructor
        /// </summary>
        static Content()
        {
            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);
        }

        /// <summary>
        /// Load a shader
        /// </summary>
        /// <param name="path"></param>
        /// <param name="shaderType"></param>
        /// <param name="compile"></param>
        public static Shader LoadShader(string path, ShaderType shaderType, bool compile = true)
        {
            path = Path.Combine(Root, path);
            var shader = Shader.Create(Path.GetFileName(path), shaderType);
            shader.SetSourceFile(path);
            if (compile)
                shader.Compile();
            return shader;
        }

        public static ShaderProgram LoadShaderProgramVF(string vertexPath, string fragmentPath)
        {
            var shaderProgram = ShaderProgram.Create($"{nameof(ShaderProgram)}({Path.GetFileName(vertexPath)}+{Path.GetFileName(fragmentPath)})");
            using (var vert = LoadShader(vertexPath, ShaderType.Vertex))
            {
                using (var frag = LoadShader(fragmentPath, ShaderType.Fragment))
                {
                    shaderProgram.AttachShaders(vert, frag);
                    shaderProgram.Link();
                    shaderProgram.DetachShaders(vert, frag);
                }
            }
            if (shaderProgram.HasFeatures(Shader.ShaderFeatures.Camera))
                shaderProgram.EnableBlockCamera();
            if (shaderProgram.HasFeatures(Shader.ShaderFeatures.Fog))
                shaderProgram.EnableFog();
            if (shaderProgram.HasFeatures(Shader.ShaderFeatures.DiffuseTexture))
                shaderProgram.EnableDiffuseTexture();
            if (shaderProgram.HasFeatures(Shader.ShaderFeatures.NormalTexture))
                shaderProgram.EnableNormalTexture();
            return shaderProgram;
        }

        /// <summary>
        /// Load a texture
        /// </summary>
        /// <param name="path"></param>
        public static Texture2D LoadTexture2D(string path)
        {
            path = Path.Combine(Root, path);
            var texture = Texture2D.FromFile(Path.GetFileName(path), path);
            return texture;
        }

        /// <summary>
        /// Load a bitmap
        /// </summary>
        /// <param name="path"></param>
        public static Bitmap LoadBitmap(string path)
        {
            path = Path.Combine(Root, path);
            return (Bitmap)Image.FromFile(path);
        }
    }
}
