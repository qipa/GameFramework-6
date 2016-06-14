using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


    public class CStream
    {
        public const int s_IntSize = 4;
        public const int s_UIntSize = 4;
        public const int s_LongSize = 8;
        public const int s_ULongSize = 8;
        public const int s_FloatSize = 4;
        public const int s_Float2Size = 8;
        public const int s_Float3Size = 12;
        public const int s_Float4Size = 16;
        public const int s_DoubleSize = 8;
        public const int s_CharSize = 1;
        public const int s_ByteSize = 1;
        public const int s_BoolSize = 1;
        public const int s_ShortSize = 2;

        //纯粹流式
        public CStream(Stream stream)
        {
            m_stream = stream;
        }

        public CStream(byte[] udata)
        {
            m_byte = udata;
            m_stream = new MemoryStream(udata);
        }

        public Stream GetStream()
        {
            return m_stream;
        }

        public void Close()
        {
            if (null != m_stream)
            {
                m_stream.Close();
            }
        }

        public byte[] GetBuffer()
        {
            return m_byte;
        }

        public void Seek(int iOff)
        {
            if (iOff < m_stream.Length)
            {
                m_stream.Position = iOff;
            }
        }

        public long Length()
        {
            return null != m_stream ? m_stream.Length : null != m_byte ? m_byte.Length : 0;
        }

        public int GetOffset()
        {
            if (m_stream != null)
                return (int)m_stream.Position;
            else
                return 0;

        }


        public void Reset()
        {
            m_stream.Position = 0;
            if (null != m_byte)
            {
                Array.Clear(m_byte, 0, m_byte.Length);
            }
        }

        public void Read(ref byte[] uDate)
        {
            if (m_stream == null || null == uDate)
            {
                return;
            }
            m_stream.Read(uDate, 0, uDate.Length);
        }

        public void Write(byte[] uDate)
        {
            if (m_stream == null || null == uDate)
            {
                return;
            }

            m_stream.Write(uDate, 0, uDate.Length);
            //m_nOffset += iCount;
        }

        public void Write(byte[] uDate, int len)
        {
            if (m_stream == null || null == uDate)
            {
                return;
            }

            m_stream.Write(uDate, 0, len);
            //m_nOffset += iCount;
        }

        /////////////////////////////////////////////////////////////////////
        //  [9/17/2013 Fisho]
        public void ReadByte(ref byte byDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_ByteSize];
            m_stream.Read(bit, 0, CStream.s_ByteSize);
            byDate = bit[0];
        }
        public byte ReadByte()
        {
            byte[] bit = new byte[CStream.s_ByteSize];
            m_stream.Read(bit, 0, CStream.s_ByteSize);
            return bit[0];
        }
        public void WriteByte(byte byDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes((char)byDate);
            m_stream.Write(bit, 0, CStream.s_ByteSize);
        }
        /////////////////////////////////////////////////////////////////////

        public void ReadInt(ref int uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_IntSize];
            m_stream.Read(bit, 0, CStream.s_IntSize);
            uDate = BitConverter.ToInt32(bit, 0);
        }
        public int ReadInt()
        {
            byte[] bit = new byte[CStream.s_IntSize];
            m_stream.Read(bit, 0, CStream.s_IntSize);
            return BitConverter.ToInt32(bit, 0);
        }

        public void WriteInt(int uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, CStream.s_IntSize);
        }

        public void ReadUInt(ref uint uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_IntSize];
            m_stream.Read(bit, 0, CStream.s_IntSize);
            uDate = BitConverter.ToUInt32(bit, 0);
            return;
        }
        public uint ReadUInt()
        {
            if (m_stream == null)
            {
                return 0;
            }
            byte[] bit = new byte[CStream.s_IntSize];
            m_stream.Read(bit, 0, CStream.s_IntSize);
            return BitConverter.ToUInt32(bit, 0);
        }

        public void WriteUInt(uint uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, CStream.s_IntSize);
        }

        public void ReadUShort(ref ushort uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_ShortSize];
            m_stream.Read(bit, 0, CStream.s_ShortSize);
            uDate = BitConverter.ToUInt16(bit, 0);
            return;
        }
        public ushort ReadUShort()
        {
            if (m_stream == null)
            {
                return 0;
            }
            byte[] bit = new byte[CStream.s_ShortSize];
            m_stream.Read(bit, 0, CStream.s_ShortSize);
            return BitConverter.ToUInt16(bit, 0);
        }

        public void WriteUShort(ushort uDate)
        {
            if (m_stream == null)
            {
                return;
            }

            m_stream.Write(BitConverter.GetBytes(uDate), 0, CStream.s_ShortSize);
        }

        public void WriteShort(short uDate)
        {
            if (m_stream == null)
            {
                return;
            }

            m_stream.Write(BitConverter.GetBytes(uDate), 0, CStream.s_ShortSize);
        }

        public void ReadLong(ref long uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_LongSize];
            m_stream.Read(bit, 0, CStream.s_LongSize);
            uDate = BitConverter.ToInt64(bit, 0);
        }

        public void ReadULong(ref ulong uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_ULongSize];
            m_stream.Read(bit, 0, CStream.s_ULongSize);
            uDate = BitConverter.ToUInt64(bit, 0);
        }

        public long ReadLong()
        {
            if (m_stream == null)
            {
                return -1;
            }
            byte[] bit = new byte[CStream.s_LongSize];
            m_stream.Read(bit, 0, CStream.s_LongSize);
            return BitConverter.ToInt64(bit, 0);
        }

        public ulong ReadULong()
        {
            if (m_stream == null)
            {
                return 0xFFFFFFFF;
            }
            byte[] bit = new byte[CStream.s_ULongSize];
            m_stream.Read(bit, 0, CStream.s_ULongSize);
            return BitConverter.ToUInt64(bit, 0);
        }

        public int GetRest()
        {
            if (null == m_stream)
            {
                return -1;
            }
            return (int)m_stream.Length - (int)m_stream.Position;
        }

        public void WriteLong(long uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, CStream.s_LongSize);
        }

        public void WriteULong(ulong uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, CStream.s_ULongSize);
        }

        public void ReadBool(ref bool uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_BoolSize];
            m_stream.Read(bit, 0, CStream.s_BoolSize);
            uDate = BitConverter.ToBoolean(bit, 0);
        }

        public bool ReadBool()
        {
            if (m_stream == null)
            {
                return false;
            }
            byte[] bit = new byte[CStream.s_BoolSize];
            m_stream.Read(bit, 0, CStream.s_BoolSize);
            return BitConverter.ToBoolean(bit, 0);
        }

        public void WriteBool(bool uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, CStream.s_BoolSize);
        }

        public void WriteVector2(float x, float y)
        {
            WriteFloat(x);
            WriteFloat(y);
        }

        public void ReadVector2(ref float x, ref float y)
        {
            ReadFloat(ref x);
            ReadFloat(ref y);
        }

        public void WriteVector3(float x, float y, float z)
        {
            WriteFloat(x);
            WriteFloat(y);
            WriteFloat(z);
        }

        public void WriteVector4(float x, float y, float z, float w)
        {
            WriteFloat(x);
            WriteFloat(y);
            WriteFloat(z);
            WriteFloat(w);
        }

        public void ReadVector3(ref float x, ref float y, ref float z)
        {
            ReadFloat(ref x);
            ReadFloat(ref y);
            ReadFloat(ref z);
        }


        public void ReadColor(ref Color col)
        {
            ReadFloat(ref col.r);
            ReadFloat(ref col.g);
            ReadFloat(ref col.b);
            ReadFloat(ref col.a);
        }

        public void WriteColor(ref Color col)
        {
            WriteFloat(col.r);
            WriteFloat(col.g);
            WriteFloat(col.b);
            WriteFloat(col.a);
        }

        public void WriteVector2(ref Vector2 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
        }

        public void ReadVector2(ref Vector2 vec)
        {
            ReadFloat(ref vec.x);
            ReadFloat(ref vec.y);
        }

        public void WriteVector3(ref Vector3 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
        }

        public void WriteVector4(ref Quaternion vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
            WriteFloat(vec.w);
        }

        public void WriteVector4(ref Vector4 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
            WriteFloat(vec.w);
        }

        public void ReadVector3(ref Vector3 vec)
        {
            ReadFloat(ref vec.x);
            ReadFloat(ref vec.y);
            ReadFloat(ref vec.z);
        }

        public void ReadVector4(ref Vector4 vec)
        {
            ReadFloat(ref vec.x);
            ReadFloat(ref vec.y);
            ReadFloat(ref vec.z);
            ReadFloat(ref vec.w);
        }

        public void ReadVector4(ref Quaternion vec)
        {
            ReadFloat(ref vec.x);
            ReadFloat(ref vec.y);
            ReadFloat(ref vec.z);
            ReadFloat(ref vec.w);
        }

        public void WriteFloat4(float x, float y, float z, float w)
        {
            WriteFloat(x);
            WriteFloat(y);
            WriteFloat(z);
            WriteFloat(w);
        }

        public void ReadFloat4(ref float x, ref float y, ref float z, ref float w)
        {
            ReadFloat(ref x);
            ReadFloat(ref y);
            ReadFloat(ref z);
            ReadFloat(ref w);
        }

        public void ReadFloat(ref float uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_FloatSize];
            m_stream.Read(bit, 0, CStream.s_FloatSize);
            uDate = BitConverter.ToSingle(bit, 0);
        }

        public void WriteFloat(float uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, CStream.s_FloatSize);
        }

        public void ReadDouble(ref double uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = new byte[CStream.s_DoubleSize];
            m_stream.Read(bit, 0, CStream.s_DoubleSize);
            uDate = BitConverter.ToDouble(bit, 0);
        }

        public void WriteDouble(double uDate)
        {
            if (m_stream == null)
            {
                return;
            }
            byte[] bit = BitConverter.GetBytes(uDate);
            m_stream.Write(bit, 0, CStream.s_DoubleSize);
        }

        public bool ReadString(out string strOut)
        {
            strOut = null;
            if (null == m_stream)
            {
                return false;
            }
            int iLenght = 0;
            ReadInt(ref iLenght);
            if (iLenght <= 0)
            {
                strOut = null;
                return true;
            }
            byte[] uData = new byte[iLenght];
            Read(ref uData);
            //读出来的都是unicode
            strOut = Encoding.UTF8.GetString(uData);
            return true;
        }

        public void ReadString(out string strOut, int size)
        {
            strOut = null;
            if (null == m_stream)
            {
                return;
            }
            byte[] uData = new byte[size];
            Read(ref uData);
            //读出来的都是unicode
            strOut = StringHelper.getUnicodeStringByBytes(ref uData);
        }

        public static uint StringInFileSize(ref string strOut)
        {
            if (string.IsNullOrEmpty(strOut))
            {
                return (uint)CStream.s_UIntSize;
            }
            byte[] uData = Encoding.UTF8.GetBytes(strOut);
            return (uint)CStream.s_UIntSize + (uint)uData.Length;
        }

        public bool WriteString(string strOut)
        {
            if (m_stream == null)
            {
                return false;
            }

            byte[] uData = null;
            int iLenght = 0;
            if (!string.IsNullOrEmpty(strOut))
            {
                uData = Encoding.UTF8.GetBytes(strOut);
                iLenght = uData.Length;
                WriteInt(iLenght);
                Write(uData);
            }
            else
            {
                WriteInt(iLenght);
            }
            return true;
        }

        private Stream m_stream = null;
        //private int         m_nOffset = 0;
        private byte[] m_byte = null;
    }
