using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media.Imaging;

namespace NekoLib.ADB
{
    class ADB
    {
        public static string driveIPAddress = string.Empty;
        public static bool Execute(string command, out List<string> output, out List<string> error)
        {
            // adb command
            /*Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.Arguments = command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // 隐藏窗口
            process.StartInfo.CreateNoWindow = true;         // 不创建新窗口    

            process.Start();
            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            process.WaitForExit();*/

            var _output = new List<string>();
            var _error = new List<string>();


            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                FileName = "adb.exe",
                Arguments = Encoding.Default.GetString(Encoding.UTF8.GetBytes(command)),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8
            };

            Process proc = new()
            {
                StartInfo = StartInfo
            };

            AutoResetEvent errorWaitHandle = new(false);
            AutoResetEvent outputWaitHandle = new(false);

            proc.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    outputWaitHandle.Set();
                }
                else
                {
                    _output.Add(e.Data);
                }
            };

            proc.ErrorDataReceived += (s, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    errorWaitHandle.Set();
                }
                else
                {
                    _error.Add(e.Data);
                }
            };

            proc.Start();
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
            proc.WaitForExit(10000);
            errorWaitHandle.WaitOne(10000);
            outputWaitHandle.WaitOne(10000);

            output = _output;
            error = _error;

            if (error.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool Usb(out string error)
        {
            // adb usb
            Execute("usb", out _, out var _error);

            if (_error.Count == 0)
            {
                error = string.Empty;
                return true;
            }
            else
            {
                error = _error[0];
                return false;
            }
        }
        public static bool Pairing(string ipAddressEndPort, string password, out string error)
        {
            // adb pair ipAddress:port password
            Execute($"pair {ipAddressEndPort} {password}", out var output, out var _error);

            if (_error.Count == 0)
            {
                error = string.Empty;
                return true;
            }
            else
            {
                error = _error[0];
                return false;
            }
        }
        public static bool MdnsCheck(out string error)
        {
            // adb mdns check
            Execute("mdns check", out _, out var _error);


            if (_error.Count == 0)
            {
                error = string.Empty;
                return true;
            }
            else
            {
                error = _error[0];
                return false;
            }
        }
        public static bool Connect(string ipAddressEndPort, out string error)
        {
            // adb connect
            Execute($"connect {ipAddressEndPort}", out var output, out var _error);

            if (_error.Count == 0)
            {
                if (output[0].Contains("10061"))
                {
                    error = output[0];
                }
                else
                {
                    error = string.Empty;
                }

                return true;
            }
            else
            {
                error = _error[0];
                return false;
            }
        }
        public static bool Disconnect(string ipAddressEndPort, out string error)
        {
            // adb disconnect
            Execute($"disconnect {ipAddressEndPort}", out _, out var _error);

            if (_error.Count == 0)
            {
                error = string.Empty;
                return true;
            }
            else
            {
                error = _error[0];
                return false;
            }
        }
        public static List<(string IPAddress, string DeviceProduct, string Model, string Device, string TransportId)> Devices()
        {
            // adb devices -l
            Execute($"devices -l", out var output, out _);
            List<(string IPAddress, string DeviceProduct, string Model, string Device, string TransportId)> deviceList = new();

            for (int i = 1; i < output.Count; i++)
            {
                string input = Regex.Replace(output[i], @"\s+", " ");
                input = Regex.Replace(input, @"(device product|model|device|transport_id):", string.Empty);
                string[] deviceInfo = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                deviceList.Add((deviceInfo[0], deviceInfo[1], deviceInfo[2], deviceInfo[3], deviceInfo[4]));
            }

            return deviceList;
        }
        public static BitmapImage Screencap()
        {
            // adb exec-out screencap -p
            #error 无法获取到截图，2秒获取一次截图，太慢了
            Execute($"exec-out screencap -p>screencap.png", out _, out _);

            BitmapImage image = new();

            if (File.Exists("screencap.png"))
            {
                var imgBytes = File.ReadAllBytes("screencap.png");
                using MemoryStream stream = new(imgBytes);

                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                File.Delete("screencap.png");
            }

            return image;
        }
    }
}
