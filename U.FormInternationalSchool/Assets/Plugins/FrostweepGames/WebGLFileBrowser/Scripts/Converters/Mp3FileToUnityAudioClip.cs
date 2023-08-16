using UnityEngine;

namespace FrostweepGames.Plugins.WebGLFileBrowser
{
    public static class Mp3FileToUnityAudioClip
    {
        public static AudioClip ToAudioClip(byte[] bytes, string name = "Audio")
        {
            return ToAudioClip(bytes, 0, name);
        }

        private static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "Audio")
        {
            int byteCount = fileBytes.Length;
            int sampleCount = byteCount / 2;  // Assuming 16-bit samples
            float[] floatArray = new float[sampleCount];

            for (int i = 0, floatIndex = 0; i < byteCount; i += 2, floatIndex++)
            {
                short sampleValue = (short)(fileBytes[i] | (fileBytes[i + 1] << 8));
                float normalizedValue = sampleValue / 32768.0f;  // Convert to float in the range [-1, 1]
                floatArray[floatIndex] = normalizedValue;
            }
            
            AudioClip audioClip = AudioClip.Create("ConvertedClip", fileBytes.Length, 1, 44100, false);
            audioClip.SetData(floatArray, offsetSamples);

            return audioClip;
        }
    }
}