using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXG2TPL
{
    public static class Strings
    {
        public const string UsageMessage = "\nTXG2TPL: Written by Struggleton\n" +
                           "Unpacking: TXG2TPL -u <File Name> <Output Directory>\n" +
                           "Packing: TXG2TPL -p <Input Directory> <Output File Name>";
        public const string PackedMessage = "Packed {0} into {1}!";
        public const string UnpackedMessage = "Unpacked {0} to {1}!";
        public const string TXGMessage = "Image Count: {0}\n" +
                                         "Image Format: {1}\n" +
                                         "Palette Format: {2}\n" +
                                         "Width: {3}\n" +
                                         "Height: {4}\n" +
                                         "Single Image: {5}\n";

        public const string DirectoryDoesNotExist = "That directory doesn't exist!";
        public const string InvalidFormat = "Invalid Image Format!";
        public const string InvalidTPL = "Invalid TPL file!";
        
    }
}
