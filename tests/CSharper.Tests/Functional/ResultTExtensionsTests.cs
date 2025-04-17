﻿using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public class ResultTExtensionsTests
{
    [Fact]
    public void Bind_SuccessResult_CallsNextAndReturnsResult()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Result> next = _ => Result.Ok();

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public void Bind_FailureResult_ReturnsMappedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Result> next = _ => Result.Ok();

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_NullNext_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Result> next = null!;

        Action act = () => initial.Bind(next);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void BindT_SuccessResult_CallsNextAndReturnsResultT()
    {
        string initialValue = "42";
        int expected = int.Parse(initialValue); ;
        Result<string> initial = Result.Ok(initialValue);
        Func<string, Result<int>> next = s => Result.Ok(expected);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public void BindT_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Result<int>> next = _ => Result.Ok(42);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindT_NullNext_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Result<int>> next = null!;

        Action act = () => initial.Bind(next);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Ensure_SuccessResult_ReturnsValidationChain()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, bool> predicate = s => s.Length > 0;
        Error error = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = initial.Ensure(predicate, error);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, "test");
    }

    [Fact]
    public void Ensure_FailureResult_ReturnsValidationChainWithFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, bool> predicate = s => s.Length > 0;
        Error validationError = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = initial.Ensure(predicate, validationError);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Ensure_NullPredicate_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, bool> predicate = null!;
        Error error = new("Too short", code: "SHORT");

        Action act = () => initial.Ensure(predicate, error);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Ensure_NullError_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, bool> predicate = s => s.Length > 0;
        Error error = null!;

        Action act = () => initial.Ensure(predicate, error);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_SuccessResult_TransformsValueAndReturnsResult()
    {
        Result<string> initial = Result.Ok("42");
        Func<string, int> map = s => int.Parse(s);
        int expected = 42;

        Result<int> result = initial.Map(map);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public void Map_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, int> map = s => int.Parse(s);

        Result<int> result = initial.Map(map);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Map_NullMap_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> map = null!;

        Action act = () => initial.Map(map);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MapError_FailureResult_ReturnsNonTypedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);

        Result result = initial.MapError();

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void MapError_SuccessResult_ThrowsInvalidOperationException()
    {
        Result<string> initial = Result.Ok("test");

        Action act = () => initial.MapError();

        AssertUtility.AssertException<InvalidOperationException>(act);
    }

    [Fact]
    public void MapErrorT_FailureResult_ReturnsTypedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);

        Result<int> result = initial.MapError<string, int>();

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void MapErrorT_SuccessResult_ThrowsInvalidOperationException()
    {
        Result<string> initial = Result.Ok("test");

        Action act = () => initial.MapError<string, int>();

        AssertUtility.AssertException<InvalidOperationException>(act);
    }

    [Fact]
    public void Match_SuccessOnly_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = s => s.Length;
        int expected = 4;

        int? result = initial.Match(onSuccess);

        result.Should().Be(expected);
    }

    [Fact]
    public void Match_SuccessOnly_FailureResult_ReturnsDefault()
    {
        Result<string> initial = Result.Fail<string>("Error");
        Func<string, int> onSuccess = s => s.Length;

        int? result = initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void Match_SuccessOnly_NullOnSuccess_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = null!;

        Action act = () => initial.Match(onSuccess);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailure_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = _ => -1;
        int expected = 4;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(expected);
    }

    [Fact]
    public void Match_SuccessAndFailure_FailureResult_CallsOnFailureAndReturnsValue()
    {
        Result<string> initial = Result.Fail<string>("Error");
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = errors => errors.Length;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void Match_SuccessAndFailure_NullOnSuccess_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = null!;
        Func<Error[], int> onFailure = _ => -1;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailure_NullOnFailure_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = null!;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Recover_FailureResult_CallsFallbackAndReturnsResult()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        string recoveredValue = "recovered";
        Func<Error[], Result<string>> fallback = _ => Result.Ok(recoveredValue);

        Result<string> result = initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, recoveredValue);
    }

    [Fact]
    public void Recover_SuccessResult_DoesNotCallFallbackAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        Func<Error[], Result<string>> fallback = _ => Result.Ok("recovered");

        Result<string> result = initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, originalValue);
    }

    [Fact]
    public void Recover_NullFallback_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Fail<string>("Error");
        Func<Error[], Result<string>> fallback = null!;

        Action act = () => initial.Recover(fallback);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Tap_SuccessResult_CallsActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        bool wasCalled = false;
        Action<string> action = _ => wasCalled = true;

        Result<string> result = initial.Tap(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void Tap_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        bool wasCalled = false;
        Action<string> action = _ => wasCalled = true;

        Result<string> result = initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Tap_NullAction_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Action<string> action = null!;

        Action act = () => initial.Tap(action);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void TapError_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result<string> result = initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void TapError_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result<string> result = initial.TapError(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapError_NullAction_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Fail<string>("Error");
        Action<Error[]> action = null!;

        Action act = () => initial.TapError(action);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }
}