#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataDataCompression;

class Lz77Compressor : ICompressor
{
    private const byte BufferSize = byte.MaxValue;
    private readonly byte[] ringbufferWindow = new byte[BufferSize];
    private byte windowPointer;
    private bool isRingbufferOverflow;
    private readonly byte[] bufferLookahead = new byte[BufferSize];
    private byte lookaheadPointer;

    private readonly byte[] dataBuffer = new byte[1];
    private readonly byte[] encodingBuffer = new byte[3];

    public void Compress(Stream input, Stream output)
    {
        byte value = 0;
        byte position = 0;
        byte matchPosition = 0;
        var hasMatch = false;
        var isEncoding = true;

        if (!TryReadStream(input, out value))
        {
            return;
        }

        AddToLookahead(value);
        while (isEncoding)
        {
            hasMatch = TryFindLongestMatch(out position);
            if (hasMatch)
            {
                matchPosition = position;
                while (true)
                {
                    if (!TryReadStream(input, out value))
                    {
                        isEncoding = false;
                        break;
                    }

                    AddToLookahead(value);
                    hasMatch = TryFindLongestMatch(out position);
                    if (!hasMatch)
                    {
                        var location = (byte)(windowPointer - matchPosition);
                        var length = (byte)(lookaheadPointer - 1);
                        EncodePointer(output, location, length);

                        for (var i = 0; i < lookaheadPointer - 1; i++)
                        {
                            AddToWindow(bufferLookahead[i]);
                        }

                        var peek = bufferLookahead[lookaheadPointer - 1];
                        ResetLookahead();
                        AddToLookahead(peek);

                        break;
                    }

                    matchPosition = position;
                }
            }
            else
            {
                EncodeNullPointer(output, value);
                AddToWindow(value);
                ResetLookahead();
                if (!TryReadStream(input, out value))
                {
                    isEncoding = false;
                }

                AddToLookahead(value);
            }
        }

        if (hasMatch)
        {
            var location = (byte)(windowPointer - matchPosition);
            EncodePointer(output, location, lookaheadPointer);
        }
    }

    public void Decompress(Stream input, Stream output) { }

    public void EncodeNullPointer(Stream output, byte value)
    {
        encodingBuffer[0] = 0;
        encodingBuffer[1] = 0;
        encodingBuffer[2] = value;
        output.Write(encodingBuffer, 0, 3);
    }

    public void EncodePointer(Stream output, byte location, byte length)
    {
        encodingBuffer[0] = location;
        encodingBuffer[1] = length;
        output.Write(encodingBuffer, 0, 2);
    }

    public bool TryReadStream(Stream input, out byte data)
    {
        data = 0;
        var hasRead = false;
        int read = 0;
        read = input.Read(dataBuffer, 0, 1);
        hasRead = read > 0;

        if (hasRead)
        {
            data = dataBuffer[0];
        }

        return hasRead;
    }

    public void AddToWindow(byte value)
    {
        ringbufferWindow[windowPointer] = value;
        AdvanceWindowPointer();
    }

    public void AdvanceWindowPointer()
    {
        var currentPointer = windowPointer;
        windowPointer++;
        isRingbufferOverflow |= (windowPointer < currentPointer);
    }

    public void AddToLookahead(byte value)
    {
        bufferLookahead[lookaheadPointer] = value;
        AdvanceLookaheadPointer();
    }

    public void AdvanceLookaheadPointer()
    {
        lookaheadPointer++;
    }

    public void ResetLookahead()
    {
        lookaheadPointer = 0;
    }

    public bool TryFindLongestMatch(out byte position)
    {
        position = 0;
        bool result = false;
        byte bufferCount = 0;
        byte matchPosition = 0;
        byte ringbufferWindowLength = isRingbufferOverflow ? BufferSize : windowPointer;
        byte bufferPosition = ringbufferWindowLength;
        bufferPosition--;

        // return if lookahead pointer is not set
        if (lookaheadPointer == 0)
        {
            return result;
        }

        // 1. scan buffer for bufferLookahead[0] backwards
        // 2. hold buffer position, advance buffer and bufferLookahead by one
        // 3. if the there is a whole match then return the buffer position
        // 4. if there is no match advance buffer position by one and repeat from step 1

        while (bufferCount < ringbufferWindowLength && !isRingbufferOverflow)
        {
            // scan
            while (
                bufferPosition < BufferSize
                && ringbufferWindow[bufferPosition] != bufferLookahead[0]
            )
            {
                bufferPosition--;
            }

            // abort if the scan did not find anything
            if (bufferPosition == BufferSize)
            {
                break;
            }

            // hold buffer position
            position = bufferPosition;
            while (
                ringbufferWindow[bufferPosition] == bufferLookahead[matchPosition]
                && matchPosition < lookaheadPointer
            )
            {
                bufferPosition++;
                matchPosition++;
            }

            // did the whole match pass?
            if (matchPosition == lookaheadPointer)
            {
                result = true;
                break;
            }
            else
            {
                matchPosition = 0;
                bufferPosition = position;
                bufferPosition--;
            }

            bufferCount++;
        }

        return result;
    }
}
