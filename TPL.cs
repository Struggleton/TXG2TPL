using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXG2TPL
{
    class TPL
    {
        private const uint _Identifier = 0x0020AF30;
        public uint ImageCount;
        public uint ImageTableOffset;

        public uint ImageHeaderOffset;
        public uint PaletteHeaderOffset;

        public ushort ColorCount;
        private ushort Padding;
        public uint PaletteFormat;
        public uint PaletteOffset;

        public ushort Height;
        public ushort Width;
        public uint ImageFormat;
        public uint ImageOffset;
        public uint WrapS;
        public uint WrapT;
        public uint MinFilter;
        public uint MagFilter;
        public float LODBias;
        public byte EdgeLOD;
        public byte MinLOD;
        public byte MaxLOD;
        public byte Unpacked;

        private int ImageSize;
        public byte[] ImageData;
        public byte[] PaletteData;

        public TPL() { }
        public TPL(Stream stream, TXGHeader txg)
        {
            Write(stream, txg);
        }
        public TPL(Stream stream)
        {
            Read(stream);
        }

        private void Write(Stream stream, TXGHeader txg)
        {
            using (BigEndianWriter writer = new BigEndianWriter(stream))
            {
                writer.Write(0x0020AF30);
                writer.Write(1);
                writer.Write(0xC);
                writer.Write(0);
                writer.Write(0x14);
                if (Convert.ToBoolean(txg.PaletteEntry))
                {
                    writer.Write((ushort)(txg.PaletteData.Length / 2));
                    writer.Write((ushort)0);
                    writer.Write(txg.PaletteFormat);
                    writer.Write(0x20);
                    writer.Write(txg.PaletteData);
                }
                writer.Write(0xC, (uint)writer.BaseStream.Position);
                writer.Write((ushort)txg.Height);
                writer.Write((ushort)txg.Width);
                writer.Write(txg.ImageFormat);
                long PastOffset = writer.BaseStream.Position;
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(1);
                writer.Write(1);
                writer.Write(0);
                writer.Write(0);
                writer.AlignPosition(0x20);
                writer.Write(PastOffset, (uint)writer.BaseStream.Position);
                writer.Write(txg.ImageData);
            }
        }
        private void Read(Stream stream)
        {
            using (BigEndianReader reader = new BigEndianReader(stream))
            {
                if (reader.ReadUInt32() != _Identifier)
                    throw new InvalidDataException("Invalid TPL file!");
                if ((ImageCount = reader.ReadUInt32()) != 1)
                    throw new InvalidDataException("TPLs with only 1 image are supported at the moment!");
                ImageTableOffset = reader.ReadUInt32();
                reader.BaseStream.Position = ImageTableOffset;
                ImageHeaderOffset = reader.ReadUInt32();
                PaletteHeaderOffset = reader.ReadUInt32();

                if (PaletteHeaderOffset != 0)
                {
                    reader.BaseStream.Position = PaletteHeaderOffset;
                    ColorCount = reader.ReadUInt16();
                    Padding = reader.ReadUInt16();
                    PaletteFormat = reader.ReadUInt32();
                    PaletteOffset = reader.ReadUInt32();
                    PaletteData = reader.ReadBytes(ColorCount * 2, (int)PaletteOffset);
                }

                reader.BaseStream.Position = ImageHeaderOffset;
                Height = reader.ReadUInt16();
                Width = reader.ReadUInt16();
                ImageFormat = reader.ReadUInt32();
                ImageOffset = reader.ReadUInt32();
                WrapS = reader.ReadUInt32();
                WrapT = reader.ReadUInt32();
                MinFilter = reader.ReadUInt32();
                MagFilter = reader.ReadUInt32();
                LODBias = reader.ReadSingle();
                EdgeLOD = reader.ReadByte();
                MinLOD = reader.ReadByte();
                MaxLOD = reader.ReadByte();
                Unpacked = reader.ReadByte();

                reader.BaseStream.Position = ImageOffset;
                if (PaletteOffset < ImageOffset) ImageSize = (int)(reader.BaseStream.Length - ImageOffset);
                else ImageSize = (int)(PaletteOffset - ImageOffset);
                ImageData = reader.ReadBytes(ImageSize);
            }
        }
    }
}
