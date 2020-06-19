using System;
using System.Runtime.InteropServices;

namespace MediaInfoSharp.Unsafe
{
    public class MediaInfoWindows : IMediaInfoNative
    {
        const string DllName = "MediaInfo.dll";

        [DllImport(DllName)]
        static extern IntPtr MediaInfo_New();
        [DllImport(DllName)]
        static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, long File_Size, byte[] Buffer, IntPtr Buffer_Size);
        [DllImport(DllName)]
        static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
        [DllImport(DllName)]
        static extern long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);
        [DllImport(DllName)]
        static extern void MediaInfo_Close(IntPtr Handle);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        [DllImport(DllName)]
        static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
        [DllImport(DllName)]
        static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);

        public MediaInfoWindows()
        {
            Handle = MediaInfo_New();
        }

        public void Dispose()
        {
            if (Handle != (IntPtr)0)
                MediaInfo_Delete(Handle);
        }

        public int Open(string FileName)
        {
            IntPtr FileName_Ptr = Marshal.StringToHGlobalAnsi(FileName);
            int ToReturn = (int)MediaInfoA_Open(Handle, FileName_Ptr);
            Marshal.FreeHGlobal(FileName_Ptr);
            return ToReturn;

        }
        public int Open_Buffer_Init(long File_Size, long File_Offset)
        {
            return (int)MediaInfo_Open_Buffer_Init(Handle, File_Size, File_Offset);
        }
        public int Open_Buffer_Continue(IntPtr Buffer, IntPtr Buffer_Size)
        {
            return (int)MediaInfo_Open_Buffer_Continue(Handle, Buffer, Buffer_Size);
        }
        public long Open_Buffer_Continue_GoTo_Get()
        {
            return MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle);
        }
        public int Open_Buffer_Finalize()
        {
            return (int)MediaInfo_Open_Buffer_Finalize(Handle);
        }
        public void Close()
        {
            MediaInfo_Close(Handle);
        }
        public string Inform()
        {
            return Marshal.PtrToStringAnsi(MediaInfoA_Inform(Handle, (IntPtr)0));
        }
        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
        {
            IntPtr Parameter_Ptr = Marshal.StringToHGlobalAnsi(Parameter);
            string ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter_Ptr, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));
            Marshal.FreeHGlobal(Parameter_Ptr);
            return ToReturn;
        }
        public string Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
        {
            return Marshal.PtrToStringAnsi(MediaInfoA_GetI(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
        }
        public string Option(string Option, string Value)
        {
            IntPtr Option_Ptr = Marshal.StringToHGlobalAnsi(Option);
            IntPtr Value_Ptr = Marshal.StringToHGlobalAnsi(Value);
            string ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Option(Handle, Option_Ptr, Value_Ptr));
            Marshal.FreeHGlobal(Option_Ptr);
            Marshal.FreeHGlobal(Value_Ptr);
            return ToReturn;
        }
        public int State_Get()
        {
            return (int)MediaInfo_State_Get(Handle);
        }

        public int Count_Get(StreamKind StreamKind, int StreamNumber)
        {
            return (int)MediaInfo_Count_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber);
        }

        private IntPtr Handle;

        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo)
        {
            return Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);
        }
        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter)
        {
            return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);
        }
        public string Get(StreamKind StreamKind, int StreamNumber, int Parameter)
        {
            return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text);
        }
        public string Option(string Option_)
        {
            return Option(Option_, "");
        }
        public int Count_Get(StreamKind StreamKind)
        {
            return Count_Get(StreamKind, -1);
        }
    }
}
