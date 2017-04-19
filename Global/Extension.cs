using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Global
{
    public static class Extension
    {
        public static byte[] SerializeToByteArray(this object objectData)
        {
            byte[] bytes;
            using (var _MemoryStream = new MemoryStream())
            {
                IFormatter _BinaryFormatter = new BinaryFormatter();
                _BinaryFormatter.Serialize(_MemoryStream, objectData);
                bytes = _MemoryStream.ToArray();
            }
            return bytes;
        }
        public static T DeserializeToDynamicType<T>(this byte[] byteArray)
        {
            using (var _MemoryStream = new MemoryStream(byteArray))
            {
                IFormatter _BinaryFormatter = new BinaryFormatter();
                var ReturnValue = _BinaryFormatter.Deserialize(_MemoryStream);
                return (T)ReturnValue;
            }
        }
    }
}
