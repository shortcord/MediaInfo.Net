using MediaInfoSharp.Unsafe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace MediaInfoSharp
{
    public sealed class MediaInfo : IDisposable
    {
        readonly IMediaInfoNative Native;

        public MediaInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Native = new MediaInfoWindows();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Native = new MediaInfoLinux();
            }
            else
                throw new PlatformNotSupportedException($"{RuntimeInformation.OSDescription} is not supported.");
        }

        public string GetFullInform()
        {
            Native.Option("Complete", "1");
            var toReturn = Native.Inform();
            Native.Option("Complete", "0");
            return toReturn;
        }

        /// <summary>
        /// Read a file from disk.
        /// </summary>
        /// <param name="filename">Full path to file</param>
        public ProcessedFile ReadFile(string filename)
        {
            return ReadFile(new FileInfo(filename));
        }

        /// <summary>
        /// Read a file from disk.
        /// </summary>
        public ProcessedFile ReadFile(FileInfo file)
        {
            using var bufferedStream = new BufferedStream(file.OpenRead(), 64 * 1024 * 2);
            return ProcessStream(bufferedStream);
        }

        /// <summary>
        /// Read content from a stream.<br/>
        /// Stream must be readable and seekable.
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="InvalidOperationException"/>
        public ProcessedFile ProcessStream(Stream stream)
        {
            if (!stream.CanRead)
                throw new InvalidOperationException($"{nameof(stream)} must be readable.");

            if (!stream.CanSeek)
                throw new InvalidOperationException($"{nameof(stream)} must be seekable.");

            // Ensure the stream is at the start
            stream.Seek(0, SeekOrigin.Begin);

            int bytesRead;
            byte[] buffer = new byte[64 * 1024];
            GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                Native.Open_Buffer_Init(stream.Length, 0);

                IntPtr bufferPtr = bufferHandle.AddrOfPinnedObject();

                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);

                    Status result = Status.None;

                    result = (Status)Native.Open_Buffer_Continue(bufferPtr, (IntPtr)bytesRead);

                    if ((result & Status.Finalized) == Status.Finalized)
                        break;

                    var seekTo = Native.Open_Buffer_Continue_GoTo_Get();

                    if (seekTo != -1)
                    {
                        var position = stream.Seek(seekTo, SeekOrigin.Begin);
                        Native.Open_Buffer_Init(stream.Length, position);
                    }

                } while (bytesRead > 0);

                Native.Open_Buffer_Finalize();

            }
            finally
            {
                bufferHandle.Free();
            }

            string hash;
            {
                // Ensure the stream is at the start
                stream.Seek(0, SeekOrigin.Begin);

                // setup hasher
                using var sha1Hasher = new SHA1Managed();
                var rawHash = sha1Hasher.ComputeHash(stream);
                hash = Convert.ToBase64String(rawHash);
            }

            return CreateProcessedFile(hash);
        }

        ProcessedFile CreateProcessedFile(string hash)
        {
            return new ProcessedFile(hash)
            {
                Format = Format,
                CommonExtensions = CommonExtensions,
                MimeType = MimeType,
                IsImage = IsImage,
                IsVideo = IsVideo,
                Width = Width,
                Height = Height,
                Framerate = Framerate
            };

        }

        public string GetOptionValue(string option)
            => Native.Option(option);

        public string SetOptionValue(string option, string value)
            => Native.Option(option, value);

        public string GetParameterValue(StreamKind stream, int streamNumber, string parameter)
            => Native.Get(stream, streamNumber, parameter);

        public string GetParameterValue(StreamKind stream, string parameter)
            => GetParameterValue(stream, 0, parameter);

        public string Format
            => GetParameterValue(StreamKind.General, "Format");

        public IReadOnlyCollection<string> CommonExtensions
            => GetParameterValue(StreamKind.General, "Format/Extensions")
            .Split(" ", StringSplitOptions.RemoveEmptyEntries);

        public string MimeType 
            => GetParameterValue(StreamKind.General, "InternetMediaType");

        public bool IsImage
        {
            get
            {
                int.TryParse(GetParameterValue(StreamKind.General, "ImageCount"), out int imageCount);
                return imageCount > 0;
            }
        }

        public bool IsVideo
        {
            get
            {
                int.TryParse(GetParameterValue(StreamKind.General, "VideoCount"), out int videoCount);
                return videoCount > 0;
            }
        }

        public ulong? Width
        {
            get
            {
                ulong toReturn;

                if (ulong.TryParse(GetParameterValue(StreamKind.Video, "Width"), out toReturn))
                    if (toReturn != 0)
                        return toReturn;

                if (ulong.TryParse(GetParameterValue(StreamKind.Image, "Width"), out toReturn))
                    if (toReturn != 0)
                        return toReturn;

                return null;
            }
        }

        public ulong? Height
        {
            get
            {
                ulong toReturn;

                if (ulong.TryParse(GetParameterValue(StreamKind.Video, "Height"), out toReturn))
                    if (toReturn != 0)
                        return toReturn;

                if (ulong.TryParse(GetParameterValue(StreamKind.Image, "Height"), out toReturn))
                    if (toReturn != 0)
                        return toReturn;

                return null;
            }
        }

        public float? Framerate
        {
            get
            {
                float toReturn;

                float.TryParse(GetParameterValue(StreamKind.General, "FrameRate"), out toReturn);

                return toReturn;
            }
        }

        public void Dispose()
        {
            Native?.Dispose();
        }
    }
}
