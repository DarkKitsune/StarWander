using System;
using System.Collections.Generic;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class Asset : INameable, IDisposable, IEquatable<Asset>
    {
        private static long NextID;

        /// <summary>
        /// Is this asset disposed?
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Unique asset ID
        /// </summary>
        public long UniqueID { get; private set; }

        /// <summary>
        /// Is this asset valid?
        /// </summary>
        public bool IsValid => UniqueID > 0;
        
        /// <summary>
        /// Name of the asset
        /// </summary>
        public string Name { get; private set; }

        private Asset()
        {
            Name = "";
        }

        ~Asset()
        {
            Dispose();
        }

        /// <summary>
        /// Construct an asset
        /// </summary>
        /// <param name="name"></param>
        public Asset(string name)
        {
            Name = name;
            UniqueID = ++NextID;
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;
            OnDispose();
        }

        /// <summary>
        /// Called when the object is disposed
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Make sure this object is not disposed
        /// </summary>
        public void Assert()
        {
            if (Disposed)
                throw new ObjectDisposedException(Name);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Asset))
                return false;
            return Equals((Asset)obj);
        }

        public bool Equals(Asset other)
        {
            if (other is null!)
                return false;
            return UniqueID == other.UniqueID;
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public static bool operator ==(Asset left, Asset right)
        {
            if (left is null)
                return right is null;
            if (right is null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(Asset left, Asset right)
        {
            return !(left == right);
        }
    }
}
