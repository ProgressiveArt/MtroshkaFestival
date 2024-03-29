﻿using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Application
{
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query, CancellationToken ct);
        TResult Handle(TQuery query);
    }
}