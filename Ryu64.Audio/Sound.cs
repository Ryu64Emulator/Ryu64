using System;
using System.Threading;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Ryu64.Audio
{
    public class Sound
    {
        public struct PCMSample
        {
            public int          Source;
            public int          Buffer;
            public AudioContext Context;
        }

        private static ALFormat GetSoundFormat(int bitrate, int channels)
        {
            switch (channels)
            {
                case 1: return bitrate == 8 ? ALFormat.Mono8   : ALFormat.Mono16;
                case 2: return bitrate == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotImplementedException($"{channels} channels with {bitrate} bitrate is not implemented.");
            }
        }

        public static unsafe PCMSample PlayPCM(byte[] Data, int bitrate, int channels, int freq, bool LE = true)
        {
            byte[] BufferData = Data;
            if (!LE)
            {
                int limit = BufferData.Length - (BufferData.Length % 2);
                if (limit < 1) throw new Exception("Sound too small to be swapped to Little Endian.");

                for (int i = 0; i < limit - 1; i += 2)
                {
                    byte temp         = BufferData[i];
                    BufferData[i]     = BufferData[i + 1];
                    BufferData[i + 1] = temp;
                }
            }

            AudioContext Context = new AudioContext();

            IntPtr pBuffer;
            fixed (byte* p = BufferData)
                pBuffer = (IntPtr)p;

            int Buffer = AL.GenBuffer();
            AL.BufferData(Buffer, GetSoundFormat(bitrate, channels), pBuffer, BufferData.Length, freq);

            int Source = AL.GenSource();
            AL.Source(Source, ALSourcei.Buffer, Buffer);
            AL.SourcePlay(Source);

            return new PCMSample
            {
                Source  = Source,
                Buffer  = Buffer,
                Context = Context
            };
        }

        public static bool IsSoundPlaying(PCMSample Sample)
        {
            AL.GetSource(Sample.Source, ALGetSourcei.SourceState, out int State);
            return (ALSourceState)State == ALSourceState.Playing;
        }

        public static void DisposePCM(PCMSample Sample)
        {
            Sample.Context.Dispose();
            AL.SourceStop  (Sample.Source);
            AL.DeleteSource(Sample.Source);
            AL.DeleteBuffer(Sample.Buffer);
        }
    }
}
