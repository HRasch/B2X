using System;
using System.Collections.Generic;
using System.Linq;
using B2Connect.ErpConnector.Infrastructure.Erp;
using B2Connect.ErpConnector.Models;

namespace B2Connect.ErpConnector.Infrastructure.Repository
{
    /// <summary>
    /// Article query builder with type-safe filter methods.
    /// Based on eGate NVArticleQueryBuilder pattern.
    /// </summary>
    public class ArticleQueryBuilder : EnventaQueryBuilderBase<ArticleDto>
    {
        public ArticleQueryBuilder(IEnventaSelectRepository<ArticleDto> repository)
            : base(repository)
        {
        }

        /// <summary>
        /// Filter by article ID.
        /// </summary>
        public ArticleQueryBuilder WithArticleId(string articleId)
        {
            AddWhere($"ArticleId = {EscapeString(articleId)}");
            return this;
        }

        /// <summary>
        /// Filter by multiple article IDs.
        /// </summary>
        public ArticleQueryBuilder WithArticleIds(IEnumerable<string> articleIds)
        {
            var ids = articleIds?.ToList();
            if (ids == null || !ids.Any()) return this;

            var inClause = string.Join(", ", ids.Select(EscapeString));
            AddWhere($"ArticleId IN ({inClause})");
            return this;
        }

        /// <summary>
        /// Filter by name (contains).
        /// </summary>
        public ArticleQueryBuilder WithNameContains(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                AddWhere($"Name LIKE '%{name.Replace("'", "''")}%'");
            }
            return this;
        }

        /// <summary>
        /// Filter by article state.
        /// </summary>
        public ArticleQueryBuilder WithState(ArticleState state)
        {
            AddWhere($"ArticleState = {(int)state}");
            return this;
        }

        /// <summary>
        /// Filter active articles only.
        /// </summary>
        public ArticleQueryBuilder ActiveOnly()
        {
            AddWhere($"ArticleState = {(int)ArticleState.Active}");
            return this;
        }

        /// <summary>
        /// Filter by category.
        /// </summary>
        public ArticleQueryBuilder WithCategory(string category)
        {
            if (!string.IsNullOrWhiteSpace(category))
            {
                AddWhere($"Category = {EscapeString(category)}");
            }
            return this;
        }

        /// <summary>
        /// Filter by price range.
        /// </summary>
        public ArticleQueryBuilder WithPriceRange(decimal? minPrice, decimal? maxPrice)
        {
            if (minPrice.HasValue)
            {
                AddWhere($"Price >= {minPrice.Value}");
            }
            if (maxPrice.HasValue)
            {
                AddWhere($"Price <= {maxPrice.Value}");
            }
            return this;
        }

        /// <summary>
        /// Filter by minimum stock.
        /// </summary>
        public ArticleQueryBuilder WithMinStock(int minStock)
        {
            AddWhere($"StockQuantity >= {minStock}");
            return this;
        }

        /// <summary>
        /// Filter to include only e-commerce enabled articles.
        /// </summary>
        public ArticleQueryBuilder ECommerceEnabled()
        {
            AddWhere("NoECommerce = 0");
            return this;
        }

        /// <summary>
        /// Filter by modification date.
        /// </summary>
        public ArticleQueryBuilder ModifiedSince(DateTime since)
        {
            AddWhere($"ModifiedAt >= '{since:yyyy-MM-dd HH:mm:ss}'");
            return this;
        }

        /// <summary>
        /// Order by name ascending.
        /// </summary>
        public ArticleQueryBuilder OrderByName()
        {
            OrderByClause = "Name ASC";
            return this;
        }

        /// <summary>
        /// Order by name descending.
        /// </summary>
        public ArticleQueryBuilder OrderByNameDescending()
        {
            OrderByClause = "Name DESC";
            return this;
        }

        /// <summary>
        /// Order by price ascending.
        /// </summary>
        public ArticleQueryBuilder OrderByPrice()
        {
            OrderByClause = "Price ASC";
            return this;
        }

        /// <summary>
        /// Order by price descending.
        /// </summary>
        public ArticleQueryBuilder OrderByPriceDescending()
        {
            OrderByClause = "Price DESC";
            return this;
        }

        /// <summary>
        /// Order by modification date descending (newest first).
        /// </summary>
        public ArticleQueryBuilder OrderByModifiedDescending()
        {
            OrderByClause = "ModifiedAt DESC";
            return this;
        }
    }

    /// <summary>
    /// Article repository implementation.
    /// Based on eGate NVArticleRepository pattern.
    /// </summary>
    public class EnventaArticleRepository : EnventaBaseRepository<ArticleDto, IDevFrameworkDataObject>,
        IEnventaQueryRepository<ArticleDto, ArticleQueryBuilder>
    {
        private readonly Random _random = new Random();

        public EnventaArticleRepository(EnventaScope scope) : base(scope)
        {
        }

        /// <summary>
        /// Get type-safe query builder.
        /// </summary>
        public ArticleQueryBuilder Query()
        {
            return new ArticleQueryBuilder(this);
        }

        #region Mapping Implementation

        protected override ArticleDto ToDto(IDevFrameworkDataObject entity)
        {
            // In production with actual enventa assemblies:
            // var article = entity as IcECArticle;
            // return new ArticleDto
            // {
            //     ArticleId = article.ArticleNumber,
            //     Name = article.Name,
            //     ArticleState = (ArticleState)article.State,
            //     Price = article.Price,
            //     StockQuantity = article.Stock,
            //     NoECommerce = article.NoECommerce,
            //     ModifiedAt = article.ModifiedAt
            // };

            // Mock: entity is not used, we return mock data based on context
            return null; // Will be handled by mock data generation
        }

        protected override IDevFrameworkDataObject ToEntity(ArticleDto dto)
        {
            // In production:
            // var article = Scope.Create<IcECArticle>();
            // article.ArticleNumber = dto.ArticleId;
            // article.Name = dto.Name;
            // // ... map other properties
            // return article;

            throw new NotImplementedException("Mock implementation - no entity mapping");
        }

        protected override IDevFrameworkDataObject LoadEntity(string key)
        {
            // In production:
            // var article = Scope.Create<IcECArticle>();
            // article.Load(key);
            // return article.ROWID != null ? article : null;

            // Mock: return null, Find will use mock data
            return null;
        }

        protected override IEnumerable<IDevFrameworkDataObject> GetEntities(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null)
        {
            // In production with actual enventa:
            // return Scope.FSGlobalContext.GlobalObjects.GetArticles(where, orderBy, offset, limit);

            // Mock: return empty, Select will use mock data
            return Enumerable.Empty<IDevFrameworkDataObject>();
        }

        protected override int CountEntities(string where = "")
        {
            // Mock: return fixed count
            return 50;
        }

        protected override bool ExistsEntity(string where = "")
        {
            return true;
        }

        #endregion

        #region Override for Mock Data

        public override ArticleDto Find(string key)
        {
            Logger.Trace("Find Article by key: {0}", key);

            // Mock implementation
            return new ArticleDto
            {
                ArticleId = key,
                Name = $"Article {key}",
                ArticleState = ArticleState.Active,
                Price = Math.Round((decimal)(_random.NextDouble() * 1000), 2),
                StockQuantity = _random.Next(0, 500),
                NoECommerce = false,
                ModifiedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 30))
            };
        }

        public override IEnumerable<ArticleDto> Select(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null,
            IProgress<int> progress = null,
            System.Threading.CancellationToken ct = default)
        {
            Logger.Trace("Select Articles: where='{0}', orderBy='{1}', offset={2}, limit={3}",
                where, orderBy, offset, limit);

            // Mock implementation - generate sample data
            var totalCount = 50;
            var articles = new List<ArticleDto>();

            for (int i = 1; i <= totalCount; i++)
            {
                articles.Add(new ArticleDto
                {
                    ArticleId = $"ART{i:D5}",
                    Name = $"Article {i} - Sample Product",
                    ArticleState = (ArticleState)(i % 3),
                    Price = Math.Round((decimal)(_random.NextDouble() * 1000), 2),
                    StockQuantity = _random.Next(0, 500),
                    NoECommerce = i % 10 == 0,
                    ModifiedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 30))
                });
            }

            // Apply offset/limit
            var result = articles.AsEnumerable();
            if (offset.HasValue)
                result = result.Skip(offset.Value);
            if (limit.HasValue)
                result = result.Take(limit.Value);

            return result.ToList();
        }

        #endregion
    }
}
