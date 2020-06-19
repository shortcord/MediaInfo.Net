using System;
using System.Collections.Generic;
using System.Text;

namespace MediaInfoSharp
{
    public sealed class ProcessedFile : IEquatable<ProcessedFile>
    {
        internal ProcessedFile(string hash)
        {
            Hash = hash;
        }

        public string Format { get; internal set; }
        public IReadOnlyCollection<string> CommonExtensions { get; internal set; }
        public string MimeType { get; internal set; }
        public bool IsImage { get; internal set; }
        public bool IsVideo { get; internal set; }
        public ulong? Width { get; internal set; }
        public ulong? Height { get; internal set; }
        public float? Framerate { get; internal set; }
        /// <summary>
        /// Base64 Encoded SHA1 Hash of the contents.
        /// </summary>
        public string Hash { get; private set; }

        public override bool Equals(object obj)
            => Equals(obj as ProcessedFile);
        public bool Equals(ProcessedFile other)
            => other != null && Hash == other.Hash;
        public override int GetHashCode()
            => HashCode.Combine(Hash);

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Format("Hash: {0}", Hash));
            sb.AppendLine(string.Format("Format: {0}", Format));
            sb.AppendLine(string.Format("Common Exttensions: {0}", string.Join(',', CommonExtensions)));
            sb.AppendLine(string.Format("Is Image: {0}", IsImage));
            sb.AppendLine(string.Format("Is Video: {0}", IsVideo));
            sb.AppendLine(string.Format("MimeType: {0}", MimeType));
            sb.AppendLine(string.Format("Resolution: {0} x {1}", Width, Height));
            sb.AppendLine(string.Format("Framerate: {0}", Framerate));

            return sb.ToString();
        }
    }
}
