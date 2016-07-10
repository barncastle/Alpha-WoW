using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Helpers
{
    public class Vector
    {
        public Single X;
        public Single Y;
        public Single Z;

        public Vector()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
        }

        public Vector(Single x, Single y, Single z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Single Distance(Vector v)
        {
            return (Single)Math.Sqrt(DistanceSqrd(v));
        }

        public double DistanceSqrd(Vector v)
        {
            float dX = X - v.X;
            float dY = Y - v.Y;
            float dZ = Z - v.Z;
            return (dX * dX + dY * dY + dZ * dZ);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public Single Angle(Vector v)
        {
            return (Single)Math.Atan2(v.X - X, v.Y - Y);
        }

        public Single Angle(float x, float y)
        {
            return (Single)Math.Atan2(x - X, y - Y);
        }
    }
}
