using System;
using VulpineLib.Animation;

namespace StarWander.GFX
{
    public struct GraphicInstruction : IInstruction
    {
        public enum InstructionCode
        {
            None,
            SetSprite
        }

        /// <summary>
        /// Instruction execution time
        /// </summary>
        public double Time { get; private set; }

        /// <summary>
        /// Instruction code
        /// </summary>
        public InstructionCode Code;

        /// <summary>
        /// Instruction arguments
        /// </summary>
        public object?[] Args { get; private set; }

        /// <summary>
        /// Construct a GraphicInstruction
        /// </summary>
        /// <param name="time"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        private GraphicInstruction(double time, InstructionCode code, params object[] args)
        {
            Time = time;
            Code = code;
            Args = args;
        }

        /// <summary>
        /// Construct a GraphicInstruction
        /// </summary>
        public static GraphicInstruction SetSprite(double time, Sprite sprite)
        {
            return new GraphicInstruction(time, InstructionCode.SetSprite, sprite);
        }
    }
}
