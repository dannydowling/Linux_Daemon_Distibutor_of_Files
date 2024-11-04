using System.Net.NetworkInformation;

namespace MultiSourceFileCopy
{
    public class FileCopier : IDisposable
    {
        private const int PacketSize = 1024; // Set your desired packet size in bytes

        public async static void CopyFiles(string[] destinationFilePaths, string sourceFilePath)
        {
            MeasureMedium measureMedium = new MeasureMedium();
            var orderedSources = measureMedium.orderSources(destinationFilePaths);

            foreach (var source in orderedSources)
            {
                // Check if the source file exists
                if (!File.Exists(source))
                {
                    orderedSources.Remove(source);
                    continue;
                }
            }

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 20
            };


            // Iterate through each source file
            await Parallel.ForEachAsync(orderedSources, options, async (sourceFilePath, ct) =>
            {
                // Get the file name from the source file path
                string fileName = Path.GetFileName(sourceFilePath);

                // Calculate the number of packets to copy from
                long fileSizeSource = new FileInfo(sourceFilePath).Length;
                int packetCountSource = (int)Math.Ceiling((double)fileSizeSource / PacketSize);
                int i = packetCountSource;

                // Use Task.Delay to implement a timeout
                Task delayTask = Task.Delay(5000);

                await Task.WhenAny(
                          delayTask,
                          Task.Run(() =>
                          {
                              using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                              {
                                  // we're actually reducing the packets left to copy each time the iteration happens.

                                  for (i = packetCountSource; i < packetCountSource; i--)
                                  {
                                      // Copy the packet to the destination file, cancel if timeout occurs

                                      // Create a packet-sized buffer
                                      byte[] buffer = new byte[PacketSize];

                                      // Read a packet from the source file
                                      int bytesRead = sourceStream.Read(buffer, 0, PacketSize);

                                      // Create the destination file path for the current packet
                                      string destinationFilePath = Path.Combine(sourceFilePath, $"{fileName}_Part{i + 1}.dat");

                                      // Write the packet to the destination file
                                      using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
                                      {
                                          destinationStream.Write(buffer, 0, bytesRead);
                                      }

                                      Console.WriteLine($"Packet {i + 1} from '{fileName}' copied successfully.");

                                  }
                              }
                          }
                          )
                          );
            });
            Console.WriteLine("File copying completed.");
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}