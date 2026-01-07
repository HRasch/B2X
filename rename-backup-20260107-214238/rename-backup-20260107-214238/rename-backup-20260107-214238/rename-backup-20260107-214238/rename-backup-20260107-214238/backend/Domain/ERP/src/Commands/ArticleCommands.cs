// <copyright file="ArticleCommands.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;

namespace B2Connect.ERP.Commands;

/// <summary>
/// Command to get a single article.
/// </summary>
public record GetArticleCommand(string TenantId, int ArticleId);

/// <summary>
/// Command to query articles with filtering.
/// </summary>
public record QueryArticlesCommand(string TenantId, QueryRequest Query);

/// <summary>
/// Command to sync articles with delta changes.
/// </summary>
public record SyncArticlesCommand(string TenantId, DeltaSyncRequest Request);

/// <summary>
/// Command to batch write articles.
/// </summary>
public record BatchWriteArticlesCommand(string TenantId, IEnumerable<ArticleDto> Articles);
