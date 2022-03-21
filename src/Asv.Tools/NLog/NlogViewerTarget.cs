using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NLog.Common;
using NLog.Targets;

namespace Asv.Tools.NLog
{
    [Target("NlogViewerTarget")]
    public class NlogViewerTarget : Target, IObservable<AsyncLogEventInfo>
    {
        readonly Subject<AsyncLogEventInfo> _subject = new Subject<AsyncLogEventInfo>();

        public NlogViewerTarget()
        {

        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            base.Write(logEvent);
            _subject.OnNext(logEvent);
        }

        public IDisposable Subscribe(IObserver<AsyncLogEventInfo> observer)
        {
            return _subject.ObserveOn(Scheduler.Default).Subscribe(observer);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _subject.OnCompleted();
            _subject.Dispose();
        }
    }
}
