using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


namespace Lib.Framework
{
    public static class Extension
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlCopyMemory")]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void MoveMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOMOVE = 0x2;
        private const uint SWP_NOSIZE = 0x1;

        public static void FocusAppWithName(string appName)
        {
            IntPtr hWnd = FindWindow(null, appName);
            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="topOrbottom"></param>
        public static void FocusAppWithName(string appName, int topOrbottom)
        {
            IntPtr hWnd = FindWindow(null, appName);
            if (hWnd != IntPtr.Zero)
            {
                SetWindowPos(hWnd, new IntPtr(topOrbottom), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        public static bool IsNullOrEmpyOrWhiteSpaces(this string _string)
        {
            return string.IsNullOrEmpty(_string) || string.IsNullOrEmpty(_string.Trim());
        }

        public static string GetShortVersion(this string versionNum)
        {
            string[] versionNumArray = versionNum.Split('.');
            if (versionNum.Length > 1)
                return versionNumArray[0] + "." + versionNumArray[1];
            return versionNum;
        }

        public static string ToVersionNum(this Version version)
        {
            return $"{version.Major:00}.{version.Minor:00}.{version.Build:00}.{version.Revision:00}";
        }

        public static uint ByteToInt(this byte[] bytes)
        {
            return ByteToInt(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        private static uint ByteToInt(byte a, byte b, byte c, byte d)
        {
            uint la = a;
            uint lb = ((uint)b) * 256;
            uint lc = ((uint)c) * 256 * 256;
            uint ld = ((uint)d) * 256 * 256 * 256;
            return la + lb + lc + ld;
        }

        public static byte[] StructToBytes<T>(this T structObj) where T : struct
        {
            return StructToBytes(structObj, Marshal.SizeOf(typeof(T)));
        }

        public static byte[] StructToBytes<T>(this T structObj, int size) where T : struct
        {
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, true);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }

        public static byte[] ClassToBytes<T>(this T structObj, int size) where T : class
        {
            byte[] bytes = new byte[size];
            Marshal.StructureToPtr(structObj, bytes.BytesToIntptr(), true);
            return bytes;
        }

        public static T BytesToStruct<T>(this byte[] bytes) where T : struct
        {
            return BytesToStruct<T>(bytes, 0);
        }

        public static T BytesToStruct<T>(this byte[] bytes, int offset) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            if (size > bytes.Length) return default;
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, offset, structPtr, size);
            T obj = Marshal.PtrToStructure<T>(structPtr);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        public static T BytesToStruct<T>(this byte[] bytes, int offset, out int size) where T : struct
        {
            size = Marshal.SizeOf(typeof(T));
            return BytesToStruct<T>(bytes, offset);
        }

        public static T BytesToStructP<T>(this byte[] bytes) where T : struct
        {
            return BytesToStructP<T>(bytes, 0);
        }

        public static T BytesToStructP<T>(this byte[] bytes, int offset, out int size) where T : struct
        {
            size = Marshal.SizeOf(typeof(T));
            return BytesToStructP<T>(bytes, offset);
        }

        /// <summary>
        /// Marshals data from an unmanaged block of memory to a newly allocated managed object of the type specified by a generic type parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T BytesToStructP<T>(this byte[] bytes, int offset) where T : struct
        {
            //byte pointOffset = bytes.AsSpan()[offset..].GetPinnableReference();
            //IntPtr ptr = (IntPtr)(&pointOffset);
            return Marshal.PtrToStructure<T>(bytes.BytesToIntptr(offset));
        }

        /// <summary>
        /// Marshals data from an unmanaged block of memory to a managed object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="obj"></param>
        public static void BytesToStructP<T>(this byte[] bytes, int offset, T obj) where T : class
        {
            //byte pointOffset = bytes.AsSpan()[offset..].GetPinnableReference();
            //IntPtr ptr = (IntPtr)(&pointOffset);
            Marshal.PtrToStructure(bytes.BytesToIntptr(offset), obj);
        }

        public static byte[] ToCopy(this byte[] bytes, int offset, int length)
        {
            byte[] temp = new byte[length];
            Buffer.BlockCopy(bytes, offset, temp, 0, length);
            return temp;
        }

        public static string BytesToString(this byte[] bytes, int offset, int length, Encoding encoding)
        {
            return encoding.GetString(bytes, offset, length).TrimEnd('\0');
        }

        public static string BytesToString(this byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes).TrimEnd('\0');
        }

        public static string CharsToString(this char[] chars)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(chars);
            return sb.ToString().TrimEnd('\0');
        }

        public static void StringToBytes(this string path, byte[] bytes, Encoding encoding)
        {
#if NET8_0 || NET7_0 || NET6_0 || NET6_0_OR_GREATER || NETSTANDARD2_1 || NET5_0 || NET5_0_OR_GREATER
            encoding.GetBytes(path.AsSpan(), bytes.AsSpan());
#else
            encoding.GetBytes(path, 0, path.Length, bytes, 0);
#endif
        }
        public static byte[] StringToBytes(this string path, Encoding encoding)
        {
            return encoding.GetBytes(path);
        }

        public static char[] BytesToChars(this byte[] bytes, Encoding encoding)
        {
            return encoding.GetChars(bytes);
        }

        public static byte[] CharsToBytes(this char[] path, Encoding encoding)
        {
            return encoding.GetBytes(path);
        }

        public static byte[] IntptrToBytes(this IntPtr obj, int size)
        {
            byte[] buffer = new byte[size];
            Marshal.Copy(obj, buffer, 0, size);
            return buffer;
        }

        public static IntPtr IntptrToIntptr(this IntPtr obj, uint size)
        {
            IntPtr strPtr = Marshal.AllocHGlobal((int)size);
            CopyMemory(strPtr, obj, size);
            return strPtr;
        }

        public static IntPtr IntptrToIntptr(this IntPtr obj, IntPtr des, uint size)
        {
            CopyMemory(des, obj, size);
            return des;
        }
        public static Span<byte> AsSpan(this IntPtr obj, int size)
        {
            unsafe
            {
                Span<byte> buffer = new Span<byte>((byte*)obj, size);
                return buffer;
            }
        }

        public static byte[] IntptrToBytes(this IntPtr obj, byte[] buffer, int size)
        {
            Marshal.Copy(obj, buffer, 0, size);
            return buffer;
        }

        public static IntPtr BytesToIntptr(this byte[] bytes, bool isAlloc = false)
        {
            return BytesToIntptr(bytes, 0, isAlloc);
        }
        //public static unsafe IntPtr structToIntptr<T>(this T bytes, bool isAlloc = false)
        //{
        //    IntPtr ptr;
        //    fixed (T* thisPtr = &bytes)
        //    {
        //        ptr = (IntPtr)((float*)thisPtr);
        //    }
        //    return ptr;
        //}

        public struct Vector3f
        {
            public float x;
            public float y;
            public float z;

            public unsafe float this[int index]
            {
                get
                {
                    // Get "p" somehow, so that it points to "this"...
                    fixed (Vector3f* thisPtr = &this)
                    {
                        return ((float*)thisPtr)[index];
                    }
                }
                set
                {
                    // Get "p" somehow, so that it points to "this"...
                    fixed (Vector3f* thisPtr = &this)
                    {
                        ((float*)thisPtr)[index] = value;
                    }
                }
            }
        }

        public static IntPtr BytesToIntptr(this byte[] bytes, int offset, bool isAlloc = false)
        {
            IntPtr ptr;
            if (isAlloc)
            {
                ptr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, offset, ptr, bytes.Length);
            }
            else
            {
                ptr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, offset);
                //fixed (byte* objPtr = &bytes[offset])
                //{
                //    ptr = (IntPtr)objPtr;
                //}
            }
            return ptr;
        }

        public static string Trim0(this string str)
        {
            return str.TrimEnd('\0');
        }

        public static string TrimLastChar(this string path)
        {
            return path.Remove(path.Length - 1, 1);
        }

        public static string GetEnumDescription(this Enum enumValue)
        {
            string str = enumValue.ToString();
            try
            {
                FieldInfo field = enumValue.GetType().GetField(str);
                if (field == null) return str;
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs.Length == 0) return str;
                DescriptionAttribute da = (DescriptionAttribute)objs[0];
                return da.Description;
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static T CheckValueLimit<T>(T value, T minValue, T maxValue) where T : struct, IComparable<T>, IEquatable<T>, IConvertible
        {
            if (value.CompareTo(minValue) < 0)
            {
                return minValue;
            }
            else if (value.CompareTo(maxValue) > 0)
            {
                return maxValue;
            }
            else
            {
                return value;
            }
        }

        public static double Clamp(this double value, double min, double max)
        {
            double v = value;
            double minValue = min;
            double maxValue = max;
            double ret = v < minValue ? minValue : v;
            ret = ret > maxValue ? maxValue : ret;
            return ret;
        }

        public static long Clamp(this long value, long min, long max)
        {
            long v = value;
            long minValue = min;
            long maxValue = max;
            long ret = v < minValue ? minValue : v;
            ret = ret > maxValue ? maxValue : ret;
            return ret;
        }

        public static ulong Clamp(this ulong value, ulong min, ulong max)
        {
            ulong v = value;
            ulong minValue = min;
            ulong maxValue = max;
            ulong ret = v < minValue ? minValue : v;
            ret = ret > maxValue ? maxValue : ret;
            return ret;
        }

        public static int Clamp(this int value, int min, int max)
        {
            int v = value;
            int minValue = min;
            int maxValue = max;
            int ret = v < minValue ? minValue : v;
            ret = ret > maxValue ? maxValue : ret;
            return ret;
        }

        public static uint Clamp(this uint value, uint min, uint max)
        {
            uint v = value;
            uint minValue = min;
            uint maxValue = max;
            uint ret = v < minValue ? minValue : v;
            ret = ret > maxValue ? maxValue : ret;
            return ret;
        }

        public static char[] StringToCharArray(this string str, int size = 0)
        {
            if (size > 0)
            {
                return str.ToCharArray(0, size);
            }

            return str.ToCharArray();
        }

        public static void ClearArry<T>(this T[] bytes)
        {
            Array.Clear(bytes, 0, bytes.Length);
        }

        public static string ReadAppSettings(this string parameter, string DefaultValue)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(parameter))
            {
                return ConfigurationManager.AppSettings[parameter];
            }
            return DefaultValue;
        }

        /// <summary>  
        /// 字节数组压缩  
        /// </summary>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static byte[] Compress(this byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Flush();
                zip.Close();
                byte[] buffer = ms.ToArray();
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>  
        /// 字节数组解压缩  
        /// </summary>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static byte[] Decompress(this byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                zip.Write(data, 0, data.Length);
                zip.Flush();
                zip.Close();
                byte[] buffer = ms.ToArray();
                ms.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 字符串压缩
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="str">String.</param>
        public static string Compress(this string str)
        {
            byte[] compressAfterByte = Compress(Encoding.Unicode.GetBytes(str));
            string compressString = compressAfterByte.ToBase64String();
            return compressString;
        }
        /// <summary>
        /// 字符串解压缩
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="str">String.</param>
        public static string Decompress(this string str)
        {
            byte[] compressAfterByte = Decompress(str.ToBytesBase64());
            string compressString = Encoding.Unicode.GetString(compressAfterByte);
            return compressString;
        }

        public static string ToBase64String(this byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        public static byte[] ToBytesBase64(this string base64)
        {
            return Convert.FromBase64String(base64);
        }

        public static Guid ToGuid(this byte[] buffer, int offset)
        {
#if NET8_0 || NET7_0 || NET6_0 || NET6_0_OR_GREATER || NETSTANDARD2_1 || NET5_0 || NET5_0_OR_GREATER
            return new Guid(buffer.AsSpan(offset, 16));
#else
            return new Guid(buffer.AsSpan(offset, 16).ToArray());
#endif
        }

        private static ref T ElementAt<T>(ref T[] array, int position)
        {
            return ref array[position];
        }
    }
}
