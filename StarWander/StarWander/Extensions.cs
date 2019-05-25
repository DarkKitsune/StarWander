using System;
using VulpineLib.Util;
using OpenTK;

namespace StarWander
{
    public static class Extensions
    {
        public static Matrix4 ToTKMatrix(this Matrix4<float> mat)
        {
            return new Matrix4(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                );
        }

        public static Matrix4 ToTKMatrix(this Matrix4<double> mat)
        {
            return new Matrix4(
                    (float)mat.M11, (float)mat.M12, (float)mat.M13, (float)mat.M14,
                    (float)mat.M21, (float)mat.M22, (float)mat.M23, (float)mat.M24,
                    (float)mat.M31, (float)mat.M32, (float)mat.M33, (float)mat.M34,
                    (float)mat.M41, (float)mat.M42, (float)mat.M43, (float)mat.M44
                );
        }

        public static Matrix4<float> ToVulpineMatrix(this Matrix4 mat)
        {
            return new Matrix4<float>(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                );
        }
    }
}
