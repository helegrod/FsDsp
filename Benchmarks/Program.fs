// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Jobs
open BenchmarkDotNet.Running
open FsDsp.Filters
    
[<SimpleJob(RuntimeMoniker.NetCoreApp50)>]
type public filterBenchmarks () =
    let f: IIR = new IIR ([|0.45815f;  -2.29075f;   4.58151f;  -4.58151f;   2.29075f;  -0.45815f;|], [|1.00000f;  -3.56822f;   5.14596f;  -3.63691f;   1.19339f;  -0.11634f;|])
    let rng = Random();
    let demoData = [|0 .. 44100|] |> Array.map(fun x -> 2.0*rng.NextDouble() |> float32)
    
    let list_f: ListIIR = new ListIIR ([|0.45815f;  -2.29075f;   4.58151f;  -4.58151f;   2.29075f;  -0.45815f;|], [|1.00000f;  -3.56822f;   5.14596f;  -3.63691f;   1.19339f;  -0.11634f;|]) 
    
    
    [<Benchmark>]
    member public this.list_filter() =
        list_f.filter demoData
    [<Benchmark>]
    member public this.buffer_filter() =
        f.filter demoData
        
[<EntryPoint>]
let main argv =
    let results = BenchmarkRunner.Run<filterBenchmarks>();
    0 // return an integer exit code