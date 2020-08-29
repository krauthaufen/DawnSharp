// Learn more about F# at http://fsharp.org

open System
open WebGPU

type Buffer with
    member x.Map(mode, offset, size) =
        Async.FromContinuations (fun (s,e,c) ->
            x.MapAsync(mode, offset, size, BufferMapCallback(fun status _ ->
                match status with
                | BufferMapAsyncStatus.Success -> 
                    s (x.GetMappedRange(offset, size))
                | _ ->
                    e (Exception "could not map buffer")
            ), 0n)
        )

let run(device : Device) =
    async {
        let buffer = 
            device.CreateBuffer {
                Label = "a"
                Size = 1024UL    
                Usage = BufferUsage.CopySrc ||| BufferUsage.CopyDst
                MappedAtCreation = 0
            }

        let! ptr = buffer.Map(MapMode.Write, 0un, 1024un)
        printfn "fill ptr %0x016X" ptr
        buffer.Unmap()

        buffer.Destroy()

    }

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
