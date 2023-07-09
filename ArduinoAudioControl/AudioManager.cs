using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoAudioControl
{
    public static class AudioManager
    {
        /*
        public static float GetMasterVolume2()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice device = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            AudioEndpointVolume volume = AudioEndpointVolume.FromDevice(device);
            return volume.MasterVolumeLevelScalar;
        }
        */
        public static float GetMasterVolume()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice device = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            return device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
        }

        public static void SetMasterVolume(float volume)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice device = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100;
        }

        public static bool GetMasterVolumeMute()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice device = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            return device.AudioEndpointVolume.Mute;
        }

        public static void SetMasterVolumeMute(bool isMuted)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice device = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            device.AudioEndpointVolume.Mute = isMuted;
        }
    }
}
