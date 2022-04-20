using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace Asv.Tools
{
    public delegate void RecordCallback(ref Span<byte> data);

    public delegate void RecordReadCallback(ref ReadOnlySpan<byte> data);

    public class ChunkFileStore: IChunkStore
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string MetadataFileName = "_info.json";
        private readonly string _rootFolder;
        private readonly object _sync = new();
        private string _recordFolderPath;
        private readonly Dictionary<uint, (SessionRecordMetadata, FileStream)> _files = new();

        public ChunkFileStore(string rootFolder)
        {
            _rootFolder = rootFolder;
            if (Directory.Exists(rootFolder) == false)
            {
                Directory.CreateDirectory(_rootFolder);
            }
        }

        public SessionInfo Start(SessionSettings settings,IEnumerable<SessionRecordSettings> records)
        {
            CheckNotStarted();
            lock (_sync)
            {
                CheckNotStarted();
                var id = Guid.NewGuid();
                var metadata = new SessionInfo()
                {
                    Id = id,
                    Name = settings.Name,
                    Tags = settings.Tags,
                };
                _recordFolderPath = GetSessionFolderName(id);
                if (Directory.Exists(_recordFolderPath))
                {
                    Logger.Warn($"Directory for new recording already exist {_recordFolderPath}. Remove all data.");
                    Directory.Delete(_recordFolderPath);
                }
                Directory.CreateDirectory(_recordFolderPath);
                foreach (var rec in records)
                {
                    var recordMetadata = new SessionRecordMetadata
                    {
                        Id = rec.Id,
                        Offset = rec.Offset,
                        Name = rec.Name,
                    };
                    var file = File.OpenWrite(GetRecordFileName(id, rec.Id));
                    var metadataArr = ArrayPool<byte>.Shared.Rent(SessionRecordMetadata.MetadataFileOffset);
                    var span = new Span<byte>(metadataArr,0, SessionRecordMetadata.MetadataFileOffset);
                    try
                    {
                        recordMetadata.Serialize(ref span);
                        if (span.IsEmpty == false)
                        {
                            for (int i = 0; i < span.Length; i++)
                            {
                                span[i] = 0;
                            }
                        }
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(metadataArr);
                    }
                    file.Write(metadataArr,0, SessionRecordMetadata.MetadataFileOffset);
                    _files.Add(rec.Id, (recordMetadata, file));
                }
                File.WriteAllText(Path.Combine(_recordFolderPath,MetadataFileName), JsonConvert.SerializeObject(metadata));
                IsStarted = true;
                return Current = metadata;
            }
        }

        public void Stop()
        {
            if (IsStarted == false) return;
            lock (_sync)
            {
                if (IsStarted == false) return;
                IsStarted = false;
                Current = null;
                foreach (var item in _files.Values)
                {
                    item.Item2.Flush();
                    item.Item2.Dispose();
                }
                _files.Clear();
            }
        }

        public IEnumerable<Guid> GetSessions()
        {
            foreach (var dir in Directory.EnumerateDirectories(_rootFolder))
            {
                var path = Path.GetFileName(dir);
                if (Guid.TryParse(path, out var guid))
                {
                    yield return guid;
                }
            }
        }
        public SessionInfo ReadMetadata(Guid sessionId)
        {
            var metadataFile = GetMetadataFileName(sessionId);
            return File.Exists(metadataFile) ? JsonConvert.DeserializeObject<SessionInfo>(File.ReadAllText(metadataFile)): null;
        }


        public SessionRecordMetadata ReadRecordMetadata(Guid sessionId, ushort recordId)
        {
            CheckNotStarted();
            lock (_sync)
            {
                CheckNotStarted();
                using var file = File.OpenRead(GetRecordFileName(sessionId, recordId));
                return InternalReadRecordMetadata(file);
            }
        }

        private SessionRecordMetadata InternalReadRecordMetadata(FileStream file)
        {
            var recordMetadata = new SessionRecordMetadata();
            var recordMetadataArray = ArrayPool<byte>.Shared.Rent(SessionRecordMetadata.MetadataFileOffset);
            try
            {
                file.Position = 0;
                var read = file.Read(recordMetadataArray, 0, SessionRecordMetadata.MetadataFileOffset);
                if (read != SessionRecordMetadata.MetadataFileOffset)
                {
                    throw new Exception($"Error to read record {file.Name} metadata");
                }

                var recordMetadataSpan = new ReadOnlySpan<byte>(recordMetadataArray, 0, SessionRecordMetadata.MetadataFileOffset);
                recordMetadata.Deserialize(ref recordMetadataSpan);
                return recordMetadata;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(recordMetadataArray);
            }
        }


        public bool IsStarted { get; private set; }
        public SessionInfo Current { get; set; }

        public void Append(ushort id, RecordCallback writeCallback)
        {
            CheckIsStarted();
            lock (_sync)
            {
                CheckIsStarted();

                var file = _files[id];
                var data = ArrayPool<byte>.Shared.Rent(file.Item1.Offset);
                try
                {
                    var span = new Span<byte>(data, 0, file.Item1.Offset);
                    writeCallback(ref span);
                    if (span.IsEmpty == false)
                    {
                        for (var i = 0; i < span.Length; i++)
                        {
                            span[i] = 0;
                        }
                    }
                    file.Item2.Position = file.Item2.Length;
                    file.Item2.Write(data,0, file.Item1.Offset);
                    file.Item2.Flush();
                }
                catch (Exception e)
                {
                    Logger.Error($"Error to append record {id}:{e.Message}");
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
            }
        }

        private void CheckIsStarted()
        {
            if (IsStarted == false)
            {
                throw new Exception("Record not started");
            }
        }

        private void CheckNotStarted()
        {
            if (IsStarted == true)
            {
                throw new Exception("Record is started");
            }
        }


        public void ReadRecord(Guid sessionId, ushort recordId, uint index, RecordReadCallback readCallback)
        {
            CheckNotStarted();
            lock (_sync)
            {
                CheckNotStarted();
                
                using var file = File.OpenRead(GetRecordFileName(sessionId, recordId));
                var recordMetadata = InternalReadRecordMetadata(file);
                var recordDataArray = ArrayPool<byte>.Shared.Rent(recordMetadata.Offset);
                try
                {
                    file.Position = recordMetadata.Offset * index + SessionRecordMetadata.MetadataFileOffset;
                    var read = file.Read(recordDataArray, 0, recordMetadata.Offset);
                    if (read != recordMetadata.Offset)
                    {
                        throw new Exception($"Error to read record {recordId} data");
                    }
                    var recordDataSpan = new ReadOnlySpan<byte>(recordDataArray, 0, SessionRecordMetadata.MetadataFileOffset);
                    readCallback(ref recordDataSpan);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(recordDataArray);
                }
            }
        }

        private string GetMetadataFileName(Guid sessionId)
        {
            return Path.Combine(GetSessionFolderName(sessionId), MetadataFileName);
        }

        private string GetSessionFolderName(Guid id)
        {
            return Path.Combine(_rootFolder, id.ToString());
        }

        private string GetRecordFileName(Guid sessionId,uint id)
        {
            return Path.Combine(GetSessionFolderName(sessionId), $"{id}.rtt");
        }

        public void Dispose()
        {
            Stop();
        }
    }

   
}
