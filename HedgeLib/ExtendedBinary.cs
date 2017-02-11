﻿using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HedgeLib
{
    public class ExtendedBinary
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct FloatUnion
        {
            [FieldOffset(0)]
            public float Float;
            [FieldOffset(0)]
            public uint UInt;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DoubleUnion
        {
            [FieldOffset(0)]
            public double Double;
            [FieldOffset(0)]
            public ulong ULong;
        }
    }

    public class ExtendedBinaryReader : BinaryReader
    {
        //Variables/Constants
        public Encoding Encoding = Encoding.ASCII;
        public long Offset = 0;
        public bool IsBigEndian = false;

        //Constructors
        public ExtendedBinaryReader(Stream input, bool isBigEndian = false) : base(input)
        {
            IsBigEndian = isBigEndian;
        }

        public ExtendedBinaryReader(Stream input, Encoding encoding, bool isBigEndian = false)
            : base(input, encoding)
        {
            Encoding = encoding;
            IsBigEndian = isBigEndian;
        }

        //Methods
        public void JumpTo(long position, bool absolute = true)
        {
            BaseStream.Position = (absolute) ? position : position + Offset;
        }

        public void JumpAhead(long amount = 1)
        {
            BaseStream.Position += amount;
        }

        public void JumpBehind(long amount = 1)
        {
            BaseStream.Position -= amount;
        }

        public void FixPadding(int amount = 4)
        {
            long jumpAmount = 0;
            while (BaseStream.Position % amount != 0) ++jumpAmount;
            JumpAhead(jumpAmount);
        }

        public string ReadNullTerminatedString()
        {
            char curChar;
            string str = "";

            do
            {
                curChar = ReadChar();
                if (curChar == '\0') break;
                str += curChar;
            }
            while (BaseStream.Position < BaseStream.Length && curChar != '\0');

            return str;
        }

        //2-Byte Types
        public override short ReadInt16()
        {
            var buffer = ReadBytes(2);
            if (IsBigEndian)
            {
                return (short)(buffer[0] << 8 | buffer[1]);
            }
            else
            {
                return (short)(buffer[1] << 8 | buffer[0]);
            }
        }

        public override ushort ReadUInt16()
        {
            var buffer = ReadBytes(2);
            if (IsBigEndian)
            {
                return (ushort)(buffer[0] << 8 | buffer[1]);
            }
            else
            {
                return (ushort)(buffer[1] << 8 | buffer[0]);
            }
        }

        //4-Byte Types
        public override int ReadInt32()
        {
            var buffer = ReadBytes(4);
            if (IsBigEndian)
            {
                return buffer[0] << 24 | buffer[1] << 16 |
                    buffer[2] << 8 | buffer[3];
            }
            else
            {
                return buffer[3] << 24 | buffer[2] << 16 |
                    buffer[1] << 8 | buffer[0];
            }
        }

        public override uint ReadUInt32()
        {
            var buffer = ReadBytes(4);
            if (IsBigEndian)
            {
                return (uint)(buffer[0] << 24 | buffer[1] << 16 |
                    buffer[2] << 8 | buffer[3]);
            }
            else
            {
                return (uint)(buffer[3] << 24 | buffer[2] << 16 |
                    buffer[1] << 8 | buffer[0]);
            }
        }

        public override float ReadSingle()
        {
            var buffer = ReadBytes(4);
            var floatUnion = new ExtendedBinary.FloatUnion();

            if (IsBigEndian)
            {
                floatUnion.UInt = (uint)(buffer[0] << 24 | buffer[1] << 16 |
                    buffer[2] << 8 | buffer[3]);
            }
            else
            {
                floatUnion.UInt = (uint)(buffer[3] << 24 | buffer[2] << 16 |
                    buffer[1] << 8 | buffer[0]);
            }

            return floatUnion.Float;
        }

        //8-Byte Types
        public override long ReadInt64()
        {
            var buffer = ReadBytes(8);
            if (IsBigEndian)
            {
                return buffer[0] << 56 | buffer[1] << 48 |
                    buffer[2] << 40 | buffer[3] << 32 |
                    buffer[4] << 24 | buffer[5] << 16 |
                    buffer[6] << 8 | buffer[7];
            }
            else
            {
                return buffer[7] << 56 | buffer[6] << 48 |
                    buffer[5] << 40 | buffer[4] << 32 |
                    buffer[3] << 24 | buffer[2] << 16 |
                    buffer[1] << 8 | buffer[0];
            }
        }

        public override ulong ReadUInt64()
        {
            var buffer = ReadBytes(8);
            if (IsBigEndian)
            {
                return (ulong)(buffer[0] << 56 | buffer[1] << 48 |
                    buffer[2] << 40 | buffer[3] << 32 |
                    buffer[4] << 24 | buffer[5] << 16 |
                    buffer[6] << 8 | buffer[7]);
            }
            else
            {
                return (ulong)(buffer[7] << 56 | buffer[6] << 48 |
                    buffer[5] << 40 | buffer[4] << 32 |
                    buffer[3] << 24 | buffer[2] << 16 |
                    buffer[1] << 8 | buffer[0]);
            }
        }

        public override double ReadDouble()
        {
            var buffer = ReadBytes(8);
            var doubleUnion = new ExtendedBinary.DoubleUnion();

            if (IsBigEndian)
            {
                doubleUnion.ULong = (ulong)(buffer[0] << 56 | buffer[1] << 48 |
                    buffer[2] << 40 | buffer[3] << 32 |
                    buffer[4] << 24 | buffer[5] << 16 |
                    buffer[6] << 8 | buffer[7]);
            }
            else
            {
                doubleUnion.ULong = (ulong)(buffer[7] << 56 | buffer[6] << 48 |
                    buffer[5] << 40 | buffer[4] << 32 |
                    buffer[3] << 24 | buffer[2] << 16 |
                    buffer[1] << 8 | buffer[0]);
            }

            return doubleUnion.Double;
        }

        //12-Byte Types
        public Vector3 ReadVector3()
        {
            float x, y, z;
            var buffer = ReadBytes(12);
            var floatUnion = new ExtendedBinary.FloatUnion();

            if (IsBigEndian)
            {
                floatUnion.UInt = (uint)(buffer[0] << 24 | buffer[1] << 16 |
                    buffer[2] << 8 | buffer[3]);
                x = floatUnion.Float;

                floatUnion.UInt = (uint)(buffer[4] << 24 | buffer[5] << 16 |
                    buffer[6] << 8 | buffer[7]);
                y = floatUnion.Float;

                floatUnion.UInt = (uint)(buffer[8] << 24 | buffer[9] << 16 |
                    buffer[10] << 8 | buffer[11]);
                z = floatUnion.Float;
            }
            else
            {
                floatUnion.UInt = (uint)(buffer[11] << 24 | buffer[10] << 16 |
                    buffer[9] << 8 | buffer[8]);
                x = floatUnion.Float;

                floatUnion.UInt = (uint)(buffer[7] << 24 | buffer[6] << 16 |
                    buffer[5] << 8 | buffer[4]);
                y = floatUnion.Float;

                floatUnion.UInt = (uint)(buffer[3] << 24 | buffer[2] << 16 |
                    buffer[1] << 8 | buffer[0]);
                z = floatUnion.Float;
            }

            return new Vector3(x, y, z);
        }

        //TODO: Write override methods for all types.
    }

    public class ExtendedBinaryWriter : BinaryWriter
    {
        //Variables/Constants
        public Dictionary<string, long> Offsets = new Dictionary<string, long>();
        public Encoding Encoding = Encoding.ASCII;
        public bool IsBigEndian = false;

        private byte[] dataBuffer = new byte[32];

        //Constructors
        public ExtendedBinaryWriter(bool isBigEndian = false) : base()
        {
            IsBigEndian = isBigEndian;
        }

        public ExtendedBinaryWriter(Stream output, bool isBigEndian = false) : base(output)
        {
            IsBigEndian = isBigEndian;
        }

        public ExtendedBinaryWriter(Stream output, Encoding encoding,
            bool isBigEndian = false) : base(output, encoding)
        {
            Encoding = encoding;
            IsBigEndian = isBigEndian;
        }

        //Methods
        public void AddOffset(string name)
        {
            if (Offsets.ContainsKey(name))
                Offsets[name] = BaseStream.Position;
            else
                Offsets.Add(name, BaseStream.Position);

            WriteNulls(4);
        }

        public void AddOffset(string name, long position)
        {
            if (Offsets.ContainsKey(name))
                Offsets[name] = position;
            else
                Offsets.Add(name, position);

            var curPos = BaseStream.Position;
            BaseStream.Position = position;

            WriteNulls(4);
            BaseStream.Position = curPos;
        }

        public void FillInOffset(string name, uint value)
        {
            var curPos = BaseStream.Position;
            BaseStream.Position = Offsets[name];

            Write(value);
            Offsets.Remove(name);

            BaseStream.Position = curPos;
        }

        public void WriteNull()
        {
            Write((byte)0);
        }

        public void WriteNulls(uint count)
        {
            byte[] nulls = new byte[count];
            Write(nulls);
        }

        public void WriteNullTerminatedString(string value)
        {
            var chArr = value.ToCharArray();
            Write(chArr, 0, chArr.Length);
            WriteNull();
        }

        //2-Byte Types
        public override void Write(short value)
        {
            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(value >> 8);
                dataBuffer[1] = (byte)(value);
            }
            else
            {
                dataBuffer[0] = (byte)(value);
                dataBuffer[1] = (byte)(value >> 8);
            }

            Write(dataBuffer, 0, 2);
        }

        public override void Write(ushort value)
        {
            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(value >> 8);
                dataBuffer[1] = (byte)(value);
            }
            else
            {
                dataBuffer[0] = (byte)(value);
                dataBuffer[1] = (byte)(value >> 8);
            }

            Write(dataBuffer, 0, 2);
        }

        //4-Byte Types
        public override void Write(int value)
        {
            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(value >> 24);
                dataBuffer[1] = (byte)(value >> 16);
                dataBuffer[2] = (byte)(value >> 8);
                dataBuffer[3] = (byte)(value);
            }
            else
            {
                dataBuffer[0] = (byte)(value);
                dataBuffer[1] = (byte)(value >> 8);
                dataBuffer[2] = (byte)(value >> 16);
                dataBuffer[3] = (byte)(value >> 24);
            }

            Write(dataBuffer, 0, 4);
        }

        public override void Write(uint value)
        {
            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(value >> 24);
                dataBuffer[1] = (byte)(value >> 16);
                dataBuffer[2] = (byte)(value >> 8);
                dataBuffer[3] = (byte)(value);
            }
            else
            {
                dataBuffer[0] = (byte)(value);
                dataBuffer[1] = (byte)(value >> 8);
                dataBuffer[2] = (byte)(value >> 16);
                dataBuffer[3] = (byte)(value >> 24);
            }

            Write(dataBuffer, 0, 4);
        }

        public override void Write(float value)
        {
            var floatUnion = new ExtendedBinary.FloatUnion();
            floatUnion.Float = value;

            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(floatUnion.UInt >> 24);
                dataBuffer[1] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[2] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[3] = (byte)(floatUnion.UInt);
            }
            else
            {
                dataBuffer[0] = (byte)(floatUnion.UInt);
                dataBuffer[1] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[2] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[3] = (byte)(floatUnion.UInt >> 24);
            }

            Write(dataBuffer, 0, 4);
        }

        //8-Byte Types
        public override void Write(long value)
        {
            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(value >> 56);
                dataBuffer[1] = (byte)(value >> 48);
                dataBuffer[2] = (byte)(value >> 40);
                dataBuffer[3] = (byte)(value >> 32);

                dataBuffer[4] = (byte)(value >> 24);
                dataBuffer[5] = (byte)(value >> 16);
                dataBuffer[6] = (byte)(value >> 8);
                dataBuffer[7] = (byte)(value);
            }
            else
            {
                dataBuffer[0] = (byte)(value);
                dataBuffer[1] = (byte)(value >> 8);
                dataBuffer[2] = (byte)(value >> 16);
                dataBuffer[3] = (byte)(value >> 24);

                dataBuffer[4] = (byte)(value >> 32);
                dataBuffer[5] = (byte)(value >> 40);
                dataBuffer[6] = (byte)(value >> 48);
                dataBuffer[7] = (byte)(value >> 56);
            }

            Write(dataBuffer, 0, 8);
        }

        public override void Write(ulong value)
        {
            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(value >> 56);
                dataBuffer[1] = (byte)(value >> 48);
                dataBuffer[2] = (byte)(value >> 40);
                dataBuffer[3] = (byte)(value >> 32);

                dataBuffer[4] = (byte)(value >> 24);
                dataBuffer[5] = (byte)(value >> 16);
                dataBuffer[6] = (byte)(value >> 8);
                dataBuffer[7] = (byte)(value);
            }
            else
            {
                dataBuffer[0] = (byte)(value);
                dataBuffer[1] = (byte)(value >> 8);
                dataBuffer[2] = (byte)(value >> 16);
                dataBuffer[3] = (byte)(value >> 24);

                dataBuffer[4] = (byte)(value >> 32);
                dataBuffer[5] = (byte)(value >> 40);
                dataBuffer[6] = (byte)(value >> 48);
                dataBuffer[7] = (byte)(value >> 56);
            }

            Write(dataBuffer, 0, 8);
        }

        public override void Write(double value)
        {
            var doubleUnion = new ExtendedBinary.DoubleUnion();
            doubleUnion.Double = value;

            if (IsBigEndian)
            {
                dataBuffer[0] = (byte)(doubleUnion.ULong >> 56);
                dataBuffer[1] = (byte)(doubleUnion.ULong >> 48);
                dataBuffer[2] = (byte)(doubleUnion.ULong >> 40);
                dataBuffer[3] = (byte)(doubleUnion.ULong >> 32);

                dataBuffer[4] = (byte)(doubleUnion.ULong >> 24);
                dataBuffer[5] = (byte)(doubleUnion.ULong >> 16);
                dataBuffer[6] = (byte)(doubleUnion.ULong >> 8);
                dataBuffer[7] = (byte)(doubleUnion.ULong);
            }
            else
            {
                dataBuffer[0] = (byte)(doubleUnion.ULong);
                dataBuffer[1] = (byte)(doubleUnion.ULong >> 8);
                dataBuffer[2] = (byte)(doubleUnion.ULong >> 16);
                dataBuffer[3] = (byte)(doubleUnion.ULong >> 24);

                dataBuffer[4] = (byte)(doubleUnion.ULong >> 32);
                dataBuffer[5] = (byte)(doubleUnion.ULong >> 40);
                dataBuffer[6] = (byte)(doubleUnion.ULong >> 48);
                dataBuffer[7] = (byte)(doubleUnion.ULong >> 56);
            }

            Write(dataBuffer, 0, 8);
        }

        //12-Byte Types
        public void Write(Vector3 vect)
        {
            var floatUnion = new ExtendedBinary.FloatUnion();
            if (IsBigEndian)
            {
                floatUnion.Float = vect.X;
                dataBuffer[0] = (byte)(floatUnion.UInt >> 24);
                dataBuffer[1] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[2] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[3] = (byte)(floatUnion.UInt);

                floatUnion.Float = vect.Y;
                dataBuffer[4] = (byte)(floatUnion.UInt >> 24);
                dataBuffer[5] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[6] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[7] = (byte)(floatUnion.UInt);

                floatUnion.Float = vect.Z;
                dataBuffer[8] = (byte)(floatUnion.UInt >> 24);
                dataBuffer[9] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[10] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[11] = (byte)(floatUnion.UInt);
            }
            else
            {
                floatUnion.Float = vect.X;
                dataBuffer[0] = (byte)(floatUnion.UInt);
                dataBuffer[1] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[2] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[3] = (byte)(floatUnion.UInt >> 24);

                floatUnion.Float = vect.Y;
                dataBuffer[4] = (byte)(floatUnion.UInt);
                dataBuffer[5] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[6] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[7] = (byte)(floatUnion.UInt >> 24);

                floatUnion.Float = vect.Z;
                dataBuffer[8] = (byte)(floatUnion.UInt);
                dataBuffer[9] = (byte)(floatUnion.UInt >> 8);
                dataBuffer[10] = (byte)(floatUnion.UInt >> 16);
                dataBuffer[11] = (byte)(floatUnion.UInt >> 24);
            }

            Write(dataBuffer, 0, 12);
        }

        //TODO: Write override methods for all types.
    }
}