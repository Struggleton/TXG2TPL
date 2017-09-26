using System;
using System.Collections.Generic;
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
        public uint TableOffset;

        public uint[] ITableOffsets;
        public uint[] PTableOffsets;

        public ushort EntryCount;
        private ushort _Padding;
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
        public byte EnableEdgeLOD;
        public byte MinLOD;
        public byte MaxLOD;
        private byte _Unpacked;
        public ImageDataFormat _Format;

        public byte[][] ImageData;
        public byte[][] PaletteData;
        
        public TPL(Stream stream)
        {
            using (BigEndianReader reader = new BigEndianReader(stream))
            {
                if (reader.ReadUInt32() != _Identifier)
                    throw new InvalidDataException(Strings.InvalidTPL);
                ImageCount = reader.ReadUInt32();
                TableOffset = reader.ReadUInt32();

                ITableOffsets = new uint[ImageCount];
                PTableOffsets = new uint[ImageCount];
                
                for (int i = 0; i < ImageCount; i++)
                {
                    ITableOffsets[i] = reader.ReadUInt32();
                    PTableOffsets[i] = reader.ReadUInt32();
                }

                ImageData = new byte[ImageCount][];
                PaletteData = new byte[ImageCount][];
                for (int i = 0; i < ImageCount; i++)
                {
                    reader.BaseStream.Position = ITableOffsets[i];
                    Height = reader.ReadUInt16();
                    Width = reader.ReadUInt16();
                    ImageFormat = reader.ReadUInt32();
                    ImageOffset = reader.ReadUInt32();
                    WrapS = reader.ReadUInt32();
                    WrapT = reader.ReadUInt32();
                    MinFilter = reader.ReadUInt32();
                    MagFilter = reader.ReadUInt32();
                    LODBias = reader.ReadSingle();
                    EnableEdgeLOD = reader.ReadByte();
                    MinLOD = reader.ReadByte();
                    MaxLOD = reader.ReadByte();
                    _Unpacked = reader.ReadByte();
                    _Format = ImageDataFormat.GetFormat((int)ImageFormat);
                    int ImageSize = _Format.CalculateDataSize(Width, Height);
                    ImageData[i] = reader.ReadBytes(ImageSize, (int)ImageOffset); 
                }

                if (_Format.HasPalette)
                {
                    for (int i = 0; i < ImageCount; i++)
                    {
                        reader.BaseStream.Position = PTableOffsets[i];
                        EntryCount = reader.ReadUInt16();
                        _Padding = reader.ReadUInt16();
                        PaletteFormat = reader.ReadUInt32();
                        PaletteOffset = reader.ReadUInt32();
                        PaletteData[i] = reader.ReadBytes(EntryCount * 2, (int)PaletteOffset);
                    }
                }
                
            }
        }

        public TPL(TXGHeader header, Stream stream)
        {
            using (BigEndianWriter writer = new BigEndianWriter(stream))
            {
                writer.Write(_Identifier);
                writer.Write(header.ImageCount);
                writer.Write(0x0C);

                ITableOffsets = new uint[header.ImageCount];
                PTableOffsets = new uint[header.ImageCount];

                for (int i = 0; i < header.ImageCount; i++)
                {
                    writer.Write(0);
                    writer.Write(0);
                }

                if (header._Format.HasPalette)
                {
                    for (int i = 0; i < header.ImageCount; i++)
                    {
                        PTableOffsets[i] = (uint)writer.BaseStream.Position;
                        writer.Write((ushort)(header.PaletteData[i].Length / 2));
                        writer.Write((ushort)0);
                        writer.Write(header.PaletteFormat);
                        writer.Write(0);
                        writer.AlignPosition(0x20);
                        writer.Write(PTableOffsets[i] + 0x08, (uint)writer.BaseStream.Position);
                        writer.Write(header.PaletteData[i]);
                    }
                }

                for (int i = 0; i < header.ImageCount; i++)
                {
                    ITableOffsets[i] = (uint)writer.BaseStream.Position;
                    writer.Write((ushort)header.Height);
                    writer.Write((ushort)header.Width);
                    writer.Write(header.ImageFormat);
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
                    writer.Write(header.ImageData[i]);
                }

                writer.BaseStream.Position = 0x0C;
                for (int i = 0; i < header.ImageCount; i++)
                {
                    writer.Write(ITableOffsets[i]);
                    writer.Write(PTableOffsets[i]);
                }
                writer.BaseStream.Position = 0x00;
            }
        }
    }
}
