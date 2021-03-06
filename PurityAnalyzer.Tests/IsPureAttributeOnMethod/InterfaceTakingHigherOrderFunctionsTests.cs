﻿using FluentAssertions;
using NUnit.Framework;

namespace PurityAnalyzer.Tests.IsPureAttributeOnMethod
{
    [TestFixture]
    public class InterfaceTakingHigherOrderFunctionsTests
    {
        [Test]
        public void TakeingAnInterfaceFunctionAsAParameterAndCallingItKeepsTheMethodPure()
        {
            string code = @"
using System;

public class IsPureAttribute : Attribute
{
}

public interface IInterface
{
    string Call(int input);
}

public static class Module1
{
    [IsPure]
    public static string DoSomething(IInterface function)
    {
        return function.Call(1);
    }
}";

            var dignostics = Utilities.RunPurityAnalyzer(code);
            dignostics.Length.Should().Be(0);

        }

        [Test]
        public void CallingPureHigherOrderFunctionWithAPureClassKeepsMethodPure()
        {
            string code = @"
using System;

public class IsPureAttribute : Attribute
{
}

public interface IInterface
{
    int Call(int input);
}

public class PureClass : IInterface
{
    public int Call(int input)
    {
        return input;
    }
}

public static class Module1
{
    [IsPure]
    public static int DoSomething()
    {
        return HigherOrderFunction(new PureClass());
    }

    [IsPure]
    public static int HigherOrderFunction(IInterface function)
    {
        return function.Call(1);
    }
}";

            var dignostics = Utilities.RunPurityAnalyzer(code);
            dignostics.Length.Should().Be(0);

        }

        [Test]
        public void CallingPureHigherOrderFunctionWithAnImpureFunctionMakesMethodImpure()
        {
            string code = @"
using System;

public class IsPureAttribute : Attribute
{
}

public interface IInterface
{
    string Call(int input);
}

public class ImpureClass : IInterface
{
    int state = 0;

    public string Call(int input)
    {
        state++;

        return input.ToString();
    }
}

public static class Module1
{
    [IsPure]
    public static string DoSomething()
    {
        return HigherOrderFunction(new ImpureClass());
    }

    [IsPure]
    public static string HigherOrderFunction(IInterface function)
    {
        return function.Call(1);
    }
}";

            var dignostics = Utilities.RunPurityAnalyzer(code);
            dignostics.Length.Should().BePositive();

        }

    }
}
