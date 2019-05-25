using System;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class UniformBlockBindingPoint : IDisposable, INameable
    {
        public const int MaxBindingPoints = 24;

        /// <summary>
        /// Which binding point indices are already used/reserved
        /// </summary>
        private static bool[] ReservedIndices = new bool[MaxBindingPoints];

        /// <summary>
        /// Get the name of this object
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The index of this binding point
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private UniformBlockBindingPoint()
        {
            Name = "";
        }

        private UniformBlockBindingPoint(string name, int index)
        {
            Name = name;
            Index = index;
        }

        ~UniformBlockBindingPoint()
        {
            Dispose();
        }

        private static bool TryGetNext(out int index)
        {
            for (var i = 0; i < MaxBindingPoints; i++)
                if (!ReservedIndices[i])
                {
                    index = i;
                    return true;
                }
            index = -1;
            return false;
        }

        /// <summary>
        /// Reserve a binding point and return a new BindingPoint object pointing to it
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static UniformBlockBindingPoint Create(string name)
        {
            if (TryGetNext(out var index))
            {
                Log.Info($"Reserved binding point \"{name}\" with index {index}");
                ReservedIndices[index] = true;
                return new UniformBlockBindingPoint(name, index);
            }
            throw new InvalidOperationException("Cannot reserve any more binding point indices; the max have alread been reserved");
        }

        public void Dispose()
        {
            ReservedIndices[Index] = false;
        }
    }
}
