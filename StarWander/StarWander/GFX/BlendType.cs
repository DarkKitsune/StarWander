using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    public struct BlendType
    {
        /// <summary>
        /// Alpha blend mode for premultiplied-alpha textures
        /// </summary>
        public static BlendType AlphaPremultiplied = new BlendType(
                BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha,
                BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha,
                BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd
            );
        /// <summary>
        /// Alpha blend mode for premultiplied-alpha textures
        /// </summary>
        public static BlendType Alpha = new BlendType(
                BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha,
                BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha,
                BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd
            );

        /// <summary>
        /// Source RGB factor
        /// </summary>
        public BlendingFactorSrc SrcColor { get; private set; }
        /// <summary>
        /// Destination RGB factor
        /// </summary>
        public BlendingFactorDest DestColor { get; private set; }
        /// <summary>
        /// Source alpha factor
        /// </summary>
        public BlendingFactorSrc SrcAlpha { get; private set; }
        /// <summary>
        /// Destination alpha factor
        /// </summary>
        public BlendingFactorDest DestAlpha { get; private set; }
        /// <summary>
        /// RGB equation
        /// </summary>
        public BlendEquationMode EquationColor { get; private set; }
        /// <summary>
        /// Alpha equation
        /// </summary>
        public BlendEquationMode EquationAlpha { get; private set; }

        /// <summary>
        /// Construct a new BlendType
        /// </summary>
        /// <param name="srcColor">Source RGB factor</param>
        /// <param name="destColor">Destination RGB factor</param>
        /// <param name="srcAlpha">Source alpha factor</param>
        /// <param name="destAlpha">Destination alpha factor</param>
        /// <param name="equationColor">RGB equation</param>
        /// <param name="equationAlpha">Alpha equation</param>
        public BlendType(
                BlendingFactorSrc srcColor, BlendingFactorDest destColor,
                BlendingFactorSrc srcAlpha, BlendingFactorDest destAlpha,
                BlendEquationMode equationColor = BlendEquationMode.FuncAdd,
                BlendEquationMode equationAlpha = BlendEquationMode.FuncAdd
            )
        {
            SrcColor = srcColor;
            DestColor = destColor;
            SrcAlpha = srcAlpha;
            DestAlpha = destAlpha;
            EquationColor = equationColor;
            EquationAlpha = equationAlpha;
        }

        /// <summary>
        /// Enable blending and use this BlendType
        /// </summary>
        public void Bind()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFuncSeparate(SrcColor, DestColor, SrcAlpha, DestAlpha);
            GL.BlendEquationSeparate(EquationColor, EquationAlpha);
        }

        /// <summary>
        /// Disable any BlendType
        /// </summary>
        public static void Unbind()
        {
            GL.Disable(EnableCap.Blend);
        }
    }
}
