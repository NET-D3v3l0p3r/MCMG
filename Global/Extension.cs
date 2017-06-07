using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;

namespace ShootCube.Global
{
    public static class Extension
    {
        public static byte[] SerializeToByteArray(this object objectData)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectData);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        public static T DeserializeToDynamicType<T>(this byte[] byteArray)
        {
            using (var memoryStream = new MemoryStream(byteArray))
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                var returnValue = binaryFormatter.Deserialize(memoryStream);
                return (T) returnValue;
            }
        }


        public static bool IsLess(this Vector3 value1, Vector3 value2)
        {
            return value1.X < value2.X && value1.Y < value2.Y && value1.Z < value2.Z;
        }

        public static bool IsGreater(this Vector3 value1, Vector3 value2)
        {
            return value1.X > value2.X && value1.Y > value2.Y && value1.Z > value2.Z;
        }


        public static bool IsLessOrEqual(this Vector3 value1, Vector3 value2)
        {
            return value1.X <= value2.X && value1.Y <= value2.Y && value1.Z <= value2.Z;
        }

        public static bool IsGreaterOrEqual(this Vector3 value1, Vector3 value2)
        {
            return value1.X >= value2.X && value1.Y >= value2.Y && value1.Z >= value2.Z;
        }
    }
}