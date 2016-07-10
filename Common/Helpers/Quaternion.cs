using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Helpers
{
    public class Quaternion
    {
        public Single X;
        public Single Y;
        public Single Z;
        public float W;

        public Quaternion()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 0;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}
