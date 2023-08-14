using UnityEngine;
using System;
using System.Collections;
using System.IO;
using UnityEngine.Networking;


namespace FrostweepGames.Plugins.WebGLFileBrowser
{
    public static class OggFileToUnityAudioClip
    {
        public static AudioClip ToAudioClip(byte[] bytes, string name = "Audio")
        {
            return ToAudioClip(bytes, 0, name);
        }

        private static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "Audio")
        {
            using( var vorbis = new NVorbis.VorbisReader( new MemoryStream( fileBytes, false ) ) )
            {
                Debug.Log( $"Found ogg ch={vorbis.Channels} freq={vorbis.SampleRate} samp={vorbis.TotalSamples}" );
                float[] _audioBuffer = new float[vorbis.TotalSamples]; // Just dump everything
                int read = vorbis.ReadSamples( _audioBuffer, 0, (int)vorbis.TotalSamples );
                AudioClip audioClip = AudioClip.Create( name, (int)(vorbis.TotalSamples / vorbis.Channels), vorbis.Channels, vorbis.SampleRate, false);
                audioClip.SetData( _audioBuffer, 0 );
                return audioClip;
            }
            return null;
        }

    }
    

}