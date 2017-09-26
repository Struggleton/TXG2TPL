using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXG2TPL
{
    class TXG
    {
        public uint FileCount;
        public uint[] Offsets;

        public TXG(Stream stream, string[] files)
        {
            using (BigEndianWriter writer = new BigEndianWriter(stream))
            {
                
                writer.Write(files.Length);
                Offsets = new uint[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    writer.Write(0);
                }
                writer.AlignPosition(0x20, 0xFF);

                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine(files[i]);
                    using (FileStream fStream = File.OpenRead(files[i]))
                    {
                        Offsets[i] = (uint)writer.BaseStream.Position;
                        TXGHeader txg = new TXGHeader(writer, new TPL(fStream));
                    }
                }

                writer.BaseStream.Position = 0x04;
                for (int i = 0; i < files.Length; i++)
                {
                    writer.Write(Offsets[i]);
                }
            }
            
        }

        public TXG(Stream stream, string outputDir)
        {
            using (BigEndianReader reader = new BigEndianReader(stream))
            {
                FileCount = reader.ReadUInt32();
                Offsets = new uint[FileCount];
                for (int i = 0; i < FileCount; i++)
                {
                    Offsets[i] = reader.ReadUInt32();
                }

                for (int i = 0; i < FileCount; i++)
                {
                    reader.BaseStream.Position = Offsets[i];
                    TXGHeader header = new TXGHeader(reader);
                    string fileName = Path.Combine(outputDir, i + ".tpl");
                    MemoryStream mStream = new MemoryStream();
                    TPL tpl = new TPL(header, mStream);
                    File.WriteAllBytes(fileName, mStream.ToArray());
                }
            }
        }
    }

    class TXGHeader
    {
        public uint ImageCount;
        public uint ImageFormat;
        public uint PaletteFormat;
        public uint Width;
        public uint Height;
        public uint unk_0x14;
        public uint[] ImageOffsets;
        public uint[] PaletteOffsets;

        public byte[][] ImageData;
        public byte[][] PaletteData;
        public ImageDataFormat _Format;

        public TXGHeader(BigEndianWriter writer, TPL tpl)
        {
            writer.Write(tpl.ImageCount);
            writer.Write(tpl.ImageFormat);
            writer.Write(tpl.PaletteFormat);
            writer.Write((uint)tpl.Width);
            writer.Write((uint)tpl.Height);
            writer.Write(1);

            ImageOffsets = new uint[tpl.ImageCount];
            PaletteOffsets = new uint[tpl.ImageCount];

            long PastOffset = writer.BaseStream.Position;
            for (int i = 0; i < tpl.ImageData.Length; i++)
            {
                writer.Write(0);
            }

            for (int i = 0; i < tpl.PaletteData.Length; i++)
            {
                writer.Write(0);
            }
            writer.AlignPosition(0x20, 0xFF);

            for (int i = 0; i < tpl.ImageData.Length; i++)
            {
                ImageOffsets[i] = (uint)writer.BaseStream.Position;
                writer.Write(tpl.ImageData[i]);
            }

            for (int i = 0; i < tpl.PaletteData.Length; i++)
            {
                PaletteOffsets[i] = (uint)writer.BaseStream.Position;
                writer.Write(tpl.PaletteData[i]);
            }

            long CurrentOffset = writer.BaseStream.Position;
            writer.BaseStream.Position = PastOffset;

            for (int i = 0; i < tpl.ImageData.Length; i++)
            {
                writer.Write(ImageOffsets[i]);
            }

            for (int i = 0; i < tpl.PaletteData.Length; i++)
            {
                writer.Write(PaletteOffsets[i]);
            }
            writer.BaseStream.Position = CurrentOffset;
        }

        public TXGHeader(BigEndianReader reader)
        {
            ImageCount = reader.ReadUInt32();
            ImageFormat = reader.ReadUInt32();
            PaletteFormat = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            unk_0x14 = reader.ReadUInt32();
            _Format = ImageDataFormat.GetFormat((int)ImageFormat);

            ImageData = new byte[ImageCount][];
            PaletteData = new byte[ImageCount][];

            for (int i = 0; i < ImageCount; i++)
            {
                int ImageSize = _Format.CalculateDataSize((int)Width, (int)Height);
                ImageData[i] = reader.ReadBytes(ImageSize, (int)reader.ReadUInt32());
            }

            if (_Format.HasPalette)
            {
                for (int i = 0; i < ImageCount; i++)
                {
                    PaletteData[i] = reader.ReadBytes(0x200, (int)reader.ReadUInt32());
                }
            }
        }
    }
}
