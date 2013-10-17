﻿// COPYRIGHT 2011, 2012, 2013 by the Open Rails project.
// 
// This file is part of Open Rails.
// 
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

// This file is the responsibility of the 3D & Environment Team. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.IO;
using System.Diagnostics;

namespace ORTS
{
    /// <summary>
    /// Wrapper class for the externals of library OpenAL
    /// </summary>
    public class OpenAL
    {
        public const int AL_NONE = 0;
        public const int AL_FALSE = 0;
        public const int AL_TRUE = 1;

        public const int AL_BUFFER = 0x1009;
        public const int AL_BUFFERS_QUEUED = 0x1015;
        public const int AL_BUFFERS_PROCESSED = 0x1016;
        public const int AL_PLAYING = 0x1012;
        public const int AL_SOURCE_STATE = 0x1010;
        public const int AL_SOURCE_TYPE = 0x1027;
        public const int AL_LOOPING = 0x1007;
        public const int AL_GAIN = 0x100a;
        public const int AL_VELOCITY = 0x1006;
        public const int AL_ORIENTATION = 0x100f;
        public const int AL_DISTANCE_MODEL = 0xd000;
        public const int AL_INVERSE_DISTANCE = 0xd001;
        public const int AL_INVERSE_DISTANCE_CLAMPED = 0xd002;
        public const int AL_LINEAR_DISTANCE = 0xd003;
        public const int AL_LINEAR_DISTANCE_CLAMPED = 0xd004;
        public const int AL_EXPONENT_DISTANCE = 0xd005;
        public const int AL_EXPONENT_DISTANCE_CLAMPED = 0xd006;
        public const int AL_MAX_DISTANCE = 0x1023;
        public const int AL_REFERENCE_DISTANCE = 0x1020;
        public const int AL_ROLLOFF_FACTOR = 0x1021;
        public const int AL_PITCH = 0x1003;
        public const int AL_POSITION = 0x1004;
        public const int AL_DIRECTION = 0x1005;
        public const int AL_SOURCE_RELATIVE = 0x0202;
        public const int AL_FREQUENCY = 0x2001;
        public const int AL_BITS = 0x2002;
        public const int AL_CHANNELS = 0x2003;
        public const int AL_BYTE_OFFSET = 0x1026;
        public const int AL_MIN_GAIN = 0x100d;
        public const int AL_MAX_GAIN = 0x100e;
        public const int AL_VENDOR = 0xb001;
        public const int AL_VERSION = 0xb002;
        public const int AL_RENDERER = 0xb003;
        public const int AL_DOPPLER_FACTOR = 0xc000;
        public const int AL_LOOP_POINTS_SOFT = 0x2015;
        public const int AL_STATIC = 0x1028;
        public const int AL_STREAMING = 0x1029;
        public const int AL_UNDETERMINED = 0x1030;

        public const int ALC_DEFAULT_DEVICE_SPECIFIER = 0x1004;
        public const int ALC_DEVICE_SPECIFIER = 0x1005;

        public const int AL_NO_ERROR = 0;
        public const int AL_INVALID = -1;
        public const int AL_INVALID_NAME = 0xa001; // 40961
        public const int AL_INVALID_ENUM = 0xa002; // 40962
        public const int AL_INVALID_VALUE = 0xa003; // 40963
        public const int AL_INVALID_OPERATION = 0xa004; // 40964
        public const int AL_OUT_OF_MEMORY = 0xa005; // 40965

        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr alcOpenDevice(string deviceName);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr alcCreateContext(IntPtr device, int[] attribute);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int alcMakeContextCurrent(IntPtr context);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string alcGetString(IntPtr device, int attribute);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int alcIsExtensionPresent(IntPtr device, string extensionName);

        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string AlInitialize(string devName);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int alIsExtensionPresent(string extensionName);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alGetBufferi(int buffer, int attribute, out int val);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string alGetString(int state);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int alGetError();
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alDeleteBuffers(int number, [In] ref int buffer);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alDeleteBuffers(int number, int[] buffers);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alDeleteSources(int number, [In] int[] sources);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alDeleteSources(int number, [In] ref int sources);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alDistanceModel(int model);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alGenSources(int number, out int source);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alGetSourcei(int source, int attribute, out int val);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alGetSourcef(int source, int attribute, out float val);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alGetSource3f(int source, int attribute, out float value1, out float value2, out float value3);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alListener3f(int attribute, float value1, float value2, float value3);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alListenerfv(int attribute, [In] float[] values);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alListenerf(int attribute, float value);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alGetListener3f(int attribute, out float value1, out float value2, out float value3);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourcePlay(int source);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourceRewind(int source);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourceQueueBuffers(int source, int number, [In] ref int buffer);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourcei(int source, int attribute, int val);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourcef(int source, int attribute, float val);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSource3f(int source, int attribute, float value1, float value2, float value3);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourcefv(int source, int attribute, [In] float[] values);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourceStop(int source);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourceUnqueueBuffers(int source, int number, int[] buffers);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alSourceUnqueueBuffers(int source, int number, ref int buffers);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int alGetEnumValue(string enumName);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alGenBuffers(int number, out int buffer);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alBufferData(int buffer, int format, [In] byte[] data, int size, int frequency);
        [SuppressUnmanagedCodeSecurity, DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void alBufferiv(int buffer, int attribute, [In] int[] values);

        public static int alSourceUnqueueBuffer(int SoundSourceID)
        {
            int bufid = 0;
            OpenAL.alSourceUnqueueBuffers(SoundSourceID, 1, ref bufid);
            return bufid;
        }

        public static string GetErrorString(int error)
        {
            if (error == AL_INVALID_ENUM)
                return "Invalid Enumeration";
            else if (error == AL_INVALID_NAME)
                return "Invalid Name";
            else if (error == AL_INVALID_OPERATION)
                return "Invalid Operation";
            else if (error == AL_INVALID_VALUE)
                return "Invalid Value";
            else if (error == AL_OUT_OF_MEMORY)
                return "Out Of Memory";
            else if (error == AL_NO_ERROR)
                return "No Error";
            
            return "";
        }

        public static void Initialize()
        {
            //if (alcIsExtensionPresent(IntPtr.Zero, "ALC_ENUMERATION_EXT") == AL_TRUE)
            //{
            //    string deviceList = alcGetString(IntPtr.Zero, ALC_DEVICE_SPECIFIER);
            //    string[] split = deviceList.Split('\0');
            //    Trace.TraceInformation("___devlist {0}",deviceList);
            //}
            int[] attribs = new int[0];
            IntPtr device = alcOpenDevice(null);
            IntPtr context = alcCreateContext(device, attribs);
            alcMakeContextCurrent(context);

            Trace.TraceInformation("Initialized OpenAL {0}; device '{1}' by '{2}'", alGetString(AL_VERSION), alGetString(AL_RENDERER), alGetString(AL_VENDOR));
        }
    }

    /// <summary>
    /// WAVEFILEHEADER binary structure
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct WAVEFILEHEADER
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] szRIFF;
        [FieldOffset(4), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint ulRIFFSize;
        [FieldOffset(8), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint padding;
    }

    /// <summary>
    /// RIFFCHUNK binary structure
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RIFFCHUNK
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] szChunkName;
        [FieldOffset(4), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint ulChunkSize;
    }

    /// <summary>
    /// WAVEFORMATEX binary structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WAVEFORMATEX
    {
        public ushort wFormatTag;
        public ushort nChannels;
        public uint nSamplesPerSec;
        public uint nAvgBytesPerSec;
        public ushort nBlockAlign;
        public ushort wBitsPerSample;
        public ushort cbSize;
    }

    /// <summary>
    /// WAVEFORMATEXTENSIBLE binary structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WAVEFORMATEXTENSIBLE
    {
        public WAVEFORMATEX Format;
        public ushort wValidBitsPerSample;
        public uint dwChannelMask;
        public Guid SubFormat;
    }

    /// <summary>
    /// CUECHUNK binary structure
    /// Describes the CUE chunk list of a wave file
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CUECHUNK
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] szChunkName;
        [FieldOffset(4), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint ulChunkSize;
        [FieldOffset(8), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint ulNumCuePts;
    }

    /// <summary>
    /// CUEPT binary structure
    /// Describes one CUE point in CUE list
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CUEPT
    {
        public uint ulID;
        public uint ulPlayPos;
        public uint ulRiffID;
        public uint ulChunkStart;
        public uint ulBlockStart;
        public uint ulByteStart;
    }


    /// <summary>
    /// SMPLCHUNK binary structure
    /// Describes the SMPL chunk list of a wave file
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SMPLCHUNK
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] ChunkName;
        [FieldOffset(4), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint ChunkSize;
        [FieldOffset(8), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint Manufacturer;
        [FieldOffset(12), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint Product;
        [FieldOffset(16), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint SmplPeriod;
        [FieldOffset(20), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint MIDIUnityNote;
        [FieldOffset(24), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint MIDIPitchFraction;
        [FieldOffset(28), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint SMPTEFormat;
        [FieldOffset(32), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint SMPTEOffset;
        [FieldOffset(36), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint NumSmplLoops;
        [FieldOffset(40), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint SamplerData;
    }

    /// <summary>
    /// SMPLLOOP binary structure
    /// Describes one SMPL loop in loop list
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SMPLLOOP
    {
        public uint ID;
        public uint Type;
        public uint ChunkStart;
        public uint ChunkEnd;
        public uint Fraction;
        public uint PlayCount;
    }
    public enum WAVEFORMATTYPE
    {
        WT_UNKNOWN,
        WT_PCM,
        WT_EXT
    }

    /// <summary>
    /// Helper class to load wave files
    /// </summary>
    public class WaveFileData
    {
        // Constants from C header files
        private const ushort WAVE_FORMAT_PCM                = 1;
        private const ushort WAVE_FORMAT_EXTENSIBLE         = 0xFFFE;

        private const ushort SPEAKER_FRONT_LEFT             = 0x1;
        private const ushort SPEAKER_FRONT_RIGHT            = 0x2;
        private const ushort SPEAKER_FRONT_CENTER           = 0x4;
        private const ushort SPEAKER_LOW_FREQUENCY          = 0x8;
        private const ushort SPEAKER_BACK_LEFT              = 0x10;
        private const ushort SPEAKER_BACK_RIGHT             = 0x20;
        private const ushort SPEAKER_FRONT_LEFT_OF_CENTER   = 0x40;
        private const ushort SPEAKER_FRONT_RIGHT_OF_CENTER  = 0x80;
        private const ushort SPEAKER_BACK_CENTER            = 0x100;
        private const ushort SPEAKER_SIDE_LEFT              = 0x200;
        private const ushort SPEAKER_SIDE_RIGHT             = 0x400;
        private const ushort SPEAKER_TOP_CENTER             = 0x800;
        private const ushort SPEAKER_TOP_FRONT_LEFT         = 0x1000;
        private const ushort SPEAKER_TOP_FRONT_CENTER       = 0x2000;
        private const ushort SPEAKER_TOP_FRONT_RIGHT        = 0x4000;
        private const ushort SPEAKER_TOP_BACK_LEFT          = 0x8000;
        
        // General info about current wave file
        public bool isKnownType;
        public WAVEFORMATEXTENSIBLE wfEXT;
        public WAVEFORMATTYPE wtType = new WAVEFORMATTYPE();

        public uint ulDataSize;
        public uint ulDataOffset;

        public ushort nChannels;
        public uint nSamplesPerSec;
        public ushort nBitsPerSample;

        public uint ulFormat;
        public FileStream pFile;
        public uint[] CuePoints;

        public WaveFileData()
        {
            pFile = null;
            isKnownType = false;
            wtType = WAVEFORMATTYPE.WT_UNKNOWN;

            ulFormat = 0;
            ulDataSize = 0;
            ulDataOffset = 0;

            nChannels = 0;
            nSamplesPerSec = 0;
            nBitsPerSample = 0;
        }

        public void Dispose()
        {
            if (pFile != null)
                pFile.Close();

            pFile = null;
        }

        /// <summary>
        /// Tries to read and parse a binary wave file
        /// </summary>
        /// <param name="n">Name of the file</param>
        /// <returns>True if success</returns>
        private bool ParseWAV(string n)
        {
            pFile = File.Open(n, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (pFile == null)
                return false;

            // Read Wave file header
            WAVEFILEHEADER waveFileHeader = new WAVEFILEHEADER();
            {
                GetNextStructureValue<WAVEFILEHEADER>(pFile, out waveFileHeader, - 1);
                // Check if wave file
                string hdr = new string(waveFileHeader.szRIFF);
                if (hdr != "RIFF" && hdr != "WAVE")
                    return false;

                // Read chunks : fmt, data, cue
                RIFFCHUNK riffChunk = new RIFFCHUNK();
                {
                    while (GetNextStructureValue<RIFFCHUNK>(pFile, out riffChunk, -1))
                    {
                        // Format chunk
                        hdr = new string (riffChunk.szChunkName);
                        if (hdr == "fmt ")
                        {
                            WAVEFORMATEXTENSIBLE waveFmt = new WAVEFORMATEXTENSIBLE();
                            if (riffChunk.ulChunkSize <= Marshal.SizeOf(waveFmt))
                            {
                                GetNextStructureValue<WAVEFORMATEXTENSIBLE>(pFile, out waveFmt, (int)riffChunk.ulChunkSize);

                                // Determine if this is a WAVEFORMATEX or WAVEFORMATEXTENSIBLE wave file
                                if (waveFmt.Format.wFormatTag == WAVE_FORMAT_PCM)
                                {
                                    isKnownType = true;
                                    wtType =  WAVEFORMATTYPE.WT_PCM;
                                    waveFmt.wValidBitsPerSample = waveFmt.Format.wBitsPerSample;
                                }
                                else if (waveFmt.Format.wFormatTag == WAVE_FORMAT_EXTENSIBLE)
                                {
                                    isKnownType = true;
                                    wtType = WAVEFORMATTYPE.WT_EXT;
                                }

                                wfEXT = waveFmt;
                                nBitsPerSample = waveFmt.Format.wBitsPerSample;
                                nChannels = waveFmt.Format.nChannels;
                                nSamplesPerSec = waveFmt.Format.nSamplesPerSec;
                            }
                            // Unexpected length
                            else
                            {
                                pFile.Seek(riffChunk.ulChunkSize, SeekOrigin.Current);
                            }
                        }
                        // Data chunk
                        else if (hdr == "data")
                        {
                            ulDataSize = riffChunk.ulChunkSize;
                            ulDataOffset = (uint)pFile.Position;
                            pFile.Seek(riffChunk.ulChunkSize, SeekOrigin.Current);
                        }
                        // CUE points
                        else if (hdr == "cue ")
                        {
                            // Seek back and read CUE header
                            pFile.Seek(Marshal.SizeOf(riffChunk) * -1, SeekOrigin.Current);
                            CUECHUNK cueChunk;
                            GetNextStructureValue<CUECHUNK>(pFile, out cueChunk, -1);
                            CuePoints = new uint[cueChunk.ulNumCuePts];
                            {
                                CUEPT cuePt;
                                uint pos;
                                // Read all CUE points
                                for (uint i = 0; i < cueChunk.ulNumCuePts; i++)
                                {
                                    if (GetNextStructureValue<CUEPT>(pFile, out cuePt, -1))
                                    {
                                        pos = 0;
                                        pos += cuePt.ulChunkStart;
                                        pos += cuePt.ulBlockStart;
                                        pos += cuePt.ulByteStart;

                                        CuePoints[i] = pos;
                                    }
                                }
                            }
                        }
                        else if (hdr == "smpl")
                        {
                            // Seek back and read SMPL header
                            pFile.Seek(Marshal.SizeOf(riffChunk) * -1, SeekOrigin.Current);
                            SMPLCHUNK smplChunk;
                            GetNextStructureValue<SMPLCHUNK>(pFile, out smplChunk, -1);
                            if (smplChunk.NumSmplLoops > 0)
                            {
                                CuePoints = new uint[smplChunk.NumSmplLoops * 2];
                                {
                                    SMPLLOOP smplLoop;
                                    for (uint i = 0; i < smplChunk.NumSmplLoops; i++)
                                    {
                                        if (GetNextStructureValue<SMPLLOOP>(pFile, out smplLoop, -1))
                                        {
                                            CuePoints[i * 2] = smplLoop.ChunkStart;
                                            CuePoints[i * 2 + 1] = smplLoop.ChunkEnd;
                                        }
                                    }
                                }
                            }
                        }
                        else // skip the unknown chunks
                        {
                            pFile.Seek(riffChunk.ulChunkSize, SeekOrigin.Current);
                        }

                        // Ensure that we are correctly aligned for next chunk
                        if ((riffChunk.ulChunkSize & 1) == 1)
                            pFile.Seek(1, SeekOrigin.Current);
                    } //get next chunk

                    // If no data found
                    if (ulDataSize == 0 || ulDataOffset == 0)
                        return false;

                    if (CuePoints != null)
                        Array.Sort(CuePoints);

                    return isKnownType;
                }
            }
        }

        /// <summary>
        /// Gets the wave file's correspondig AL format number
        /// </summary>
        /// <param name="pulFormat">Place to put the format number</param>
        /// <returns>True if success</returns>
        private bool GetALFormat(ref int pulFormat)
        {
            pulFormat = 0;

            if (wtType == WAVEFORMATTYPE.WT_PCM)
            {
                if (wfEXT.Format.nChannels == 1)
                {
                    switch (wfEXT.Format.wBitsPerSample)
                    {
                        case 4:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_MONO_IMA4");
                            break;
                        case 8:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_MONO8");
                            break;
                        case 16:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_MONO16");
                            break;
                    }
                }
                else if (wfEXT.Format.nChannels == 2)
                {
                    switch (wfEXT.Format.wBitsPerSample)
                    {
                        case 4:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_STEREO_IMA4");
                            break;
                        case 8:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_STEREO8");
                            break;
                        case 16:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_STEREO16");
                            break;
                    }
                }
                else if ((wfEXT.Format.nChannels == 4) && (wfEXT.Format.wBitsPerSample == 16))
                    pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_QUAD16");
            }
            else if (wtType == WAVEFORMATTYPE.WT_EXT)
            {
                if ((wfEXT.Format.nChannels == 1) && ((wfEXT.dwChannelMask == SPEAKER_FRONT_CENTER) || (wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT)) || (wfEXT.dwChannelMask == 0)))
                {
                    switch (wfEXT.Format.wBitsPerSample)
                    {
                        case 4:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_MONO_IMA4");
                            break;
                        case 8:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_MONO8");
                            break;
                        case 16:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_MONO16");
                            break;
                    }
                }
                else if ((wfEXT.Format.nChannels == 2) && (wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT)))
                {
                    switch (wfEXT.Format.wBitsPerSample)
                    {
                        case 4:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_STEREO_IMA4");
                            break;
                        case 8:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_STEREO8");
                            break;
                        case 16:
                            pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_STEREO16");
                            break;
                    }
                }
                else if ((wfEXT.Format.nChannels == 2) && (wfEXT.Format.wBitsPerSample == 16) && (wfEXT.dwChannelMask == (SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT)))
                    pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_REAR16");
                else if ((wfEXT.Format.nChannels == 4) && (wfEXT.Format.wBitsPerSample == 16) && (wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT)))
                    pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_QUAD16");
                else if ((wfEXT.Format.nChannels == 6) && (wfEXT.Format.wBitsPerSample == 16) && (wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_FRONT_CENTER | SPEAKER_LOW_FREQUENCY | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT)))
                    pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_51CHN16");
                else if ((wfEXT.Format.nChannels == 7) && (wfEXT.Format.wBitsPerSample == 16) && (wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_FRONT_CENTER | SPEAKER_LOW_FREQUENCY | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT | SPEAKER_BACK_CENTER)))
                    pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_61CHN16");
                else if ((wfEXT.Format.nChannels == 8) && (wfEXT.Format.wBitsPerSample == 16) && (wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_FRONT_CENTER | SPEAKER_LOW_FREQUENCY | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT | SPEAKER_SIDE_LEFT | SPEAKER_SIDE_RIGHT)))
                    pulFormat = OpenAL.alGetEnumValue("AL_FORMAT_71CHN16");
            }

            return pulFormat != 0;
        }

        /// <summary>
        /// Reads the wave contents of a wave file
        /// </summary>
        /// <param name="ToMono">True if must convert to mono before return</param>
        /// <returns>Read wave data</returns>
        private byte[] ReadData(bool ToMono)
        {
            byte[] buffer = null;
            if (pFile == null || ulDataOffset == 0 || ulDataSize == 0)
            {
                return buffer;
            }

            buffer = new byte[ulDataSize];
            if (buffer == null)
            {
                return buffer;
            }

            pFile.Seek(ulDataOffset, SeekOrigin.Begin);
            int size = (int)ulDataSize;
            if (pFile.Read(buffer, 0, size) != size)
            {
                buffer = null;
            }

            if (ToMono)
            {
                byte[] newbuffer = ConvertToMono(buffer);
                buffer = newbuffer;
            }

            return buffer;
        }

        /// <summary>
        /// Converts the read wave buffer to mono
        /// </summary>
        /// <param name="buffer">Buffer to convert</param>
        /// <returns>The converted buffer</returns>
        private byte[] ConvertToMono(byte[] buffer)
        {
            if (wfEXT.Format.nChannels == 1)
                return buffer;

            int pos = 0;
            int len = (int)ulDataSize / 2;

            byte[] retval = new byte[len];

            if (wfEXT.Format.wBitsPerSample == 8)
            {
                byte newval;

                while (pos < len)
                {
                    newval = buffer[pos * 2];
                    retval[pos] = newval;
                    pos++;
                }
            }
            else
            {
                MemoryStream sms = new MemoryStream(buffer);
                BinaryReader srd = new BinaryReader(sms);
                MemoryStream dms = new MemoryStream(retval);
                BinaryWriter drw = new BinaryWriter(dms);
                ushort newval;

                len /= 2;
                while (pos < len)
                {
                    newval = srd.ReadUInt16();
                    newval = srd.ReadUInt16();
                    drw.Write(newval);
                    pos++;
                }

                drw.Flush();
                drw.Close();
                dms.Flush();
                dms.Close();
                srd.Close();
                sms.Close();
            }

            wfEXT.Format.nChannels = 1;
            ulDataSize = (uint)retval.Length;
            if (CuePoints != null)
                for (var i = 0; i < CuePoints.Length; i++)
                    if (CuePoints[i] != 0xFFFFFFFF)
                        CuePoints[i] /= 2;

            return retval;
        }

        /// <summary>
        /// Opens, reads the given wave file. 
        /// Also creates the AL buffers and fills them with data
        /// </summary>
        /// <param name="Name">Name of the wave file to read</param>
        /// <param name="BufferIDs">Array of the buffer IDs to place</param>
        /// <param name="BufferLens">Array of the length data to place</param>
        /// <param name="ToMono">Indicates if the wave must be converted to mono</param>
        /// <param name="isReleasedWithJump">True if sound possibly be released with jump</param>
        /// <returns>True if success</returns>
        public static bool OpenWavFile(string Name, ref int[] BufferIDs, ref int[] BufferLens, bool ToMono, bool isReleasedWithJump, ref int numCuePoints)
        {
            WaveFileData wfi = new WaveFileData();
            int fmt = -1;

            if (!wfi.ParseWAV(Name))
            {
                return false;
            }

            if (wfi.ulDataSize == 0 || ((int)wfi.ulDataSize) == -1)
            {
                Trace.TraceWarning("Skipped wave file with invalid length {0}", Name);
                return false;
            }

            byte[] buffer = wfi.ReadData(ToMono);
            if (buffer == null)
            {
                return false;
            }

            if (!wfi.GetALFormat(ref fmt))
            {
                return false;
            }

            if (buffer.Length != wfi.ulDataSize)
            {
                Trace.TraceWarning("Invalid wave file length in header; expected {1}, got {2} in {0}", Name, buffer.Length, wfi.ulDataSize);
                wfi.ulDataSize = (uint)buffer.Length;
            }

            bool alLoopPointsSoft = false;
            int[] samplePos = new int[2];
            if (!isReleasedWithJump && wfi.CuePoints != null && wfi.CuePoints.Length > 1)
            {
                samplePos[0] = (int)(wfi.CuePoints[0]);
                samplePos[1] = (int)(wfi.CuePoints.Last());
                if (samplePos[0] < samplePos[1] && samplePos[1] <= wfi.ulDataSize / (wfi.nBitsPerSample / 8 * wfi.nChannels))
                    alLoopPointsSoft = OpenAL.alIsExtensionPresent("AL_SOFT_LOOP_POINTS") == OpenAL.AL_TRUE;
                numCuePoints = wfi.CuePoints.Length;
            }

            if (wfi.CuePoints == null || wfi.CuePoints.Length == 1 || alLoopPointsSoft)
            {
                BufferIDs = new int[1];
                BufferLens = new int[1];

                BufferLens[0] = (int)wfi.ulDataSize;

                if (BufferLens[0] > 0)
                {
                    OpenAL.alGenBuffers(1, out BufferIDs[0]);
                    OpenAL.alBufferData(BufferIDs[0], fmt, buffer, (int)wfi.ulDataSize, (int)wfi.nSamplesPerSec);

                    if (alLoopPointsSoft)
                        OpenAL.alBufferiv(BufferIDs[0], OpenAL.AL_LOOP_POINTS_SOFT, samplePos);
                }
                else
                    BufferIDs[0] = 0;

                return true;
            }
            else
            {
                BufferIDs = new int[wfi.CuePoints.Length + 1];
                BufferLens = new int[wfi.CuePoints.Length + 1];
                numCuePoints = wfi.CuePoints.Length;

                uint prevAdjPos = 0;
                for (var i = 0; i < wfi.CuePoints.Length; i++)
                {
                    uint adjPos = wfi.CuePoints[i] * wfi.nBitsPerSample / 8 * wfi.nChannels;
                    if (adjPos > wfi.ulDataSize)
                    {
                        Trace.TraceWarning("Invalid cue point in wave file; Length {1}, CUE {2}, BitsPerSample {3}, Channels {4} in {0}", Name, wfi.ulDataSize, adjPos, wfi.nBitsPerSample, wfi.nChannels);
                        wfi.CuePoints[i] = 0xFFFFFFFF;
                        adjPos = prevAdjPos;
                    }
                    
                    BufferLens[i] = (int)adjPos - (int)prevAdjPos;
                    if (BufferLens[i] > 0)
                    {
                        OpenAL.alGenBuffers(1, out BufferIDs[i]);
                        OpenAL.alBufferData(BufferIDs[i], fmt, GetFromArray(buffer, (int)prevAdjPos, BufferLens[i]), BufferLens[i], (int)wfi.nSamplesPerSec);
                    }
                    else
                    {
                        BufferIDs[i] = 0;
                    }
                    
                    if (i == wfi.CuePoints.Length - 1)
                    {
                        BufferLens[i + 1] = (int)wfi.ulDataSize - (int)adjPos;
                        if (BufferLens[i + 1] > 0)
                        {
                            OpenAL.alGenBuffers(1, out BufferIDs[i + 1]);
                            OpenAL.alBufferData(BufferIDs[i + 1], fmt, GetFromArray(buffer, (int)adjPos, BufferLens[i + 1]), BufferLens[i + 1], (int)wfi.nSamplesPerSec);
                        }
                        else
                        {
                            BufferIDs[i + 1] = 0;
                        }
                    }
                    prevAdjPos = adjPos;
                }
            }

            return true;
        }

        /// <summary>
        /// Extracts an array of bytes from an another array of bytes
        /// </summary>
        /// <param name="buffer">Initial buffer</param>
        /// <param name="offset">Offset from copy</param>
        /// <param name="len">Number of bytes to copy</param>
        /// <returns>New buffer with the extracted data</returns>
        static byte[] GetFromArray(byte[] buffer, int offset, int len)
        {
            byte[] retval = new byte[len];
            Buffer.BlockCopy(buffer, offset, retval, 0, len);
            return retval;
        }
        
        /// <summary>
        /// Reads a given structure from a FileStream
        /// </summary>
        /// <typeparam name="T">Type to read, must be able to Marshal to native</typeparam>
        /// <param name="fs">FileStream from read</param>
        /// <param name="retval">The filled structure</param>
        /// <param name="len">The bytes to read, -1 if the structure size must be filled</param>
        /// <returns>True if success</returns>
        public static bool GetNextStructureValue<T>(FileStream fs, out T retval, int len)
        {
            byte[] buffer;
            retval = default(T);
            if (len == -1)
            {
                buffer = new byte[Marshal.SizeOf(retval.GetType())];
            }
            else
            {
                buffer = new byte[len];
            }

            try
            {
                if (fs.Read(buffer, 0, buffer.Length) != buffer.Length)
                    return false;

                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                retval = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), retval.GetType());
                handle.Free();
                return true;
            }
            catch 
            {
                return false;
            }
        } 
    }
}

