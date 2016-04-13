using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace wuxingogo.Tools
{
    public interface IWriteField
    {
        void Write(Stream s);
    }
    public class StreamUtils
    {
		static Encoding DefaultEncoding = Encoding.UTF8;
        private int SwapInt32(int int32)
        {
            return (int32 & 0xFF) << 24 | (int32 >> 8 & 0xFF) << 16 |
                (int32 >> 16 & 0xFF) << 8 | (int32 >> 24 & 0xFF);
        }

        static byte[] ReadBytes(Stream s, int count)
        {
            byte[] buffer = new byte[count];
            int l = s.Read(buffer, 0, count);
            if (l != count)
                UnityEngine.Debug.LogError("StreamUtils.ReadBytes failed...");
            return buffer;
        }
        public static String Read(Stream s, out String value, params int[] count)
        {
            Byte[] strBuf = null;
            if (null != count && count.Length > 0 && count[0] > 0)
            {
                strBuf = new Byte[count[0]];
                s.Read(strBuf, 0, count[0]);
            }
            else
            {
                strBuf = new Byte[sizeof(ushort)];
                s.Read(strBuf, 0, strBuf.Length);
               	Array.Reverse(strBuf);
                ushort strLength = BitConverter.ToUInt16(strBuf, 0);
                strBuf = new Byte[strLength];
                s.Read(strBuf, 0, strBuf.Length);
            }
			value = DefaultEncoding.GetString(strBuf, 0, strBuf.Length);
            return value;
        }
        public static Byte[] Read(Stream s, Byte[] value, params int[] count)
        {
            Byte[] buf = null;
            if (null != count && count.Length > 0 && count[0] > 0)
            {
                buf = new Byte[count[0]];
                s.Read(buf, 0, count[0]);
            }
            else
            {
                buf = new Byte[sizeof(ushort)];
                s.Read(buf, 0, buf.Length);
                Array.Reverse(buf);
                ushort strLength = BitConverter.ToUInt16(buf, 0);
                buf = new Byte[strLength];
                s.Read(buf, 0, buf.Length);
            }
            value = buf;
            return value;
        }
        public static Byte Read(Stream s, Byte value)
        {
            Byte[] buf = new Byte[1];
            s.Read(buf, 0, 1);
            value = buf[0];
            return value;
        }
        public static T Read<T>(Stream s, out T value)// where T : IConvertible
        {
            value = default(T);
            int len = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            byte[] buffer = ReadBytes(s, len);
            Array.Reverse(buffer);
            if (value is Int64)
                value = (T)(Object)BitConverter.ToInt64(buffer, 0);
            else if (value is UInt64)
                value = (T)(Object)BitConverter.ToUInt64(buffer, 0);
            else if (value is Int32)
                value = (T)(Object)BitConverter.ToInt32(buffer, 0);
            else if (value is UInt32)
                value = (T)(Object)BitConverter.ToUInt32(buffer, 0);
            else if (value is Int16)
                value = (T)(Object)BitConverter.ToInt16(buffer, 0);
            else if (value is UInt16)
                value = (T)(Object)BitConverter.ToUInt16(buffer, 0);
            else if (value is Single)
                value = (T)(Object)BitConverter.ToSingle(buffer, 0);
            else if (value is Double)
                value = (T)(Object)BitConverter.ToDouble(buffer, 0);
            else if (value is Char)
                value = (T)(Object)BitConverter.ToChar(buffer, 0);
            else if (value is Boolean)
                value = (T)(Object)BitConverter.ToBoolean(buffer, 0);
            else if (value is Byte)
                value = (T)(Object)(buffer[0]);
            else if (value is SByte)
                value = (T)(Object)(buffer[0]);
            else
                UnityEngine.Debug.LogError(string.Format("StreamUtils.Read<Number> failed, is {0} Type...", value.ToString()));
            return value;
        }
        public static Stream Write(Stream s, String value, params int[] count)
        {
            byte[] length = null;
            byte[] buffer = null;
            if (null != count && count.Length > 0 && count[0] > 0)
            {
                length = BitConverter.GetBytes((ushort)count[0]);
                buffer = DefaultEncoding.GetBytes(value.ToCharArray(), 0, count[0]);
            }
            else
            {
				buffer = DefaultEncoding.GetBytes(value);
                length = BitConverter.GetBytes((ushort)buffer.Length);
            }
            Array.Reverse(length);
            s.Write(length, 0, length.Length);
            s.Write(buffer, 0, buffer.Length);
            return s;
        }
        public static Stream Write(Stream s, Byte[] value, params int[] count)
        {
            byte[] length = null;
            byte[] buffer = null;
            if (null != count && count.Length > 0 && count[0] > 0)
            {
                length = BitConverter.GetBytes((ushort)count[0]);
                buffer = new byte[count[0]];
                Array.Copy(value, buffer, count[0]);
            }
            else
            {
                buffer = new byte[value.Length];
                Array.Copy(value, buffer, buffer.Length);
                length = BitConverter.GetBytes((ushort)buffer.Length);
            }
            Array.Reverse(length);
            s.Write(length, 0, length.Length);
            s.Write(buffer, 0, buffer.Length);
            return s;
        }
        public static Stream Write<LenT, ValueT>(Stream s, ValueT[] values)
        //    where LenT : IConvertible
            where ValueT : class, IWriteField
        {
            if (null == values || values.Length <= 0)
            {
                LenT zero = default(LenT);
                Write(s, zero);
                return s;
            }
            Write(s, (LenT)(Object)values.Length);
            for (int i = 0; i < values.Length; ++i)
            {
                ValueT v = values[i] as ValueT;
                v.Write(s);
            }
            return s;
        }
        public static Stream Write<T>(Stream s, T value)// where T : IConvertible
        {
            byte[] bin = null;
            if (value is Int64)
                bin = BitConverter.GetBytes((Int64)(Object)value);
            else if (value is UInt64)
                bin = BitConverter.GetBytes((UInt64)(Object)value);
            else if (value is Int32)
                bin = BitConverter.GetBytes((Int32)(Object)value);
            else if (value is UInt32)
                bin = BitConverter.GetBytes((UInt32)(Object)value);
            else if (value is Int16)
                bin = BitConverter.GetBytes((Int16)(Object)value);
            else if (value is UInt16)
                bin = BitConverter.GetBytes((UInt16)(Object)value);
            else if (value is Single)
                bin = BitConverter.GetBytes((Single)(Object)value);
            else if (value is Double)
                bin = BitConverter.GetBytes((Double)(Object)value);
            else if (value is Char)
                bin = BitConverter.GetBytes((Char)(Object)value);
            else if (value is Boolean)
                bin = BitConverter.GetBytes((Boolean)(Object)value);
            else if (value is Byte)
            {
                bin = new byte[1];
                bin[0] = (Byte)(Object)value;
            }
            else if (value is SByte)
            {
                bin = new byte[1];
                bin[0] = (byte)(Object)value;
            }
            else
                UnityEngine.Debug.LogError(string.Format("StreamUtils.Write<Number> failed, is {0} Type...", value.ToString()));
            Array.Reverse(bin);
            s.Write(bin, 0, bin.Length);
            return s;
        }

        public static T BitConvert<T>(byte[] b, out T value)// where T : IConvertible
        {
            value = default(T);
            int len = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            byte[] bin = new byte[len];
            for (int i = 0; i < len; ++i)
                bin[i] = b[i];
            Array.Reverse(bin);
            if (value is Int64)
                value = (T)(Object)BitConverter.ToInt64(bin, 0);
            else if (value is UInt64)
                value = (T)(Object)BitConverter.ToUInt64(bin, 0);
            else if (value is Int32)
                value = (T)(Object)BitConverter.ToInt32(bin, 0);
            else if (value is UInt32)
                value = (T)(Object)BitConverter.ToUInt32(bin, 0);
            else if (value is Int16)
                value = (T)(Object)BitConverter.ToInt16(bin, 0);
            else if (value is UInt16)
                value = (T)(Object)BitConverter.ToUInt16(bin, 0);
            else if (value is Single)
                value = (T)(Object)BitConverter.ToSingle(bin, 0);
            else if (value is Double)
                value = (T)(Object)BitConverter.ToDouble(bin, 0);
            else if (value is Char)
                value = (T)(Object)BitConverter.ToChar(bin, 0);
            else if (value is Boolean)
                value = (T)(Object)BitConverter.ToBoolean(bin, 0);
            else if (value is Byte)
                value = (T)(Object)(bin[0]);
            else if (value is SByte)
                value = (T)(Object)(bin[0]);
            else
                UnityEngine.Debug.LogError(string.Format("StreamUtils.Read<Number> failed, is {0} Type...", value.ToString()));
            return value;
        }
        
    }
}
