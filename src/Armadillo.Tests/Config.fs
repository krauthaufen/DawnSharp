namespace Expecto

open FsCheck
open Aardvark.Base

type CustomProvider() =
    static member Range1i() =
        gen {
            let! min = Arb.generate<int>
            let! size = Gen.choose (0, 50)
            return Range1i(min, min + size)

        } |> Arb.fromGen

[<AutoOpen>]
module ExpectoConfigs =

    let cfg = 
        { 
            FsCheckConfig.defaultConfig with 
                maxTest = 1000
                endSize = 300 
                arbitrary = [ typeof<CustomProvider> ]
        }
    
    let property name test =
        testPropertyWithConfig cfg name test
