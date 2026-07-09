using Bookify.Application.Abstractions.Messaging;
using Bookify.ArchitectureTests.Infrastructure;
using FluentAssertions;
using FluentValidation;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Application;

public class ApplicationTests : BaseTest
{
    [Fact]
    public void CommandHandler_ShouldHave_NameEndingWith_CommandHandler()
    {
        Type[] failingTypes = ApplicationAssembly
            .GetTypes()
            .Where(type => type.Namespace is not "Bookify.Application.Abstractions.Behaviors")
            .Where(type => ImplementsGenericInterface(type, typeof(ICommandHandler<>)) ||
                           ImplementsGenericInterface(type, typeof(ICommandHandler<,>)))
            .Where(type => !type.Name.EndsWith("CommandHandler", StringComparison.Ordinal))
            .ToArray();

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void CommandHandler_Should_NotBePublic()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryHandler_ShouldHave_NameEndingWith_QueryHandler()
    {
        Type[] failingTypes = ApplicationAssembly
            .GetTypes()
            .Where(type => type.Namespace is not "Bookify.Application.Abstractions.Behaviors")
            .Where(type => ImplementsGenericInterface(type, typeof(IQueryHandler<,>)))
            .Where(type => !type.Name.EndsWith("QueryHandler", StringComparison.Ordinal))
            .ToArray();

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void QueryHandler_Should_NotBePublic()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validator_ShouldHave_NameEndingWith_Validator()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validator_Should_NotBePublic()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .NotBePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    private static bool ImplementsGenericInterface(Type type, Type genericInterfaceDefinition) =>
        type.GetInterfaces().Any(interfaceType =>
            interfaceType.IsGenericType &&
            interfaceType.GetGenericTypeDefinition() == genericInterfaceDefinition);
}
