/// Module containing types used to aid during testing.
[<Sealed>]
module CacheService.UnitTests.TestTypes

[<Struct>]
type ExpectedServerSettings = {
    HostAddress: string
    Port: int
}