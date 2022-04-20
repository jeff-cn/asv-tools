using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Asv.Tools.ChunkStoreHelper;

namespace Asv.Tools
{
    public static class ChunkStoreHelper
    {
        public const string NameRegexString = "^[A-Za-z][A-Za-z0-9_]{3,29}$";
        public static readonly Regex NameRegex = new(NameRegexString, RegexOptions.Compiled);

        public static void CheckAndSetName(ref string name, string value)
        {
            CheckName(value);
            name = value;
        }

        public static void CheckName(string name)
        {
            if (NameRegex.IsMatch(name) == false)
                throw new ArgumentException(
                    $"Param name '{name}' not match regex '{NameRegexString}')");
        }
    }

    public interface IChunkStore:IDisposable
    {
        bool IsStarted { get; }
        SessionInfo Current { get; set; }
        SessionInfo Start(SessionSettings settings, IEnumerable<SessionRecordSettings> records);
        void Append(ushort id, RecordCallback writeCallback);
        void Stop();
        IEnumerable<Guid> GetSessions();
        SessionInfo ReadMetadata(Guid sessionId);
        SessionRecordMetadata ReadRecordMetadata(Guid sessionId, ushort recordId);
        void ReadRecord(Guid sessionId, ushort recordId, uint index, RecordReadCallback readCallback);

    }
}
