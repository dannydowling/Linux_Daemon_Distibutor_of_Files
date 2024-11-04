
using System.Diagnostics;

public class MeasureMedium 
{
    public Dictionary<string, long> sources;
    public long speed;
    public List<string> orderSources(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            //create the file stream to each of the endpoints
            FileStream myStream = new FileStream(args[i], FileMode.Open);
            sources.Add(args[i], measure(myStream));
        }
        sources.OrderBy(x => sources.Values);
    return sources.Keys.ToList();
    }

    public long measure(Stream myStream)
    {
        Stopwatch stopwatch = new Stopwatch();
        int offset = 0;
        stopwatch.Reset();
        stopwatch.Start();

        byte[] buffer = new byte[64]; // 64 Byte buffer
        int actualReadBytes = myStream.Read(buffer, offset, buffer.Length);
        stopwatch.Stop();
        long speed = (actualReadBytes * 8) / stopwatch.ElapsedMilliseconds; // kbps
        return speed;     
    }
}
 