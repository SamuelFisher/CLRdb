// Redis DB file parser for .NET https://github.com/SamuelFisher/CLRdb
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

namespace CLRdb
{
    public class RdbStreamReader : IDisposable
    {
        private readonly BinaryReader reader;
        private readonly Dictionary<string, string> auxiliaryFields;

        private bool initialized = false;

        public RdbStreamReader(Stream input)
        {
            reader = new BinaryReader(input);
            auxiliaryFields = new Dictionary<string, string>();
            Database = -1;
        }

        public IReadOnlyDictionary<string, string> AuxiliaryField => auxiliaryFields;

        public int Database { get; private set; }

        public KeyValuePair<string, string> Current { get; private set; }

        public void ReadHeader()
        {
            if (!initialized)
                Initialize();
        }

        public bool ReadNext()
        {
            RdbField read;
            do
            {
                read = Read();
            } while (read != RdbField.EndOfFile && !Enum.IsDefined(typeof(RdbValue), (RdbValue)read));

            return read != RdbField.EndOfFile;
        }

        private RdbField Read()
        {
            if (!initialized)
                Initialize();

            var field = (RdbField)reader.ReadByte();
            switch (field)
            {
                case RdbField.AuxiliaryField:
                {
                    byte[] fieldKey = reader.ReadRedisString();
                    string fieldStringKey = Encoding.UTF8.GetString(fieldKey, 0, fieldKey.Length);
                    byte[] fieldValue = reader.ReadRedisString();
                    string fieldStringValue = Encoding.UTF8.GetString(fieldValue, 0, fieldValue.Length);
                    auxiliaryFields.Add(fieldStringKey, fieldStringValue);
                    break;
                }
                case RdbField.ResizeDb:
                {
                    int hashTableSize = reader.ReadVariableLengthInt32();
                    int expiryHashTableSize = reader.ReadVariableLengthInt32();
                    break;
                }
                case RdbField.DatabaseSelector:
                {
                    Database = reader.ReadVariableLengthInt32();
                    break;
                }
                case RdbField.ExpireTimeMilliseconds:
                {
                    reader.ReadBytes(8); // Unix timestamp expiry
                    break;
                }
                case RdbField.EndOfFile:
                {
                    break;
                }
                default:
                {
                    // Plain key-value
                    ReadKeyValue((RdbValue)field);
                    break;
                }
            }

            return field;
        }

        private void Initialize()
        {
            reader.ReadBytes(5); // REDIS
            reader.ReadBytes(4); // version
            initialized = true;
        }

        private void ReadKeyValue(RdbValue type)
        {
            byte[] key = reader.ReadRedisString();
            string keyString = Encoding.UTF8.GetString(key, 0, key.Length);

            switch (type)
            {
                case RdbValue.String:
                    byte[] value = reader.ReadRedisString();
                    string valueString = Encoding.UTF8.GetString(value, 0, value.Length);
                    Current = new KeyValuePair<string, string>(keyString, valueString);
                    break;
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        private enum RdbField
        {
            AuxiliaryField = 0xFA,
            ResizeDb = 0xFB,
            ExpireTimeMilliseconds = 0xFC,
            ExpireTime = 0xFD,
            DatabaseSelector = 0xFE,
            EndOfFile = 0xFF
        }

        private enum RdbValue
        {
            String = 0,
            List = 1,
            Set = 2,
            SortedSet = 3,
            Hash = 4,
            Zipmap = 9,
            Ziplist = 10,
            Intset = 11,
            SortedSetZiplist = 12,
            HashmapZiplist = 13
        }
    }
}
