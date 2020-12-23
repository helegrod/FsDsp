open System
open System.IO
open System.Threading
open NAudio.Wave
open FsDsp.Filters

[<EntryPoint>]
let main argv =
    let filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\resources\\demo.flac"))
    let f: IIR = new IIR ([|0.45815f;  -2.29075f;   4.58151f;  -4.58151f;   2.29075f;  -0.45815f;|], [|1.00000f;  -3.56822f;   5.14596f;  -3.63691f;   1.19339f;  -0.11634f;|])
    use fileProvider = new AudioFileReader(filePath);
    use wo = new WaveOutEvent()
    let wp = new BufferedWaveProvider(fileProvider.WaveFormat)
    wo.Init(wp)
    wo.Play()
    
    let sampleProvider = fileProvider.ToSampleProvider()
    let buffer: float32[] = Array.zeroCreate(4096)
    let byteBuffer: byte[] =  Array.zeroCreate(4096 * 4)
    
    while fileProvider.CanRead do
        if(wp.BufferedDuration < TimeSpan.FromSeconds(1.0)) then
            let samples = sampleProvider.Read(buffer, 0, 4096)
            let processed = f.filter buffer
            Buffer.BlockCopy(processed, 0, byteBuffer, 0, byteBuffer.Length)
            wp.AddSamples(byteBuffer, 0, byteBuffer.Length)           
            Thread.Sleep(10)
    0 