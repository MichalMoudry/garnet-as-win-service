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
        let! conn = ConnectionMultiplexer.ConnectAsync(
            "localhost:6379,password=AxKZn7WuI.2dB6dp5|1z"
        )
        cache <- Some(conn.GetDatabase())
    }

/// A test case covering a simple write and read from the cache.
[<TestCase("testKey", "test_value")>]
[<Ignore("For local dev")>]
let TestStringKeySetAndRead (key: string, value: string) =
    let uniqueKey = $"{key}_{Guid.NewGuid()}"

    let setResult = cache.Value.StringSet(uniqueKey, value)
    let keyVal = cache.Value.StringGet(uniqueKey)

    Assert.Multiple(fun () -> (
        Assert.That(setResult, Is.True)
        Assert.That(keyVal.ToString(), Is.EqualTo(value))
    ))

/// A test case covering handling of JSON content.
[<Test>]
[<Ignore("For local dev")>]
let TestJsonValueSetAndRead () =
    let uniqueKey = $"test_val_{Guid.NewGuid()}"
    let inputData = {
        TestString = "test string"
        TestInt = 5
        TestDate = DateTimeOffset.Now
    }
    let setResult = cache.Value.StringSet(
        uniqueKey,
        JsonSerializer.Serialize(inputData)
    )

    let cacheContent = JsonSerializer.Deserialize<TestData>(
        cache.Value.StringGet(uniqueKey)
    )

    Assert.Multiple(fun () -> (
        Assert.That(setResult, Is.True)
        Assert.That(cacheContent, Is.EqualTo(inputData));
    ))
