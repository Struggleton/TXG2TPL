using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXG2TPL
{
    class ImageDataFormat
    {
        public string Name;
        public bool HasPalette;
        public int BitsPerPixel;
        public int BlockWidth;
        public int BlockHeight;

        public static ImageDataFormat I4 = new ImageDataFormat("I4", false, 4, 8, 8);
        public static ImageDataFormat I8 = new ImageDataFormat("I8", false, 8, 8, 4);
        public static ImageDataFormat IA4 = new ImageDataFormat("IA4", false, 8, 8, 4);
        public static ImageDataFormat IA8 = new ImageDataFormat("IA8", false, 16, 4, 4);
        public static ImageDataFormat RGB565 = new ImageDataFormat("RGB565", false, 16, 4, 4);
        public static ImageDataFormat RGB5A3 = new ImageDataFormat("RGB5A3", false, 16, 4, 4);
        public static ImageDataFormat RGBA8 = new ImageDataFormat("RGBA8", false, 32, 4, 4);
        public static ImageDataFormat C4 = new ImageDataFormat("C4", true, 4, 8, 8);
        public static ImageDataFormat C8 = new ImageDataFormat("C8", true, 8, 8, 4);
        public static ImageDataFormat C14X2 = new ImageDataFormat("C14X2", true, 16, 4, 4);
        public static ImageDataFormat CMPR = new ImageDataFormat("CMPR", false, 4, 4, 8);

        
        public ImageDataFormat(string name, bool hasPalette, int bitsPerPixel, int blockWidth, int blockHeight)
        {
            Name = name;
            HasPalette = hasPalette;
            BitsPerPixel = bitsPerPixel;
            BlockWidth = blockWidth;
            BlockHeight = blockHeight;
        }

        public static ImageDataFormat GetFormat(int formatID)
        {
            switch (formatID)
            {
                case 0x00: return I4;
                case 0x01: return I8;
                case 0x02: return IA4;
                case 0x03: return IA8;
                case 0x04: return RGB565;
                case 0x05: return RGB5A3;
                case 0x06: return RGBA8;
                case 0x08: return C4;
                case 0x09: return C8;
                case 0x0A: return C14X2;
                case 0x0E: return CMPR;
                default: throw new InvalidDataException(Strings.InvalidFormat);
            }
        }

        public int CalculateDataSize(int width, int height)
        {
            return (RoundWidth(width) * RoundHeight(height) * BitsPerPixel >> 3);
        }

        private int RoundWidth(int width)
        {
            return width + ((BlockWidth - (width % BlockWidth)) % BlockWidth);
        }

        private int RoundHeight(int height)
        {
            return height + ((BlockHeight - (height % BlockHeight)) % BlockHeight);
        }
    }
}
