CLRdb
=====

[![Build Status](https://travis-ci.org/SamuelFisher/CLRdb.svg?branch=master)](https://travis-ci.org/SamuelFisher/CLRdb)

CLRdb is a Redis RDB file parser for .NET.

## Installation

```PowerShell
PM> Install-Package CLRdb
```

## Features

Currently only supports string values.

## Supported Platforms

- .NET Platform Standard 1.0 and above

## Usage

Reading an RDB file into a dictionary:

```C#
IDictionary<string, string> db = RdbReader.Read(File.OpenRead("dump.rdb"));
```

Reading key/value pairs from an RDB file one-by-one:

```C#
using (var reader = new RdbStreamReader(File.OpenRead("dump.rdb"))
{
    while (reader.ReadNext())
    {
        KeyValuePair<string, string> keyValue = reader.Current;
    }
}
```
