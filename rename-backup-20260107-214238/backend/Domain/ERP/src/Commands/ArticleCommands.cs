// <copyright file="ArticleCommands.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;

namespace B2X.ERP.Commands;

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
