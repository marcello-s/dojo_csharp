#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataDataCompression;

class RleCompressor : ICompressor
{
    private readonly byte[] dataBuffer = new byte[1];
    private readonly byte[] encodingBuffer = new byte[2];
    private byte value;
    private byte count;

    public void Compress(Stream input, Stream output)
    {
        value = 0;
        count = 1;
        int read = 0;

        read = input.Read(dataBuffer, 0, 1);
        value = dataBuffer[0];

        while ((read = input.Read(dataBuffer, 0, dataBuffer.Length)) > 0)
        {
            if (dataBuffer[0] == value)
            {
                count++;

                if (count == 0) // use byte overflow
                {
                    encodingBuffer[0] = byte.MaxValue;
                    encodingBuffer[1] = value;
                    output.Write(encodingBuffer, 0, encodingBuffer.Length);
                    count = 1;
                }
            }
            else
            {
                encodingBuffer[0] = count;
                encodingBuffer[1] = value;
                output.Write(encodingBuffer, 0, encodingBuffer.Length);
                count = 1;
            }

            value = dataBuffer[0];
        }

        encodingBuffer[0] = count;
        encodingBuffer[1] = value;
        output.Write(encodingBuffer, 0, encodingBuffer.Length);
    }

    public void Decompress(Stream input, Stream output)
    {
        int read = 0;
        while ((read = input.Read(encodingBuffer, 0, encodingBuffer.Length)) > 0)
        {
            count = encodingBuffer[0];
            value = encodingBuffer[1];

            for (var i = 0; i < count; i++)
            {
                output.WriteByte(value);
            }
        }
    }
}
