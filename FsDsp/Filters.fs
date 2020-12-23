module FsDsp.Filters

open FsDsp.DataStructures
          

type public IIR (b: float32 array, a: float32 array, ?initialX: float32 array, ?initialY: float32 array) =
    let N = Array.length a
    let M = Array.length b
    
    let _b = Array.indexed b |> Array.skip 1
    let _a = Array.indexed a |> Array.skip 1
    
    let xState = initialX |> Option.fold (fun _ v -> v) (Array.zeroCreate M) |> CircularArray
    let yState = initialY |> Option.fold (fun _ v -> v) (Array.zeroCreate N) |> CircularArray
    
    member public this.filter (data: float32 array): float32 array =
        data |> Array.map (fun f ->
            let sumB = _b |> Array.fold (fun (sum: float32) (i: int, bi:float32) -> (sum + bi * xState.[i - 1])) (f*b.[0])
            let sumA = _a |> Array.fold (fun (sum: float32) (i: int, ai:float32) -> (sum + ai * yState.[i - 1])) (0.0f)
            let yn = (sumB-sumA)
            xState.Write(f)
            yState.Write(yn)
            yn
        )

type public ListIIR (b: float32 array, a: float32 array, ?initialX: float32 array, ?initialY: float32 array) =
    let N = Array.length a
    let M = Array.length b
    
    let _b = Array.indexed b |> Array.skip 1
    let _a = Array.indexed a |> Array.skip 1
    
    let mutable xState = initialX |> Option.fold (fun _ v -> v |> Array.toList) ([for i in 0 .. M -> 0.0f])
    let mutable yState = initialY |> Option.fold (fun _ v -> v |> Array.toList) ([for i in 0 .. N -> 0.0f])
    
    member public this.filter (data: float32 array): float32 array =
        data |> Array.map (fun f ->
            let sumB = _b |> Array.fold (fun (sum: float32) (i: int, bi:float32) -> (sum + bi * xState.[i - 1])) (f*b.[0])
            let sumA = _a |> Array.fold (fun (sum: float32) (i: int, ai:float32) -> (sum + ai * yState.[i - 1])) (0.0f)
            let yn = (sumB-sumA)
            xState <- f::xState.[0..M-1]
            yState <- yn::yState.[0..N-1]
            yn
        )