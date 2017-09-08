using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TXG2TPL
{
    public class BigEndianReader : BinaryReader
    {
        private byte[] a16 = new byte[2];
        private byte[] a32 = new byte[4];
        private byte[] a64 = new byte[8];

        public BigEndianReader(Stream stream) : base(stream) { }

        public string ReadString(int count)
        {
            return System.Text.Encoding.ASCII.GetString(base.ReadBytes(count));
        }

        public override Int16 ReadInt16()
        {
            a16 = base.ReadBytes(2);
            Array.Reverse(a16);
            return BitConverter.ToInt16(a16, 0);
        }

        public override int ReadInt32()
        {
            a32 = base.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToInt32(a32, 0);
        }

        public byte[] ReadBytes(int count, int offset)
        {
            long pastOffset = base.BaseStream.Position;
            base.BaseStream.Position = offset;
            byte[] data = base.ReadBytes(count);
            base.BaseStream.Position = pastOffset;
            return data;
        }

        public override Int64 ReadInt64()
        {
            a64 = base.ReadBytes(8);
            Array.Reverse(a64);
            return BitConverter.ToInt64(a64, 0);
        }

        public override UInt16 ReadUInt16()
        {
            a16 = base.ReadBytes(2);
            Array.Reverse(a16);
            return BitConverter.ToUInt16(a16, 0);
        }

        public override UInt32 ReadUInt32()
        {
            a32 = base.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToUInt32(a32, 0);
        }

        public override Single ReadSingle()
        {
            a32 = base.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToSingle(a32, 0);
        }

        public override UInt64 ReadUInt64()
        {
            a64 = base.ReadBytes(8);
            Array.Reverse(a64);
            return BitConverter.ToUInt64(a64, 0);
        }

        public override Double ReadDouble()
        {
            a64 = base.ReadBytes(8);
            Array.Reverse(a64);
            return BitConverter.ToUInt64(a64, 0);
        }

        public string ReadStringToNull()
        {
            string result = "";
            char c;
            for (int i = 0; i < base.BaseStream.Length; i++)
            {
                if ((c = (char)base.ReadByte()) == 0)
                {
                    break;
                }
                result += c.ToString();
            }
            return result;
        }
    }

    public class BigEndianWriter : BinaryWriter
    {
        private byte[] a16 = new byte[2];
        private byte[] a32 = new byte[4];
        private byte[] a64 = new byte[8];

        public BigEndianWriter(Stream output) : base(output) { }
        public override void Write(Int16 value)
        {
            a16 = BitConverter.GetBytes(value);
            Array.Reverse(a16);
            base.Write(a16);
        }

        public override void Write(Int32 value)
        {
            a32 = BitConverter.GetBytes(value);
            Array.Reverse(a32);
            base.Write(a32);
        }

        public override void Write(Int64 value)
        {
            a64 = BitConverter.GetBytes(value);
            Array.Reverse(a64);
            base.Write(a64);
        }

        public void Write(long position, byte[] data)
        {
            long ret = base.BaseStream.Position;
            base.BaseStream.Position = position;
            base.Write(data);
            base.BaseStream.Position = ret;
        }

        public void Write(long position, uint value)
        {
            long ret = base.BaseStream.Position;
            base.BaseStream.Position = position;
            a32 = BitConverter.GetBytes(value);
            Array.Reverse(a32);
            base.Write(a32);
            base.BaseStream.Position = ret;
        }

        public override void Write(UInt16 value)
        {
            a16 = BitConverter.GetBytes(value);
            Array.Reverse(a16);
            base.Write(a16);
        }

        public override void Write(UInt32 value)
        {
            a32 = BitConverter.GetBytes(value);
            Array.Reverse(a32);
            base.Write(a32);
        }

        public override void Write(Single value)
        {
            a32 = BitConverter.GetBytes(value);
            Array.Reverse(a32);
            base.Write(a32);
        }

        public override void Write(UInt64 value)
        {
            a64 = BitConverter.GetBytes(value);
            Array.Reverse(a64);
            base.Write(a64);
        }

        public override void Write(Double value)
        {
            a64 = BitConverter.GetBytes(value);
            Array.Reverse(a64);
            base.Write(a64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Align(int value, int alignment)
        {
            return (value + (alignment - 1)) & ~(alignment - 1);
        }

        public void AlignPosition(int alignmentBytes, byte paddingValue = 0x00)
        {
            long align = Align((int)base.BaseStream.Position, alignmentBytes);
            byte[] array = Enumerable.Repeat(paddingValue, (int)(align - base.BaseStream.Position)).ToArray();
            base.Write(array);
        }
    }
}