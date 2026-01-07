// <copyright file="ArticleQueries.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;

namespace B2Connect.ERP.Queries;

/// <summary>
/// Query to get a single article.
/// </summary>
public record GetArticleQuery(string TenantId, int ArticleId);

/// <summary>
/// Query to get articles by IDs.
/// </summary>
public record GetArticlesByIdsQuery(string TenantId, IEnumerable<int> ArticleIds);

/// <summary>
/// Query to get articles with cursor pagination.
/// </summary>
public record QueryArticlesQuery(string TenantId, QueryRequest Query);
