using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using System.Diagnostics;

namespace TurnOffDisplay
{
    internal class MicrophoneManager
    {
        private HubConnection _connection;
        private BufferedWaveProvider waveProvider;
        private WaveOutEvent waveOut;
        private string debugFilePath = @"C:\Users\yanmo\Desktop\tmp\test";
        private int i = 1;

        public MicrophoneManager()
        {
            this._connection = new HubConnectionBuilder().WithUrl("https://192.168.0.177/audiohub", options =>
            {
                options.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                    {
                        clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => true;
                    }
                    return message;
                };
            }).Build();

            this.waveProvider = new BufferedWaveProvider(new WaveFormat(48000, 1));
            this.waveOut = new WaveOutEvent
            {
                DeviceNumber = -1//this.GetDeviceNumber("CABLE Input")
            };
            this.waveOut.Init(this.waveProvider);
        }

        public async Task StartAsync()
        {
            try
            {
                this._connection.On<byte[]>("SendAudioChunk", async (audioData) =>
                {
                    try
                    {
                        this.DebugAudio(audioData);
                        var decodedData = this.DecodeAAC(audioData);
                        this.waveProvider.AddSamples(decodedData, 6000, decodedData.Length - 6000);

                        while (this.waveOut.PlaybackState != PlaybackState.Playing)
                        {
                            this.waveOut.Play();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error in adding buffer " + ex.Message + " of length " + audioData.Length);
                    }
                });
                await this._connection.StartAsync();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("Error in connection" + ex.Message);
            }
        }

        public async Task StopAsync()
        {
            await this._connection.StopAsync();
        }

        private void DebugAudio(byte[] data, bool play = false)
        {
            try
            {
                File.WriteAllBytes(this.debugFilePath + this.i, data);
                if (play)
                {
                    using (var audioFile = new AudioFileReader(this.debugFilePath + this.i))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                this.i++;
            }
        }

        private int GetDeviceNumber(string deviceName)
        {
            for (var i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);
                if (capabilities.ProductName.StartsWith(deviceName))
                {
                    return i;
                }
            }
            throw new ArgumentException($"No device found with name {deviceName}");
        }

        private byte[] DecodeAAC(byte[] aacData)
        {
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(tempFilePath, aacData);
            try
            {
                // Use NAudio to decode the temporary file
                using (var reader = new MediaFoundationReader(tempFilePath))
                using (var outputStream = new MemoryStream())
                {
                    WaveFileWriter.WriteWavFileToStream(outputStream, reader);
                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.Write("Error in decoding " + ex.Message);
                return new byte[1];
            }
            finally
            {
                // Clean up: delete the temporary file
                File.Delete(tempFilePath);
            }
        }
    }
}
