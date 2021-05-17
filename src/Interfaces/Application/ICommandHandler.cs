using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Application
{
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand commandRecord, CancellationToken ct);
    }
}