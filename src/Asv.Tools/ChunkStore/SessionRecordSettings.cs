using System;
using System.Text.RegularExpressions;

namespace Asv.Tools
{
    public class SessionRecordSettings: ISizedSpanSerializable,IEquatable<SessionRecordSettings>
    {
        

        private string _name;

        internal SessionRecordSettings()
        {

        }

        public SessionRecordSettings(ushort id,string name,ushort offset)
        {
            if (offset <= 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            Id = id;
            Name = name;
            Offset = offset;
        }

        public ushort Id { get; set; }

        public string Name
        {
            get => _name;
            set => ChunkStoreHelper.CheckAndSetName(ref _name,value);
        }

        public ushort Offset { get; set; }

        public virtual void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Id = BinSerialize.ReadUShort(ref buffer);
            Name = BinSerialize.ReadString(ref buffer);
            Offset = BinSerialize.ReadUShort(ref buffer);
        }

        public virtual void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteUShort(ref buffer, Id);
            BinSerialize.WriteString(ref buffer, Name);
            BinSerialize.WriteUShort(ref buffer, Offset);
        }

        public virtual int GetByteSize()
        {
            return sizeof(ushort) * 2 + BinSerialize.GetSizeForString(Name);
        }

        public bool Equals(SessionRecordSettings other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _name == other._name && Id == other.Id && Offset == other.Offset;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SessionRecordSettings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Offset.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SessionRecordSettings left, SessionRecordSettings right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SessionRecordSettings left, SessionRecordSettings right)
        {
            return !Equals(left, right);
        }
    }
}
