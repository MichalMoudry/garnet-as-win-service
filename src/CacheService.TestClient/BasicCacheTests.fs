module CacheService.TestClient

open System
open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Serialization
open NUnit.Framework
open StackExchange.Redis

/// A simple structure for testing different data types in JSON.
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
            "127.0.0.1:6379,password=AxKZn7WuI.2dB6dp5|1z,abortConnect=false"
        )
        cache <- Some(conn.GetDatabase())
    }

/// A test case covering a simple write and read from the cache.
[<TestCase("testKey", "test_value")>]
//[<Ignore("For local dev")>]
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
//[<Ignore("For local dev")>]
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

[<Test>]
let TestListOps () =
    task {
        let key = $"test_list_{Guid.NewGuid()}"
        let input = {
            TestString = "test string"
            TestInt = 5
            TestDate = DateTimeOffset.Now
        }
        let content = JsonSerializer.Serialize(input)

        let! id1 = cache.Value.ListLeftPushAsync(key, content)
        Assert.That(id1, Is.EqualTo(1))

        let! id2 = cache.Value.ListLeftPushAsync(key, content)
        Assert.That(id2, Is.EqualTo(2))

        let! valuesRange = cache.Value.ListRangeAsync(key)
        let values = List<TestData>(valuesRange.Length)
        for value in valuesRange do
            values.Add(JsonSerializer.Deserialize(value))

        Assert.That(values, Does.Contain(input))
    }
