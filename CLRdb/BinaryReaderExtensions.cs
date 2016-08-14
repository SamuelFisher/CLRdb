// Redis RDB file parser for .NET https://github.com/SamuelFisher/CLRdb
//
// Copyright(C) 2016 Samuel Fisher
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lzf;

namespace CLRdb
{
    static class BinaryReaderExtensions
    {
        public static int ReadVariableLengthInt32(this BinaryReader reader)
        {
            int value;
            if (!reader.ReadVariableLengthInt32(out value))
                throw new IOException();
            return value;
        }

        public static bool ReadVariableLengthInt32(this BinaryReader reader, out int value)
        {
            byte first = reader.ReadByte();
            int startBits = first >> 6;

            switch (startBits)
            {
                case 0:
                    value = first & 0x3F;
                    return true;
                case 1:
                    byte second = reader.ReadByte();
                    value = ((first & 0x3F) << 8) + second;
                    return true;
                case 2:
                    value = reader.ReadInt32();
                    return true;
                default: // Special format
                    value = first & 0x3F;
                    return false;
            }
        }

        public static byte[] ReadRedisString(this BinaryReader reader)
        {
            int value;
            if (reader.ReadVariableLengthInt32(out value))
                return reader.ReadBytes(value);

            switch (value)
            {
                case 0:
                    return reader.ReadBytes(1);
                case 1:
                    return reader.ReadBytes(2);
                case 2:
                    return reader.ReadBytes(4);
                case 3:
                    return ReadCompressedString(reader);
            }

            throw new IOException("Invalid format.");
        }

        private static byte[] ReadCompressedString(BinaryReader reader)
        {
            int compressedLength = reader.ReadVariableLengthInt32();
            int uncompressedLength = reader.ReadVariableLengthInt32();
            byte[] compressed = reader.ReadBytes(compressedLength);

            var lzf = new LZF();
            byte[] uncompressed = new byte[uncompressedLength];
            lzf.Decompress(compressed, compressedLength, uncompressed, uncompressedLength);
            return uncompressed;
        }
    }
}
