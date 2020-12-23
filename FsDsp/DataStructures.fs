module FsDsp.DataStructures

let inline (%!) a b = (a % b + b) % b

type public CircularArray<'T> (a: 'T array ) =
    let data = a
    let mutable tracker = 0
    
    member this.Read index =
        data.[(tracker + index) %! a.Length] 
    member this.Write value =
        if tracker = 0 then tracker <- a.Length - 1 else tracker <- tracker - 1
        data.[tracker] <- value
    
    member this.Item
        with get(index) =
            this.Read(index)