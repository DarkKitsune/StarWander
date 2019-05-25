namespace StarWander.GFX
{
    public struct GLHandle
    {
        public static GLHandle Null => new GLHandle(0);

        public int Handle { get; private set; }

        public static explicit operator GLHandle(int intHandle)
        {
            return new GLHandle { Handle = intHandle };
        }

        public static explicit operator int(GLHandle handle)
        {
            return handle.Handle;
        }

        public GLHandle(int handle)
        {
            Handle = handle;
        }
    }
}
