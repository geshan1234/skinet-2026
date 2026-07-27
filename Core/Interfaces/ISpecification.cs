using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Interfaces;

public interface ISpecification<T> where T : BaseEntity
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    bool IsDistinct { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
    IQueryable<T> ApplyCriteria(IQueryable<T> query);
}

public interface ISpecification<T, TResult> : ISpecification<T> where T : BaseEntity
{
    Expression<Func<T, TResult>>? Select { get; }
}
