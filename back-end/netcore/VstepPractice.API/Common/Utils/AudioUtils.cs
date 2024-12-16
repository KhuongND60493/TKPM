using NAudio.Wave;
using NAudio.MediaFoundation;

namespace VstepPractice.API.Common.Utils;

public static class AudioUtils
{
    public static async Task<Stream> ConvertM4aToWav(Stream m4aStream)
    {
        // Initialize MediaFoundation for M4A support
        MediaFoundationApi.Startup();

        try
        {
            // Create a temporary file for the M4A content
            var tempM4aPath = Path.GetTempFileName();
            var tempWavPath = Path.GetTempFileName();

            try
            {
                // Save the M4A stream to temp file
                using (var fileStream = File.Create(tempM4aPath))
                {
                    await m4aStream.CopyToAsync(fileStream);
                }

                // Convert M4A to WAV
                using (var reader = new MediaFoundationReader(tempM4aPath))
                {
                    // Convert to WAV format required by Azure Speech Service
                    var wavFormat = new WaveFormat(16000, 16, 1); // 16kHz, 16-bit, mono
                    using (var converter = new MediaFoundationResampler(reader, wavFormat))
                    {
                        WaveFileWriter.CreateWaveFile(tempWavPath, converter);
                    }
                }

                // Read the WAV file into a memory stream
                var wavStream = new MemoryStream();
                using (var fileStream = File.OpenRead(tempWavPath))
                {
                    await fileStream.CopyToAsync(wavStream);
                }

                // Reset stream position
                wavStream.Position = 0;
                return wavStream;
            }
            finally
            {
                // Clean up temp files
                if (File.Exists(tempM4aPath))
                    File.Delete(tempM4aPath);
                if (File.Exists(tempWavPath))
                    File.Delete(tempWavPath);
            }
        }
        finally
        {
            MediaFoundationApi.Shutdown();
        }
    }
}