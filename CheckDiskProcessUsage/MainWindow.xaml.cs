using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
    }
}


