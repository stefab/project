using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace EtsyRobot.Storage.Infrastructure
{
	internal class FileMutex : IDisposable
	{
		private FileMutex(FileStream stream)
		{
			this._stream = stream;
		}

		#region IDisposable Members
		public void Dispose()
		{
			if (this._stream != null)
			{
				this._stream.Dispose();
			}
		}
		#endregion

		static public FileMutex Acquire(string fileName, TimeSpan maxDuration)
		{
			_tracer.TraceEvent(TraceEventType.Verbose, 0, "Acquiring a lock for {0}...", fileName);
			TimeSpan duration = TimeSpan.FromSeconds(0);
			while (duration <= maxDuration)
			{
				try
				{
					var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 100,
					                            FileOptions.DeleteOnClose);

					_tracer.TraceEvent(TraceEventType.Verbose, 0, "Lock for {0} acquired successfully.", fileName);

					return new FileMutex(stream);
				}
				catch (IOException)
				{
					Thread.Sleep(1000);
					duration = duration.Add(TimeSpan.FromSeconds(1));

					_tracer.TraceEvent(TraceEventType.Verbose, 0, "Lock for {0} has not been acquired in {1} seconds",
					                   fileName, duration.Seconds);
				}
			}

		throw new TimeoutException(string.Format("Cannot acquire a lock for {0}.", fileName));
		}

		static private readonly TraceSource _tracer = new TraceSource("Storage.Infrastructure", SourceLevels.All);

		private readonly FileStream _stream;
	}
}