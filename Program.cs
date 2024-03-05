﻿//Written for ﻿games in the Spiller Engine, by Ratalaika Games. https://ratalaikagames.com/
//The Minotaur https://store.steampowered.com/app/412540/
//Defend Your Crypt https://store.steampowered.com/app/457450/
//League of Evil https://store.steampowered.com/app/491060/
//Squareboy vs Bullies Arena Edition https://store.steampowered.com/app/709620/
using System;
using System.IO;
using System.IO.Compression;

namespace Ratalaika_Games_Spiller_Engine_Extractor
{
    class Program
    {
        public static BinaryReader br;

        private static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));
            br.ReadChars(15);//"Ratalaika Games"
            br.BaseStream.Position = 32;
            int ZLibSize = br.ReadInt32();
            int unknown1 = br.ReadInt32();
            int unknown2 = br.ReadInt32();
            int unknown3 = br.ReadInt32();
            Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]));

            MemoryStream fileData = new();
            br.ReadInt16();
            using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(ZLibSize - 2)), CompressionMode.Decompress))
                ds.CopyTo(fileData);
            
            ZLibSize = br.ReadInt32();
            int DecompressedSize = br.ReadInt32();

            MemoryStream fileTable = new();
            br.ReadInt16();
            using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(ZLibSize - 2)), CompressionMode.Decompress))
                ds.CopyTo(fileTable);
            br.Close();

            br = new(fileTable);
            br.BaseStream.Position = 0;
            System.Collections.Generic.List<FileTableEntry> table = new();
            while (br.BaseStream.Position < br.BaseStream.Length)
                table.Add(new());
            br.Close();

            br = new(fileData);
            foreach(FileTableEntry file in table)
            {
                br.BaseStream.Position = file.end - file.Size;
                Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + Path.GetDirectoryName(file.name));
                using FileStream FS = File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + file.name);
                BinaryWriter bw = new(FS);
                bw.Write(br.ReadBytes(file.Size));
                bw.Close();
            }
            br.Close();
        }

        class FileTableEntry
        {
            public readonly string name = NullTerminatedString();
            public readonly int Size = br.ReadInt32();
            public readonly int end = br.ReadInt32();
            readonly int unknown1 = br.ReadInt32();
            readonly int unknown2 = br.ReadInt32();
            readonly int unknown3 = br.ReadInt32();
        }

        public static string NullTerminatedString()
        {
            char[] fileName = Array.Empty<char>();
            char readchar = (char)1;
            while (readchar > 0)
            {
                readchar = br.ReadChar();
                Array.Resize(ref fileName, fileName.Length + 1);
                fileName[^1] = readchar;
            }
            Array.Resize(ref fileName, fileName.Length - 1);
            string name = new(fileName);
            return name;
        }
    }
}
