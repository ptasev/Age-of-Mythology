namespace AoMEngineLibrary.Graphics
{
    using MiscUtil;

    public struct Vector3<T>
    {
        public T X;
        public T Y;
        public T Z;

        public Vector3(T value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
        }
        public Vector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public T LongestAxisLength()
        {
            if (Operator.LessThan<T>(X, Y))
            {
                if (Operator.LessThan<T>(Y, Z))
                {
                    return Z;
                }
                else
                {
                    return Y;
                }
            }
            else
            {
                if (Operator.LessThan<T>(X, Z))
                {
                    return Z;
                }
                else
                {
                    return X;
                }
            }
        }

        public static Vector3<T> operator -(Vector3<T> one)
        {
            return new Vector3<T>(Operator.Negate<T>(one.X), Operator.Negate<T>(one.Y), Operator.Negate<T>(one.Z));
        }
        public static Vector3<T> operator -(Vector3<T> one, Vector3<T> two)
        {
            return new Vector3<T>(Operator.Subtract<T>(one.X, two.X), Operator.Subtract<T>(one.Y, two.Y), Operator.Subtract<T>(one.Z, two.Z));
        }
        public static Vector3<T> operator +(Vector3<T> one, Vector3<T> two)
        {
            return new Vector3<T>(Operator.Add<T>(one.X, two.X), Operator.Add<T>(one.Y, two.Y), Operator.Add<T>(one.Z, two.Z));
        }
        public static Vector3<T> operator /(Vector3<T> one, T two)
        {
            return new Vector3<T>(Operator.Divide<T>(one.X, two), Operator.Divide<T>(one.Y, two), Operator.Divide<T>(one.Z, two));
        }

        public static Vector3<T> CrossProduct(Vector3<T> v1, Vector3<T> v2)
        {
            return
            (
               new Vector3<T>
               (
                  Operator.Subtract<T>(Operator.Multiply<T>(v1.Y, v2.Z), Operator.Multiply<T>(v1.Z, v2.Y)),
                  Operator.Subtract<T>(Operator.Multiply<T>(v1.Z, v2.X), Operator.Multiply<T>(v1.X, v2.Z)),
                  Operator.Subtract<T>(Operator.Multiply<T>(v1.X, v2.Y), Operator.Multiply<T>(v1.Y, v2.X))
               )
            );
        }
        public static T DotProduct(Vector3<T> v1, Vector3<T> v2)
        {
            return
            (
                Operator.Add<T>(Operator.Add<T>(Operator.Multiply<T>(v1.X, v2.X), Operator.Multiply<T>(v1.Y, v2.Y)), Operator.Multiply<T>(v1.Z, v2.Z))
            );
        }
        public static Vector3<T> CalcNormalOfFace(Vector3<T>[] pPositions, Vector3<T>[] pNormals)
        {
            Vector3<T> p0 = pPositions[1] - pPositions[0];
            Vector3<T> p1 = pPositions[2] - pPositions[0];
            Vector3<T> faceNormal = Vector3<T>.CrossProduct(p0, p1);

            Vector3<T> vertexNormal = pNormals[0];
            float dot = Operator.Convert<T, float>(Vector3<T>.DotProduct(faceNormal, vertexNormal));

            return (dot < 0.0f) ? -faceNormal : faceNormal;
        }
    }
}
