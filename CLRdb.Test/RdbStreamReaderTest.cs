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
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CLRdb.Test
{
    [TestFixture]
    public class RdbStreamReaderTest
    {
        private RdbStreamReader reader;

        [SetUp]
        public void Setup()
        {
            reader = new RdbStreamReader(typeof(RdbStreamReaderTest).GetTypeInfo().Assembly.GetManifestResourceStream("CLRdb.Test.dump.rdb"));
        }

        [Test]
        public void DatabaseNumber()
        {
            reader.ReadNext();
            Assert.That(reader.Database, Is.EqualTo(0));
        }
    }
}
