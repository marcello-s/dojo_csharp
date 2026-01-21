#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer
{
    class StoppedEventData(Exception error = null!) : EventArgs
    {
        public Exception Error { get; private set; } = error;
    }
}
