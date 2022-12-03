// namespace ResourceTypes.Misc;
//
// public class StreamMapLoader
// {
//     public FileInfo file { get; set; }
//     public StreamGroup[] groups { get; set; }
//     public string[] groupHeaders { get; set; }
//     public StreamLine[] lines { get; set; }
//     public StreamLoader[] loaders { get; set; }
//     public StreamBlock[] blocks { get; set; }
//     public ulong[] hashes { get; set; }
//
//     //Only used after the StreamMap has been updated.
//     private ulong[] upGroupHeaders { get; set; }
//     private string rawPool { get; set; }
//
//     public StreamMapLoader(FileInfo info)
//     {
//         file = info;
//         using (BinaryReader reader = new BinaryReader(File.Open(info.FullName, FileMode.Open)))
//         {
//             ReadFromFile(reader);
//         }
//     }
//     
//     public static StreamMapLoader ReadFromFile(BinaryReader reader)
//     {
//         if (reader.ReadInt32() != 1299346515)
//         {
//             throw new InvalidDataException(); // TODO add message
//         }
//
//         if (reader.ReadInt32() != 0x6)
//         {
//             throw new InvalidDataException(); // TODO add message
//         }
//
//         int fileSize = reader.ReadInt32();
//         int unk0 = reader.ReadInt32();
//
//         int numGroups = reader.ReadInt32();
//         int groupOffset = reader.ReadInt32();
//         int numHeaders = reader.ReadInt32();
//         int headerOffset = reader.ReadInt32();
//         int numLines = reader.ReadInt32();
//         int lineOffset = reader.ReadInt32();
//         int numLoaders = reader.ReadInt32();
//         int loadersOffset = reader.ReadInt32();
//         int numBlocks = reader.ReadInt32();
//         int blockOffset = reader.ReadInt32();
//         int numHashes = reader.ReadInt32();
//         int hashOffset = reader.ReadInt32();
//         int poolSize = reader.ReadInt32();
//         int poolOffset = reader.ReadInt32();
//
//         if (reader.BaseStream.Position == groupOffset)
//         {
//             throw new InvalidDataException("Failed to reach the starting offset for group declaration.");
//         }
//
//         var groups = new StreamGroup[numGroups];
//         
//         for (int i = 0; i < groups.Length; i++)
//         {
//             StreamGroup map = new StreamGroup();
//             map.nameIDX = reader.ReadInt32();
//             map.Name = ReadFromBuffer((long)((ulong)map.nameIDX + (ulong)poolOffset), reader.BaseStream.Position, reader);
//             map.Type = (GroupTypes)reader.ReadInt32();
//             map.Unk01 = reader.ReadInt32();
//             map.startOffset = reader.ReadInt32();
//             map.endOffset = reader.ReadInt32();
//             map.unk5 = reader.ReadInt32();
//             groups[i] = map;
//         }
//
//         ToolkitAssert.Ensure(reader.BaseStream.Position == headerOffset, "Did not reach the header starting offset");
//
//         groupHeaders = new string[numHeaders];
//         ulong[] ulongHeaders = new ulong[numHeaders];
//
//         for (int i = 0; i < numHeaders; i++)
//         {
//             ulongHeaders[i] = reader.ReadUInt64();
//             groupHeaders[i] = ReadFromBuffer((long)(ulongHeaders[i] + (ulong)poolOffset), reader.BaseStream.Position, reader);
//         }
//
//         ToolkitAssert.Ensure(reader.BaseStream.Position == lineOffset, "Did not reach the line data starting offset!");
//
//         lines = new StreamLine[numLines];
//
//         for (int i = 0; i < numLines; i++)
//         {
//             StreamLine map = new StreamLine();
//             map.nameIDX = reader.ReadInt32();
//             map.lineID = reader.ReadInt32();
//             map.groupID = reader.ReadInt32();
//             map.LoadType = reader.ReadInt32();
//             map.flagIDX = reader.ReadInt32();
//             map.Unk5 = reader.ReadInt32();
//             map.Unk10 = reader.ReadUInt64();
//             map.Unk11 = reader.ReadUInt64();
//             map.Unk12 = reader.ReadInt32();
//             map.Unk13 = reader.ReadInt32();
//             map.Unk14 = reader.ReadInt32();
//             map.Unk15 = reader.ReadInt32();
//             map.Group = groupHeaders[map.groupID];
//             map.Name = ReadFromBuffer((long)((ulong)map.nameIDX + (ulong)poolOffset), reader.BaseStream.Position, reader);
//             map.Flags = ReadBufferSpecial((long)((ulong)map.flagIDX + (ulong)poolOffset), reader.BaseStream.Position, reader).TrimEnd('\0').Replace('\0', '|');
//             lines[i] = map;
//         }
//
//         ToolkitAssert.Ensure(reader.BaseStream.Position == loadersOffset, "Did not reach the loader data starting offset!");
//
//         loaders = new StreamLoader[numLoaders];
//
//         for (int i = 0; i < numLoaders; i++)
//         {
//             StreamLoader map = new StreamLoader();
//             map.start = reader.ReadInt32();
//             map.end = reader.ReadInt32();
//             map.Type = (GroupTypes)reader.ReadInt32();
//                 
//             map.LoaderSubID = reader.ReadInt32();
//             map.LoaderID = reader.ReadInt32();
//             map.LoadType = reader.ReadInt32();
//             map.pathIDX = reader.ReadInt32();
//             map.entityIDX = reader.ReadInt32();
//             map.Path = ReadFromBuffer((long)((ulong)map.pathIDX + (ulong)poolOffset), reader.BaseStream.Position, reader);
//             map.Entity = ReadBufferSpecial((long)((ulong)map.entityIDX + (ulong)poolOffset), reader.BaseStream.Position, reader).TrimEnd('\0').Replace('\0', '|');
//             loaders[i] = map;
//         }
//
//         ToolkitAssert.Ensure(reader.BaseStream.Position == blockOffset, "Did not reach the block declaration starting offset!");
//
//         blocks = new StreamBlock[numBlocks];
//         for (int i = 0; i < numBlocks; i++)
//         {
//             StreamBlock map = new StreamBlock();
//             map.startOffset = reader.ReadInt32();
//             map.endOffset = reader.ReadInt32();
//             map.Hashes = new ulong[map.endOffset - map.startOffset];
//             blocks[i] = map;
//         }
//
//         ToolkitAssert.Ensure(reader.BaseStream.Position == hashOffset, "Did not reach the block hashes starting offset!");
//
//         hashes = new ulong[numHashes];
//
//         for (int i = 0; i < numHashes; i++)
//         {
//             hashes[i] = reader.ReadUInt64();                                                                                              
//         }
//
//         for(int i = 0; i < numBlocks; i++)
//         {
//             var block = blocks[i];
//             Array.Copy(hashes, block.startOffset, block.Hashes, 0, block.Hashes.Length);
//         }
//
//         ToolkitAssert.Ensure(reader.BaseStream.Position == poolOffset, "Did not reach the buffer pool starting offset!");
//
//         reader.BaseStream.Seek(poolSize, SeekOrigin.Current);
//
//         ToolkitAssert.Ensure(reader.BaseStream.Position == reader.BaseStream.Length, "Did not reach the end of the file!");
//     }
//
//     private string ReadBufferSpecial(long start, long end, BinaryReader reader)
//     {
//         reader.BaseStream.Position = start;
//         string newString = "";
//         int nHitTrail = 0;
//
//         while (nHitTrail != 2)
//         {
//             if (reader.PeekChar() == '\0' && nHitTrail < 2)
//             {
//                 newString += reader.ReadChar();
//                 nHitTrail++;
//             }
//             else if (reader.PeekChar() == '\0')
//             {
//                 nHitTrail++;
//                 reader.ReadByte();
//             }
//             else
//             {
//                 newString += reader.ReadChar();
//                 nHitTrail = 0;
//             }
//         }
//         reader.BaseStream.Position = end;
//         return newString;
//     }
//
//     private string ReadFromBuffer(long start, long end, BinaryReader reader)
//     {
//         reader.BaseStream.Position = start;
//         string result = StringHelpers.ReadString(reader);
//         reader.BaseStream.Position = end;
//         return result;
//     }
//
//     private void Update()
//     {
//         Dictionary<string, int> pool = new Dictionary<string, int>();
//         int size = 0;
//         rawPool = "";
//
//         int loaderIDX = 0;
//         foreach (var group in groups)
//         {
//             int idx = -1;
//             if (!pool.TryGetValue(group.Name, out idx))
//             {
//                 idx = size;
//                 pool.Add(group.Name, size);
//                 size += group.Name.Length + 2;
//                 rawPool += (group.Name + '\0' + '\0');
//             }
//
//             group.nameIDX = idx;
//             for (int x = group.startOffset; x < group.startOffset + group.endOffset; x++)
//             {
//                 loaderIDX++;
//                 var loader = loaders[x];
//                 if (loader.LoaderSubID == 1)
//                 {
//                     loader.LoaderID = x != 0 ? loaders[x - 1].LoaderID : 1;
//                 }
//                 else
//                 {
//                     loader.LoaderID = loaderIDX;
//                 }
//
//                 idx = -1;
//                 if (!pool.TryGetValue(loader.Path, out idx))
//                 {
//                     idx = size;
//                     pool.Add(loader.Path, size);
//                     size += loader.Path.Length + 2;
//                     rawPool += (loader.Path + '\0' + '\0');
//                 }
//                 loader.pathIDX = idx;
//                 if (!pool.TryGetValue(loader.Entity, out idx))
//                 {
//                     idx = size;
//                     pool.Add(loader.Entity, size);
//                     string[] splits = loader.Entity.Split('|');
//                     string entity = loader.Entity.Replace('|', '\0');
//
//                     rawPool += (entity + '\0' + '\0');
//                     size += loader.Entity.Length + 2;
//                     if (splits.Length > 5)
//                     {
//                         rawPool += ('\0');
//                         size++;
//                     }
//                 }
//                 loader.entityIDX = idx;
//
//             }
//         }
//
//         List<string> newGH = new List<string>();
//         List<ulong> hashGH = new List<ulong>();
//         string groupName = "";
//         foreach (var line in lines)
//         {
//             int idx = -1;
//
//             if (groupName != line.Group)
//             {
//                 groupName = line.Group;
//                 if (!pool.TryGetValue(line.Group, out idx))
//                 {
//                     pool.Add(line.Group, size);
//                     line.groupID = newGH.Count;
//                     newGH.Add(line.Group);
//                     hashGH.Add((ulong)size);
//                     size += line.Group.Length + 2;
//                     rawPool += (line.Group + '\0' + '\0');
//                 }
//                 else
//                 {
//                     line.groupID = newGH.Count;
//                     newGH.Add(line.Group);
//                     hashGH.Add((ulong)idx);
//                 }
//             }
//             else
//             {
//                 line.groupID = newGH.Count - 1;
//             }
//             if (!pool.TryGetValue(line.Name, out idx))
//             {
//                 idx = size;
//                 pool.Add(line.Name, size);
//                 size += line.Name.Length + 2;
//                 rawPool += (line.Name + '\0' + '\0');
//             }
//             line.nameIDX = idx;
//             if (!pool.TryGetValue(line.Flags, out idx))
//             {
//                 idx = size;
//                 pool.Add(line.Flags, size);
//                 size += line.Flags.Length + 3;
//                 string flags = line.Flags.Replace('|', '\0');
//                 rawPool += (flags + '\0' + '\0' + '\0');
//             }
//             line.flagIDX = idx;
//         }
//         groupHeaders = newGH.ToArray();
//         upGroupHeaders = hashGH.ToArray();
//     }
//
//     public void WriteToFile()
//     {
//         Update();
//         var oldString = file.FullName.Remove(file.FullName.Length - 4, 4);
//         oldString += "_old.bin";
//         File.Copy(file.FullName, oldString, true);
//         using (BinaryWriter writer = new BinaryWriter(File.Open(file.FullName, FileMode.Create)))
//         {
//             InternalWriteToFile(writer);
//         }
//     }
//     private void InternalWriteToFile(BinaryWriter writer)
//     {
//         int groupOffset = 0;
//         int headerOffset = 0;
//         int lineOffset = 0;
//         int loadersOffset = 0;
//         int blockOffset = 0;
//         int hashOffset = 0;
//         int poolOffset = 0;
//
//         writer.Write(new byte[72]);
//
//         groupOffset = 72;
//
//         foreach (var group in groups)
//         {
//             writer.Write(group.nameIDX);
//             writer.Write((int)group.Type);
//             writer.Write(group.Unk01);
//             writer.Write(group.startOffset);
//             writer.Write(group.endOffset);
//             writer.Write(group.unk5);
//         }
//
//         headerOffset = (int)writer.BaseStream.Position;
//
//         foreach (var value in upGroupHeaders)
//         {
//             writer.Write(value);
//         }
//
//         lineOffset = (int)writer.BaseStream.Position;
//
//         foreach (var line in lines)
//         {
//             writer.Write(line.nameIDX);
//             writer.Write(line.lineID);
//             writer.Write(line.groupID);
//             writer.Write(line.LoadType);
//             writer.Write(line.flagIDX);
//             writer.Write(line.Unk5);
//             writer.Write(line.Unk10);
//             writer.Write(line.Unk11);
//             writer.Write(line.Unk12);
//             writer.Write(line.Unk13);
//             writer.Write(line.Unk14);
//             writer.Write(line.Unk15);
//         }
//
//         loadersOffset = (int)writer.BaseStream.Position;
//
//         foreach (var loader in loaders)
//         {
//             writer.Write(loader.start);
//             writer.Write(loader.end);
//             writer.Write((int)loader.Type);
//             writer.Write(loader.LoaderSubID);
//             writer.Write(loader.LoaderID);
//             writer.Write(loader.LoadType);
//             writer.Write(loader.pathIDX);
//             writer.Write(loader.entityIDX);
//         }
//
//         blockOffset = (int)writer.BaseStream.Position;
//
//         foreach (var block in blocks)
//         {
//             writer.Write(block.startOffset);
//             writer.Write(block.endOffset);
//         }
//
//         hashOffset = (int)writer.BaseStream.Position;
//
//         foreach (var value in hashes)
//         {
//             writer.Write(value);
//         }
//
//         poolOffset = (int)writer.BaseStream.Position;
//
//         writer.Write(rawPool.ToCharArray());
//
//         writer.BaseStream.Seek(0, SeekOrigin.Begin);
//         writer.Write(1299346515);
//         writer.Write(0x6);
//         writer.Write((uint)writer.BaseStream.Length);
//         writer.Write(0);
//         writer.Write(groups.Length);
//         writer.Write(groupOffset);
//         writer.Write(groupHeaders.Length);
//         writer.Write(headerOffset);
//         writer.Write(lines.Length);
//         writer.Write(lineOffset);
//         writer.Write(loaders.Length);
//         writer.Write(loadersOffset);
//         writer.Write(blocks.Length);
//         writer.Write(blockOffset);
//         writer.Write(hashes.Length);
//         writer.Write(hashOffset);
//         writer.Write(rawPool.Length);
//         writer.Write(poolOffset);
//     }
// }