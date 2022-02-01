using System;
using System.Collections.Concurrent;

namespace Asv.Tools
{
    public class IntegerDiagnostic : IDigitDiagnostic<int>
    {
        private readonly ConcurrentDictionary<DiagnosticKey, DiagnosticItem> _values;

        public IntegerDiagnostic(string groupName, ConcurrentDictionary<DiagnosticKey, DiagnosticItem> values)
        {
            _values = values;
            GroupName = groupName;
        }

        public string GroupName { get; }

        public void Update(string name, Func<int, int> updateCallback)
        {
            _values.AddOrUpdate(new DiagnosticKey(GroupName, name), new DiagnosticItem
            {
                LastUpdate = DateTime.Now,
                FormatString = null,
                IntValue = updateCallback(0),
                ItemType = DiagnosticItemType.Integer,
            }, (diagnosticKey, s) =>
            {
                s.IntValue = updateCallback(s.IntValue);
                s.LastUpdate = DateTime.Now;
                return s;
            });
        }

        public void Increment(string name, int value)
        {
            throw new NotImplementedException();
        }

        public int this[string name]
        {
            get => Get(name)?.IntValue ?? 0;
            set => _values.AddOrUpdate(new DiagnosticKey(GroupName, name), new DiagnosticItem
            {
                LastUpdate = DateTime.Now,
                FormatString = null,
                IntValue = value,
                ItemType = DiagnosticItemType.Integer,
            }, (diagnosticKey, s) =>
            {
                s.IntValue = value;
                s.LastUpdate = DateTime.Now;
                return s;
            });
        }

        private DiagnosticItem Get(string name)
        {
            return _values.TryGetValue(new DiagnosticKey(GroupName, name), out var item) ? item : null;
        }

        public int this[string name, string format]
        {
            set => _values.AddOrUpdate(new DiagnosticKey(GroupName, name), new DiagnosticItem
            {
                LastUpdate = DateTime.Now,
                FormatString = format,
                IntValue = value,
                ItemType = DiagnosticItemType.Integer,
            }, (diagnosticKey, s) =>
            {
                s.IntValue = value;
                s.LastUpdate = DateTime.Now;
                return s;
            });
        }

        public int this[string name, string format, TimeSpan lifeTime]
        {
            set => _values.AddOrUpdate(new DiagnosticKey(GroupName, name), new DiagnosticItem
            {
                LastUpdate = DateTime.Now,
                FormatString = format,
                IntValue = value,
                ItemType = DiagnosticItemType.Integer,
                LifeTime = lifeTime,
            }, (diagnosticKey, s) =>
            {
                s.IntValue = value;
                s.LastUpdate = DateTime.Now;
                return s;
            });
        }

        public int this[string name, TimeSpan lifeTime]
        {
            set => _values.AddOrUpdate(new DiagnosticKey(GroupName, name), new DiagnosticItem
            {
                LastUpdate = DateTime.Now,
                FormatString = null,
                IntValue = value,
                ItemType = DiagnosticItemType.Integer,
                LifeTime = lifeTime,
            }, (diagnosticKey, s) =>
            {
                s.IntValue = value;
                s.LastUpdate = DateTime.Now;
                return s;
            });
        }

        public void Dispose()
        {

        }
    }
}
