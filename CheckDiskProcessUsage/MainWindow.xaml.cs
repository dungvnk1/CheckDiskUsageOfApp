using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Collections.Generic;

namespace CheckDiskProcessUsage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private DiskUsageMonitor _diskUsageMonitor;

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string appPath = AppPathTextBox.Text;
            if (!string.IsNullOrEmpty(appPath))
            {
                // Assume you want to check the usage every minute (60000 milliseconds)
                _diskUsageMonitor = new DiskUsageMonitor(appPath, 60000);
                _diskUsageMonitor.Start();
                MessageBox.Show("Monitoring started. Please wait for 5 minutes to check process disk usage!");
            }
            else
            {
                MessageBox.Show("Please enter a valid path.");
            }
        }


        /*First test
        private void CheckDiskUsageButton_Click(object sender, RoutedEventArgs e)
        {
            string appPath = AppPathTextBox.Text.Trim();

            if (string.IsNullOrEmpty(appPath))
            {
                MessageBox.Show("Please enter the path to the application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Start the application process
                Process appProcess = new Process();
                appProcess.StartInfo.FileName = appPath;
                appProcess.Start();

                // Wait for the process to start (you may adjust the delay as needed)
                System.Threading.Thread.Sleep(5000); // Wait for 5 seconds

                // Check disk usage stability
                long initialDiskUsage = Process.GetCurrentProcess().WorkingSet64;

                // Simulate some activity in your application
                // ...

                // Wait for additional time to observe stability
                System.Threading.Thread.Sleep(5000); // Wait for another 5 seconds (adjust as needed)

                // Check disk usage again after some activity
                long finalDiskUsage = Process.GetCurrentProcess().WorkingSet64;

                // Compare disk usage to determine stability
                bool isStable = Math.Abs(finalDiskUsage - initialDiskUsage) < 1024; // Example: Stable if the difference is less than 1 KB

                // Display the result
                if (isStable)
                {
                    MessageBox.Show($"Disk usage is stable.\nInitial Disk Usage: {FormatBytes(initialDiskUsage)}\nFinal Disk Usage: {FormatBytes(finalDiskUsage)}", "Disk Usage Information");
                }
                else
                {
                    MessageBox.Show($"Disk usage is not stable.\nInitial Disk Usage: {FormatBytes(initialDiskUsage)}\nFinal Disk Usage: {FormatBytes(finalDiskUsage)}", "Disk Usage Information");
                }

                // Close the application process
                appProcess.CloseMainWindow();
                appProcess.WaitForExit();
                appProcess.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking disk usage: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;

            while (dblSByte >= 1024 && i < suffixes.Length - 1)
            {
                dblSByte /= 1024;
                i++;
            }

            return $"{dblSByte:0.##} {suffixes[i]}";
        }
        End first test*/
    }

    public class DiskUsageMonitor
    {
        private System.Timers.Timer _timer;
        private System.Timers.Timer _evaluationTimer;
        private string _processName;
        private double _pollInterval;
        private List<long> _diskUsageList = new List<long>();

        public DiskUsageMonitor(string processName, double pollIntervalInMilliseconds)
        {
            _processName = processName;
            _pollInterval = pollIntervalInMilliseconds;

            _timer = new System.Timers.Timer(_pollInterval);
            _timer.Elapsed += CheckDiskUsage;
            _timer.AutoReset = true;

            // Đặt thời gian chạy là 5 phút (300000 milliseconds)
            _evaluationTimer = new System.Timers.Timer(300000);
            _evaluationTimer.Elapsed += EvaluateAndReport;
            _evaluationTimer.AutoReset = false; // Chỉ chạy một lần
        }

        public void Start()
        {
            _timer.Start();
            _evaluationTimer.Start();
        }

        private void CheckDiskUsage(object sender, ElapsedEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(_processName);

            foreach (Process process in processes)
            {
                long diskUsage = GetProcessDiskUsage(process);
                _diskUsageList.Add(diskUsage);
            }
        }

        private long GetProcessDiskUsage(Process process)
        {
            long diskUsage = 0;
            try
            {
                PerformanceCounter diskCounter = new PerformanceCounter("Process", "IO Data Bytes/sec", process.ProcessName);
                diskUsage = diskCounter.RawValue;
                diskCounter.Close();
            }
            catch (Exception ex)
            {
                // Log or handle exception
                MessageBox.Show($"Error getting disk usage: {ex.Message}");
            }
            return diskUsage;
        }

        private void EvaluateAndReport(object sender, ElapsedEventArgs e)
        {
            Stop();
            bool isStable = EvaluateStability();
            Application.Current.Dispatcher.Invoke(() =>
                MessageBox.Show(isStable ? "Disk usage is stable." : "Disk usage is not stable.")
            );
        }

        public bool EvaluateStability()
        {
            if (_diskUsageList.Count < 2) return true; // Not enough data to evaluate stability.

            // Example stability evaluation: standard deviation less than a threshold indicates stability
            double average = _diskUsageList.Average();
            double sumOfSquaresOfDifferences = _diskUsageList.Sum(val => (val - average) * (val - average));
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / _diskUsageList.Count);

            // Check if standard deviation is lower than a determined threshold, e.g.:
            return sd < 10000; // Threshold of 10,000 bytes/sec difference for stability. Adjust as needed.
        }
        public void Stop()
        {
            _timer.Stop();
            _evaluationTimer.Stop();
        }
    }
}


