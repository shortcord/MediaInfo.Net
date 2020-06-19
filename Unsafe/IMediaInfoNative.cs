using System;

namespace MediaInfoSharp.Unsafe
{
    public interface IMediaInfoNative : IDisposable
    {
        void Close();
        int Count_Get(StreamKind StreamKind);
        int Count_Get(StreamKind StreamKind, int StreamNumber);
        string Get(StreamKind StreamKind, int StreamNumber, int Parameter);
        string Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo);
        string Get(StreamKind StreamKind, int StreamNumber, string Parameter);
        string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo);
        string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch);
        string Inform();
        int Open(string FileName);
        int Open_Buffer_Continue(IntPtr Buffer, IntPtr Buffer_Size);
        long Open_Buffer_Continue_GoTo_Get();
        int Open_Buffer_Finalize();
        int Open_Buffer_Init(long File_Size, long File_Offset);
        string Option(string Option_);
        string Option(string Option, string Value);
        int State_Get();
    }
}