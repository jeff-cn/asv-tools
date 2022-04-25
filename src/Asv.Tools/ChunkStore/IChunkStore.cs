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
        SessionMetadata Current { get; set; }
        SessionMetadata Start(SessionSettings settings, IEnumerable<SessionFieldSettings> records);
        uint Append(uint fieldId, FieldWriteCallback writeWriteCallback);
        void Stop();
        IEnumerable<Guid> GetSessions();
        SessionInfo GetSessionInfo(Guid sessionId);
        IEnumerable<uint> GetFieldsIds(Guid sessionId);
        SessionFieldInfo GetFieldInfo(Guid sessionId, uint recordId);
        void ReadRecord(Guid sessionId, uint recordId, uint index, FieldReadCallback readCallback);

    }
}
