using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoAudioControl
{
    internal class AudioDevice
    {
        static List<MMDevice> _microphones = new List<MMDevice>();
        static List<MMDevice> _speakers = new List<MMDevice>();
        static CoreAudioApi.MMDevice _activeSpeaker;
        static CoreAudioApi.AudioEndPointVolumeVolumeRange _volumeRange;
        static float _normalSpeakerVolume;

        /*
        private class RenderDevice
        {
            public readonly string Name;
            public readonly MMDevice Device;

            public RenderDevice(MMDevice device)
            {
                Device = device;
                Name = $"{device.Properties[PKey.DeviceDescription].Value} ({device.DeviceInterfaceFriendlyName})";
            }

            public override string ToString()
            {
                return Name;
            }
        }
        public static MMDeviceCollection GetAudioDevices()
        {
            CoreAudio.MMDeviceEnumerator deviceEnum = new CoreAudio.MMDeviceEnumerator(Guid.NewGuid());
            return deviceEnum.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        }
        public static void SetDefaultDevice(string id)
        {
            CoreAudio.MMDeviceEnumerator deviceEnum = new CoreAudio.MMDeviceEnumerator(Guid.NewGuid());
            CoreAudio.MMDevice device = deviceEnum.GetDevice(id);
            deviceEnum.SetDefaultAudioEndpoint(device);
        }

        public static void AudioDevices()
        {
            CoreAudio.MMDeviceEnumerator deviceEnumerator = new CoreAudio.MMDeviceEnumerator(Guid.NewGuid());
            CoreAudio.MMDeviceCollection devCol = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            int selectedIndex = 0;
            Console.WriteLine("Devices:");
            for (int i = 0; i < devCol.Count; i++)
            {
                Console.Write("{0}", devCol[i].DeviceInterfaceFriendlyName);
                RenderDevice rdev = new RenderDevice(devCol[i]);
                // rdev.Device.AudioSessionManager2.OnSessionCreated += HandleSessionCreated;
                if (rdev.Device.Selected) selectedIndex = i;
                Console.WriteLine(" [*]");
                // ComboBoxDevices.Items.Add(rdev);
            }
        }
        */
        public static MMDeviceCollection AudioDevices()
        {
            // Get the active microphone and speakers
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            MMDeviceCollection micList = deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eCapture, EDeviceState.DEVICE_STATE_ACTIVE);
            MMDeviceCollection speakerList = deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE);

            _activeSpeaker = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            _volumeRange = _activeSpeaker.AudioEndpointVolume.VolumeRange;
            _normalSpeakerVolume = _activeSpeaker.AudioEndpointVolume.MasterVolumeLevel;

            // ?? TODO: Add support for selecting applications that when their audio is above a certain level, turn down the audio of other applications.
            //DevicePeriod dp = _activeSpeaker.DevicePeriod;
            //Console.WriteLine(dp.DefaultPeriod);
            //Console.WriteLine(dp.MinimumPeriod);

            for (int i = 0; i < micList.Count; i++)
            {
                MMDevice mic = micList[i];
                _microphones.Add(mic);
                Console.WriteLine("Found microphone: " + mic.FriendlyName + " " + mic.ID);
            }

            for (int i = 0; i < speakerList.Count; i++)
            {
                MMDevice speaker = speakerList[i];
                _speakers.Add(speaker);
                Console.WriteLine("Found speaker: " + speaker.FriendlyName + " " + speaker.ID);
            }

            return speakerList;
        }

        public static void SetDefaultAudioOutputDevice(string deviceId)
        {
            // Create a new audio PolicyConfigClient
            PolicyConfigClient client = new PolicyConfigClient();

            // Using PolicyConfigClient, set the given device as the default device (for its type)
            client.SetDefaultEndpoint(deviceId, ERole.eMultimedia);
        }

        public static void SetDefaultAudioInputDevice(string deviceId)
        {
            // Create a new audio PolicyConfigClient
            PolicyConfigClient client = new PolicyConfigClient();

            // Using PolicyConfigClient, set the given device as the default communication device (for its type)
            client.SetDefaultEndpoint(deviceId, ERole.eCommunications);
        }
    }
}
