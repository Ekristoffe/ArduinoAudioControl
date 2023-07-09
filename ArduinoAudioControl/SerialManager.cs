using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArduinoAudioControl
{
    internal class SerialManager
    {
        static bool _xContinue;
        static SerialPort _spSerialPort;
        static Thread _tReadThread;

        public static void Serial()
        {
            // string name;
            // string message;
            // StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            _tReadThread = new Thread(Read);

            // Create a new SerialPort object with default settings.
            _spSerialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _spSerialPort.PortName = SetPortName("COM10");
            _spSerialPort.BaudRate = SetPortBaudRate(250000);
            _spSerialPort.Parity = SetPortParity(_spSerialPort.Parity);
            _spSerialPort.DataBits = SetPortDataBits(_spSerialPort.DataBits);
            _spSerialPort.StopBits = SetPortStopBits(_spSerialPort.StopBits);
            _spSerialPort.Handshake = SetPortHandshake(_spSerialPort.Handshake);
            //_spSerialPort.PortName = "COM10";
            //_spSerialPort.BaudRate = 250000;
            //_spSerialPort.Parity = SetPortParity(_spSerialPort.Parity);
            //_spSerialPort.DataBits = SetPortDataBits(_spSerialPort.DataBits);
            //_spSerialPort.StopBits = SetPortStopBits(_spSerialPort.StopBits);
            //_spSerialPort.Handshake = SetPortHandshake(_spSerialPort.Handshake);

            // Set the read/write timeouts
            _spSerialPort.ReadTimeout = 500;
            _spSerialPort.WriteTimeout = 500;
            _spSerialPort.DtrEnable = true;
            _spSerialPort.RtsEnable = true;

            _spSerialPort.Open();
            _xContinue = true;
            _tReadThread.Start();
            /*
            Console.Write("Name: ");
            name = Console.ReadLine();

            Console.WriteLine("Type QUIT to exit");

            while (_xContinue)
            {
                message = Console.ReadLine();

                if (stringComparer.Equals("quit", message))
                {
                    _xContinue = false;
                }
                else
                {
                    _spSerialPort.WriteLine(
                        String.Format("<{0}>: {1}", name, message));
                }
            }
            */
            /*
            _tReadThread.Join();
            _spSerialPort.Close();
            */
        }

        public static void Close()
        {
            _xContinue = false;
            _tReadThread.Join();
            _spSerialPort.Close();
        }

        public static void Read()
        {
            MMDeviceCollection speakerList = AudioDevice.AudioDevices();
            Console.WriteLine("Found {0} speaker", speakerList.Count);
            int iLastVolume = (int)AudioManager.GetMasterVolume();
            int iLastMute = 0;
            int iLastOutput = 0;
            int iOutputSelect = 0;
            if (AudioManager.GetMasterVolumeMute())
            {
                iLastMute = 1;
            }
            _spSerialPort.WriteLine(String.Format("A,{0},{1},{2}", iLastVolume, iLastMute, iLastOutput));
            Console.WriteLine("booted");
            //WindowsSound sC = new SoundControl.WindowsSound();
            while (_xContinue)
            {
                try
                {
                    string message = _spSerialPort.ReadLine();
                    Console.WriteLine("message received: {0}", message);
                    string[] line = message.Split(',');
                    if (line.Length == 1)
                    {
                        if (line[0] == "I")
                        {
                            _spSerialPort.WriteLine(String.Format("A,{0},{1},{2}", iLastVolume, iLastMute, iLastOutput));
                            Console.WriteLine("init asked");
                        }
                    }
                    else if (line.Length == 4)
                    {
                        if (line[0] == "D")
                        {
                            int iVolume = int.TryParse(line[1], out iVolume) ? iVolume : iLastVolume;
                            int iIsMute = int.TryParse(line[2], out iIsMute) ? iIsMute : iLastMute;
                            int iOutput = int.TryParse(line[3], out iOutput) ? iOutput : iLastOutput;
                            if (iLastVolume != iVolume)
                            {
                                Console.WriteLine("Volume:{0}", iVolume);
                                AudioManager.SetMasterVolume(iVolume);
                                iLastVolume = iVolume;
                            }
                            if (iLastMute != iIsMute)
                            {
                                bool xIsMute = (iIsMute != 0);
                                if (xIsMute)
                                {
                                    Console.WriteLine("Muted");
                                }
                                else
                                {
                                    Console.WriteLine("Un-muted");
                                }
                                AudioManager.SetMasterVolumeMute(xIsMute);
                                iLastMute = iIsMute;
                            }
                            if (iLastOutput != iOutput)
                            {
                                iLastOutput = iOutput;
                                if (iOutputSelect < (speakerList.Count - 1))
                                {
                                    iOutputSelect++;
                                }
                                else
                                {
                                    iOutputSelect = 0;
                                }
                                Console.WriteLine("Output selected: {0} > {1}", speakerList[iOutputSelect].FriendlyName, speakerList[iOutputSelect].ID);
                                AudioDevice.SetDefaultAudioOutputDevice(speakerList[iOutputSelect].ID);

                                iLastVolume = (int)AudioManager.GetMasterVolume();
                                if (AudioManager.GetMasterVolumeMute())
                                {
                                    iLastVolume = 1;
                                }
                                _spSerialPort.WriteLine(String.Format("A,{0},{1},{2}", iLastVolume, iLastMute, iLastOutput));
                            }
                        }
                    }
                }
                catch (TimeoutException) { }
            }
        }

        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public static string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("COM port({0}): ", defaultPortName);
            portName = Console.ReadLine();

            if ((portName == "") || (portName == null))
            {
                portName = defaultPortName;
            }
            Console.WriteLine("");
            return portName;
        }

        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate({0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();

            if ((baudRate == "") || (baudRate == null))
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            Console.WriteLine("");
            return int.Parse(baudRate);
        }

        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Parity({0}):", defaultPortParity.ToString());
            parity = Console.ReadLine();

            if ((parity == "") || (parity == null))
            {
                parity = defaultPortParity.ToString();
            }

            Console.WriteLine("");
            return (Parity)Enum.Parse(typeof(Parity), parity);
        }

        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Data Bits({0}): ", defaultPortDataBits);
            dataBits = Console.ReadLine();

            if ((dataBits == "") || (dataBits == null))
            {
                dataBits = defaultPortDataBits.ToString();
            }

            Console.WriteLine("");
            return int.Parse(dataBits);
        }

        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available Stop Bits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Stop Bits({0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();

            if ((stopBits == "") || (stopBits == null))
            {
                stopBits = defaultPortStopBits.ToString();
            }

            Console.WriteLine("");
            return (StopBits)Enum.Parse(typeof(StopBits), stopBits);
        }

        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Handshake({0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine();

            if ((handshake == "") || (handshake == null))
            {
                handshake = defaultPortHandshake.ToString();
            }

            Console.WriteLine("");
            return (Handshake)Enum.Parse(typeof(Handshake), handshake);
        }

    }
}
