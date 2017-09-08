using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TXG2TPL
{
    class TXG
    {
        public uint FileCount;
        public uint[] Offsets;
        public TXG() { }

        public TXG(Stream stream)
        {
            Read(stream);
        }

        public TXG(Stream stream, string[] files)
        {
            Write(stream, files);
        }
        
        private void Read(Stream stream)
        {
            using (BigEndianReader reader = new BigEndianReader(stream))
            {
                FileCount = reader.ReadUInt32();
                Offsets = new uint[FileCount + 1];
                for (int i = 0; i < FileCount; i++)
                {
                    Offsets[i] = reader.ReadUInt32();
                }
                Offsets[FileCount] = (uint)reader.BaseStream.Length;

                for (int i = 0; i < FileCount; i++)
                {
                    reader.BaseStream.Position = Offsets[i];
                    TXGHeader TXGHeader = new TXGHeader(reader, Offsets[i + 1]);
                    MemoryStream mStream = new MemoryStream();
                    TPL tpl = new TPL(mStream, TXGHeader);
                    File.WriteAllBytes(string.Format("{0}.tpl", i), mStream.ToArray());
                }
            }
        }

        private void Write(Stream stream, string[] files)
        {
            using (BigEndianWriter writer = new BigEndianWriter(stream))
            {
                writer.Write(files.Length);
                for (int i = 0; i < files.Length; i++)
                {
                    writer.Write(0);
                }

                writer.AlignPosition(0x20, 0xFF);
                Offsets = new uint[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    using (FileStream fStream = File.OpenRead(files[i]))
                    {
                        Offsets[i] = (uint)writer.BaseStream.Position;
                        TPL tpl = new TPL(fStream);
                        TXGHeader header = new TXGHeader(writer, tpl);
                        writer.AlignPosition(0x20);
                    }
                }

                writer.BaseStream.Position = 0x04;
                for (int i = 0; i < files.Length; i++)
                {
                    writer.Write(Offsets[i]);
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
        public uint PaletteEntry;
        public uint DataOffset;
        public uint PaletteOffset;
        public byte[] ImageData;
        public byte[] PaletteData;

        public TXGHeader() { }

        public TXGHeader(BigEndianReader reader, uint TXGOffset)
        {
            Read(reader, TXGOffset);
        }

        public TXGHeader(BigEndianWriter writer, TPL tpl)
        {
            Write(writer, tpl);
        }

        private void Write(BigEndianWriter writer, TPL tpl)
        {
            if (tpl.ImageCount != 1)
                throw new InvalidDataException("Only one TPL image is supported at this time!");
            writer.Write(tpl.ImageCount);
            writer.Write(tpl.ImageFormat);
            writer.Write(tpl.PaletteFormat);
            writer.Write((uint)tpl.Width);
            writer.Write((uint)tpl.Height);
            writer.Write(0);
            if (tpl.PaletteOffset != 0)
            {
                writer.Seek(-4, SeekOrigin.Current);
                writer.Write(1); // PaletteEntry?
            }
                
            long DataCurr = writer.BaseStream.Position;
            writer.Write(0);
            writer.Write(0xFFFFFFFF);
            writer.AlignPosition(0x20);

            writer.Write(DataCurr, (uint)writer.BaseStream.Position);
            writer.Write(tpl.ImageData);

            if (tpl.PaletteOffset != 0)
            {
                writer.Write(DataCurr + 4, (uint)writer.BaseStream.Position);
                writer.Write(tpl.PaletteData);
            }
        }

        private void Read(BigEndianReader reader, uint Offset)
        {
            ImageCount = reader.ReadUInt32();
            ImageFormat = reader.ReadUInt32();
            PaletteFormat = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            PaletteEntry = reader.ReadUInt32();
            DataOffset = reader.ReadUInt32();
            PaletteOffset = reader.ReadUInt32();

            if (PaletteEntry == 1)
            {
                ImageData = reader.ReadBytes((int)(PaletteOffset - DataOffset));
                PaletteData = reader.ReadBytes((int)(Offset - PaletteOffset));
            }

            else
            {
                ImageData = reader.ReadBytes((int)(Offset - DataOffset));
            }
        }
    }
}
