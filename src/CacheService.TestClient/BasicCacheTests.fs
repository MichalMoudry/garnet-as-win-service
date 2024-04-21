[<Sealed>]
module CacheService.TestClient

open System
open System.Text.Json
open System.Text.Json.Serialization
open NUnit.Framework
open StackExchange.Redis

/// A simple structure for testing different data types in JSON.
[<Struct>]
type TestData = {
    [<JsonPropertyName("string")>]
    TestString: string
    [<JsonPropertyName("int")>]
    TestInt: int
    [<JsonPropertyName("date")>]
    TestDate: DateTimeOffset
}

let mutable cache: option<IDatabase> = None

[<SetUp>]
let Setup () =
    task {
        let! conn = ConnectionMultiplexer.ConnectAsync("localhost:6379")
        cache <- Some(conn.GetDatabase())
    }

/// A test case covering a simple write and read from the cache.
[<TestCase("testKey", "test_value")>]
let TestStringKeySetAndRead (key: string, value: string) =
    let setResult = cache.Value.StringSet(key, value)
    Assert.That(setResult, Is.True)
    let keyVal = cache.Value.StringGet("testKey")

    Assert.That(keyVal.ToString(), Is.EqualTo(value))

/// A test case covering handling of JSON content.
[<Test>]
let TestJsonValueSetAndRead () =
    let inputData = {
        TestString = "test string"
        TestInt = 5
        TestDate = DateTimeOffset.Now
    }
    let setResult = cache.Value.StringSet(
        "test_val",
        JsonSerializer.Serialize(inputData)
    )
    Assert.That(setResult, Is.True)

    let cacheContent = JsonSerializer.Deserialize<TestData>(
        cache.Value.StringGet("test_val")
    )
    Assert.That(cacheContent, Is.EqualTo(inputData));