using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public static class State
    {
        /// <summary>
        /// Minimum allowed context version
        /// </summary>
        public static Version MinVersion = new Version(4, 0);

        /// <summary>
        /// Preferred context version
        /// </summary>
        public static Version PreferredVersion = new Version(4, 3);

        /// <summary>
        /// Get the OpenGL context version
        /// </summary>
        public static Version Version { get; private set; } = new Version(0, 0);

        /// <summary>
        /// Get if the OpenGL context version is at least this version
        /// </summary>
        /// <param name="major">The major version number</param>
        /// <param name="minor">The minor version number</param>
        /// <returns>Whether the OpenGL context version is at least this version</returns>
        public static bool VersionIsAtLeast(int major, int minor)
        {
            if (Version.Major >= major)
                return true;
            if (Version.Major == major && Version.Minor >= minor)
                return true;
            return false;
        }

        /// <summary>
        /// Get if the OpenGL context version is at least this version
        /// </summary>
        /// <param name="version">The version to compare against</param>
        /// <returns>Whether the OpenGL context version is at least this version</returns>
        public static bool VersionIsAtLeast(Version version)
        {
            return VersionIsAtLeast(version.Major, version.Minor);
        }

        /// <summary>
        /// Do initialization of the GL state
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static void Init()
        {
            // Get the context version
            Version = new Version(GL.GetInteger(GetPName.MajorVersion), GL.GetInteger(GetPName.MinorVersion));
            CheckError();
            Log.Info($"Context version: {Version}", "State");

            // Check that the context version is at least MinVersion
            if (!VersionIsAtLeast(MinVersion))
                throw new PlatformNotSupportedException(
                        $"Was not able create an OpenGL context with a version of at least {MinVersion}"
                    );

            //OpenTK.Graphics.GraphicsContext.CurrentContext.ErrorChecking = true;
#if DEBUG
            // Enable debug messages if the context supports it and if targetting Debug
            if (VersionIsAtLeast(4, 3))
                GL.DebugMessageCallback(DebugProc, IntPtr.Zero);
            CheckError();
#endif

            ReservedBindingPoints.Init();
            CheckError();

            GL.ClearColor(0.5f, 0.6f, 0.7f, 0.0f);
            CheckError();

            Camera.Init();
            CheckError();

            SpriteSets.Init();

            GPUMesh.Init();
        }

        private static DebugProc DebugProc = new DebugProc(DebugCallback);
        /// <summary>
        /// Default debug callback
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="severity"></param>
        /// <param name="length"></param>
        /// <param name="messagePtr"></param>
        /// <param name="userParam"></param>
        /// <exception cref="GLErrorException"></exception>
        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr messagePtr, IntPtr userParam)
        {
#if DEBUG
            switch (id)
            {
                // Ignore these message IDs
                case 131185:
                    return;
            }
            var message = Marshal.PtrToStringAnsi(messagePtr, length);
            if (type == DebugType.DebugTypeError)
            {
                Log.Error(
                        $"(Source: {source.GetSimpleName()}, Severity: {severity.GetSimpleName()}) {message}\n", "GL"
                    );
                throw new GLErrorException(message);
            }
            else if (
                    type == DebugType.DebugTypePerformance
                    || type == DebugType.DebugTypePortability
                    || type == DebugType.DebugTypeUndefinedBehavior
                )
            {
                Log.Warning(
                       $"(Source: {source.GetSimpleName()} Severity: {severity.GetSimpleName()}) {message}\n", "GL"
                   );
            }
            else
            {
                Log.Info(
                       $"(Source: {source.GetSimpleName()}) {message}\n", "GL"
                   );
            }
#endif
        }

        /// <summary>
        /// Get a simpler name for a DebugSource
        /// </summary>
        /// <param name="self"></param>
        private static string GetSimpleName(this DebugSource self)
        {
            switch (self)
            {
                case DebugSource.DebugSourceApi:
                    return "API";
                case DebugSource.DebugSourceApplication:
                    return "Application";
                case DebugSource.DebugSourceOther:
                    return "Other";
                case DebugSource.DebugSourceShaderCompiler:
                    return "Shader Compiler";
                case DebugSource.DebugSourceThirdParty:
                    return "Third-Party";
                case DebugSource.DebugSourceWindowSystem:
                    return "Window System";
                default:
                    return "N/A";
            }
        }

        /// <summary>
        /// Get a simpler name for a DebugSeverity
        /// </summary>
        /// <param name="self"></param>
        private static string GetSimpleName(this DebugSeverity self)
        {
            switch (self)
            {
                case DebugSeverity.DebugSeverityHigh:
                    return "High";
                case DebugSeverity.DebugSeverityMedium:
                    return "Medium";
                case DebugSeverity.DebugSeverityLow:
                    return "Low";
                case DebugSeverity.DebugSeverityNotification:
                    return "Notification";
                default:
                    return "N/A";
            }
        }

        /// <summary>
        /// Check for an error and report it if there is one
        /// </summary>
        /// <exception cref="GLErrorException"></exception>
        public static void CheckError()
        {
#if DEBUG
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Log.Error($"GL Error code: {error} ({(int)error})", "State");
                throw new GLErrorException($"Got error code: {error}({(int)error})");
            }
#endif
        }

        // === Camera ===
        

        // === Fog ===
        /// <summary>
        /// Struct for the "block_fog" uniform block
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct UniformBlockFog
        {
            [FieldOffset(0)]
            public float Start;                     // Alignment 4      size 4

            [FieldOffset(4)]
            public float End;                       // Alignment 4      size 4

            [FieldOffset(16)]
            public VulpineLib.Util.Color FogColor;  // Alignment 16     size 16
        }

        /// <summary>
        /// Fog uniform buffer
        /// </summary>
        private static Buffer<UniformBlockFog> FogUniformBuffer;

        /// <summary>
        /// Initialize the fog
        /// </summary>
        private static void InitFog()
        {
            FogUniformBuffer = Buffer<UniformBlockFog>.Create(
                    "Fog Uniform Buffer", 1, BufferStorageFlags.MapWriteBit | BufferStorageFlags.ClientStorageBit
                );
            FogUniformBuffer.BindAtUniformBlockBindingPoint(ReservedBindingPoints.Camera);
            SetFog(float.MaxValue, float.MaxValue, VulpineLib.Util.Color.White);
        }

        /// <summary>
        /// Set the fog
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        public static void SetFog(float start, float end, VulpineLib.Util.Color color)
        {
            var fog = new UniformBlockFog
            {
                Start = start,
                End = end,
                FogColor = color
            };
            unsafe
            {
                var ptr = (UniformBlockFog*)FogUniformBuffer.Map(BufferAccess.WriteOnly);
                ptr[0] = fog;
                FogUniformBuffer.Unmap();
            }
        }
    }
}
