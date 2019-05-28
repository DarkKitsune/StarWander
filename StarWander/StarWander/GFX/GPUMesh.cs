using System;
using System.IO;
using VulpineLib.Util;
using VulpineLib.Util.Generics;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    internal static class GPUMesh
    {
        /// <summary>
        /// The default mesh shader program
        /// </summary>
        public static ShaderProgram DefaultShaderProgram => ShaderProgramSets.Meshes["BasicLit"];
    }

    internal class GPUMesh<T> : INameable, IDisposable
        where T : struct, IVertex<Vector3<float>>
    {
        public bool Disposed { get; private set; }

        public string Name { get; set; }

        public ShaderProgram ShaderProgram { get; }
        public int IndexCount { get; }
        public VertexAttributes MeshVertexAttributes { get;}
        public PrimitiveType PrimitiveType { get; }
        public Buffer<T> VertexBuffer { get; }
        public Buffer<int> IndexBuffer { get; }

        public GPUMesh(Mesh<T> mesh, ShaderProgram? shaderProgram = null)
        {
            Console.WriteLine($"Creating GPUMesh from mesh \"{mesh.Name}\" with {mesh.Vertices.Length} " +
                $"vertices and {mesh.Indices.Length} indices");
            Name = mesh.Name + " (GPU)";
            ShaderProgram = shaderProgram ?? GPUMesh.DefaultShaderProgram;
            IndexCount = mesh.Indices.Length;
            MeshVertexAttributes = VertexAttributes.Create(Name + "'s VertexAttributes");
            VertexBuffer = Buffer<T>.Create(
                    Name + "'s Vertex Buffer",
                    mesh.Vertices.Length,
                    BufferStorageFlags.MapReadBit | BufferStorageFlags.MapWriteBit
                );
            IndexBuffer = Buffer<int>.Create(
                    Name + "'s Index Buffer",
                    mesh.Indices.Length,
                    BufferStorageFlags.MapReadBit | BufferStorageFlags.MapWriteBit
                );
            unsafe
            {
                // Copy vertices to vertex buffer
                var vdest = (void*)VertexBuffer.Map(BufferAccess.WriteOnly);
                var vsrc = mesh.Vertices.Map();
                State.CheckError();
                System.Buffer.MemoryCopy(vsrc, vdest, mesh.Vertices.Length * TypeInfo<T>.TypeSize, mesh.Vertices.Length * TypeInfo<T>.TypeSize);
                VertexBuffer.Unmap();
                mesh.Vertices.Unmap();
                State.CheckError();

                // Copy indices to index buffer
                var idest = (void*)IndexBuffer.Map(BufferAccess.WriteOnly);
                var isrc = mesh.Indices.Map();
                State.CheckError();
                System.Buffer.MemoryCopy(isrc, idest, mesh.Indices.Length * sizeof(int), mesh.Indices.Length * sizeof(int));
                IndexBuffer.Unmap();
                mesh.Indices.Unmap();
                State.CheckError();
            }
            switch (mesh.PrimitiveType)
            {
                case MeshPrimitiveType.LineStrip:
                    PrimitiveType = PrimitiveType.LineStrip;
                    break;
                case MeshPrimitiveType.LineLoop:
                    PrimitiveType = PrimitiveType.LineLoop;
                    break;
                case MeshPrimitiveType.Lines:
                    PrimitiveType = PrimitiveType.Lines;
                    break;
                case MeshPrimitiveType.Points:
                    PrimitiveType = PrimitiveType.Points;
                    break;
                case MeshPrimitiveType.TriangleFan:
                    PrimitiveType = PrimitiveType.TriangleFan;
                    break;
                case MeshPrimitiveType.Triangles:
                    PrimitiveType = PrimitiveType.Triangles;
                    break;
                case MeshPrimitiveType.TriangleStrip:
                    PrimitiveType = PrimitiveType.TriangleStrip;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid PrimitiveType in Mesh \"{mesh.Name}\"");
            }
            BuildVertexAttributes(MeshVertexAttributes, VertexBuffer);
        }

        ~GPUMesh()
        {
            Dispose();
        }

        public void Bind()
        {
            Assert();
            ShaderProgram.Bind();
            MeshVertexAttributes.Bind();
            IndexBuffer.Bind(BufferTarget.ElementArrayBuffer);
        }

        public void Draw()
        {
            Assert();
            GL.DrawElements(PrimitiveType, IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public static void Unbind()
        {
            VertexAttributes.Unbind();
            Buffer<int>.Unbind(BufferTarget.ElementArrayBuffer);
            ShaderProgram.Unbind();
        }

        private static void BuildVertexAttributes(VertexAttributes attr, Buffer<T> vertexBuffer)
        {
            var fields = typeof(T).GetFields(
                    System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance
                );
#if DEBUG
            Log.Info($"Generating attributes for vertex type {typeof(T)}", nameof(GPUMesh));
#endif
            var fullSize = 0;
            var sizes = new int[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var size = GetFieldSize(field.FieldType);
                sizes[i] = size;
                fullSize += size;
            }

            var offset = 0;
            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var size = sizes[i];
                var stride = fullSize;
                AttributeType attrType;
                if (field.FieldType == typeof(Vector3<float>))
                    attrType = AttributeType.Vector3;
                else if (field.FieldType == typeof(Vector3<double>))
                    attrType = AttributeType.Vector3D;
                else if (field.FieldType == typeof(Vector2<float>))
                    attrType = AttributeType.Vector2;
                else if (field.FieldType == typeof(Vector2<double>))
                    attrType = AttributeType.Vector2D;
                else if (field.FieldType == typeof(Color))
                    attrType = AttributeType.Vector4;
                else if (field.FieldType == typeof(float))
                    attrType = AttributeType.Float;
                else if (field.FieldType == typeof(double))
                    attrType = AttributeType.Double;
                else if (field.FieldType == typeof(int))
                    attrType = AttributeType.Int;
                else
                    throw new InvalidOperationException($"No case for {field.FieldType} typed field in {typeof(T)}");
                attr.SetVertexAttributeEnabled(i, true);
                attr.SetVertexAttributeLayout(i, field.Name, attrType, false, stride, offset, vertexBuffer);
#if DEBUG
                Log.Info($"Attribute {i}: {field.Name} Type={attrType} Stride={stride} Offset={offset}", nameof(GPUMesh));
#endif
                offset += size;
            }
        }

        private static int GetFieldSize(Type type)
        {
            if (type == typeof(Vector3<float>))
                return sizeof(float) * 3;
            if (type == typeof(Vector3<double>))
                return sizeof(double) * 3;
            if (type == typeof(Vector2<float>))
                return sizeof(float) * 2;
            if (type == typeof(Vector2<double>))
                return sizeof(double) * 2;
            if (type == typeof(Color))
                return sizeof(float) * 4;
            if (type == typeof(float))
                return sizeof(float);
            if (type == typeof(double))
                return sizeof(double);
            if (type == typeof(int))
                return sizeof(int);
            throw new InvalidOperationException($"No case for {type} typed field in {typeof(T)}");
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;

            MeshVertexAttributes.Dispose();
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }

        public void Assert()
        {
            if (Disposed)
                throw new ObjectDisposedException(Name);
        }
    }
}
