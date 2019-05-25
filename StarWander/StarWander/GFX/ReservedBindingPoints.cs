namespace StarWander.GFX
{
    public static class ReservedBindingPoints
    {
        public static UniformBlockBindingPoint Camera { get; private set; }
        public static UniformBlockBindingPoint Fog { get; private set; }

        public static void Init()
        {
            Camera = UniformBlockBindingPoint.Create("ReservedBindingPoints.Camera");
            Fog = UniformBlockBindingPoint.Create("ReservedBindingPoints.Fog");
        }
    }
}
