using System;
using System.Collections.Generic;

namespace Asv.Tools
{
    public class SessionSettings:IEqualityComparer<SessionSettings>
    {
        private string _name;

        public SessionSettings()
        {
            
        }

        public SessionSettings(string name, params string[] tags)
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            Tags = new HashSet<string>(tags);
            foreach (var tag in tags)
            {
                ChunkStoreHelper.CheckName(tag);
            }
            Name = name;
        }

        public string Name
        {
            get => _name;
            set => ChunkStoreHelper.CheckAndSetName(ref _name, value);
        }

        public HashSet<string> Tags { get; set; }

        public bool Equals(SessionSettings x, SessionSettings y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name && Equals(x.Tags, y.Tags);
        }

        public int GetHashCode(SessionSettings obj)
        {
            unchecked
            {
                return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ (obj.Tags != null ? obj.Tags.GetHashCode() : 0);
            }
        }
    }
}
