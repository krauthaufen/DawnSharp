namespace Expecto

[<AutoOpen>]
module ExpectoConfigs =
    
    let cfg = { FsCheckConfig.defaultConfig with maxTest = 100; endSize = 300 }
    
    let property name test =
        testPropertyWithConfig cfg name test
