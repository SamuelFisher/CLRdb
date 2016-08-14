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

namespace CLRdb
{
    public static class RdbReader
    {
        public static IDictionary<string, string> Read(Stream input)
        {
            using (var reader = new RdbStreamReader(input))
            {
                var keyValues = new Dictionary<string, string>();
                while (reader.ReadNext())
                    keyValues.Add(reader.Current.Key, reader.Current.Value);
                return keyValues;
            }
        }
    }
}
