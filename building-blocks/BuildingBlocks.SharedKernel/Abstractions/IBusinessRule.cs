namespace BuildingBlocks.SharedKernel.Abstractions;

public interface IBusinessRule
{
    string Message { get; }
    bool IsBroken();
}
