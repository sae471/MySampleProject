using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using SAE471.Application.DTOs;

namespace SAE471
{
    public static class QueryableExtensions
    {
        public const string FullTextSearchOprKey = "fullTextSearchKey";

        private static readonly Dictionary<string, Func<Expression, Expression, Expression>> BinaryOpFactory =
           new Dictionary<string, Func<Expression, Expression, Expression>>();
        public static IServiceProvider ServiceProvider { get => LazyServiceProvider.LazyGetService<IServiceProvider>(); }
        static QueryableExtensions()
        {
            BinaryOpFactory.Add("and", Expression.And);
            BinaryOpFactory.Add("or", Expression.Or);

            BinaryOpFactory.Add("eq", Expression.Equal);
            BinaryOpFactory.Add("equals", Expression.Equal);
            BinaryOpFactory.Add("neq", Expression.NotEqual);
            BinaryOpFactory.Add("notEquals", Expression.NotEqual);


            BinaryOpFactory.Add("gt", Expression.GreaterThan);
            BinaryOpFactory.Add("lt", Expression.LessThan);
            BinaryOpFactory.Add("gte", Expression.GreaterThanOrEqual);
            BinaryOpFactory.Add("lte", Expression.LessThanOrEqual);


            BinaryOpFactory.Add("contain", Contains);
            BinaryOpFactory.Add("contains", Contains);
            BinaryOpFactory.Add("ncontain", NotContains);
            BinaryOpFactory.Add("notContains", NotContains);

            BinaryOpFactory.Add("isnull", IsNull);
            BinaryOpFactory.Add("isnotnull", IsNotNull);
            BinaryOpFactory.Add("isempty", IsEmpty);
            BinaryOpFactory.Add("isnotempty", IsNotEmpty);

            // BinaryOpFactory.Add("startsWith", StartsWith);

            BinaryOpFactory.Add("startswith", StartsWith);

            // BinaryOpFactory.Add("endsWith", IsNotEmpty);

            BinaryOpFactory.Add("endswith", EndsWith);

            BinaryOpFactory.Add("dateis", Expression.Equal);
            BinaryOpFactory.Add("dateisnot", Expression.NotEqual);
            BinaryOpFactory.Add("datebefore", Expression.LessThan);
            BinaryOpFactory.Add("dateisorbefore", Expression.LessThanOrEqual);
            BinaryOpFactory.Add("dateafter", Expression.GreaterThan);
            BinaryOpFactory.Add("dateisorafter", Expression.GreaterThanOrEqual);

            BinaryOpFactory.Add("in", IsIn);
            BinaryOpFactory.Add("notin", IsNotIn);
        }
        private static Expression Contains(Expression lhs, Expression rhs)
        {
            return Expression.Call(lhs, "Contains", Type.EmptyTypes, rhs);
        }

        private static Expression NotContains(Expression lhs, Expression rhs)
        {
            return Expression.Not(Expression.Call(lhs, "Contains", Type.EmptyTypes, rhs));
        }

        private static Expression IsNull(Expression lhs, Expression rhs)
        {
            return Expression.Equal(lhs, Expression.Constant(null, lhs.Type));
        }

        private static Expression IsNotNull(Expression lhs, Expression rhs)
        {
            return Expression.NotEqual(lhs, Expression.Constant(null, lhs.Type));
        }

        private static Expression IsEmpty(Expression lhs, Expression rhs)
        {
            return Expression.Equal(lhs, Expression.Constant(string.Empty, lhs.Type));
        }

        private static Expression IsNotEmpty(Expression lhs, Expression rhs)
        {
            return Expression.NotEqual(lhs, Expression.Constant(string.Empty, lhs.Type));
        }

        private static Expression StartsWith(Expression lhs, Expression rhs)
        {
            return Expression.Call(lhs, "StartsWith", Type.EmptyTypes, rhs);
        }
        private static Expression EndsWith(Expression lhs, Expression rhs)
        {
            return Expression.Call(lhs, "EndsWith", Type.EmptyTypes, rhs);
        }
        private static Expression IsIn(Expression lhs, Expression rhs)
        {
            Expression expression = null;
            foreach (var item in rhs.ToString().Replace("\"", "").Split("|"))
            {
                object value = item;
                NormalizeValue(ref value, "eq", lhs.Type);
                var binaryExpression = Expression.Equal(lhs, Expression.Constant(value, lhs.Type));
                expression = expression == null ? binaryExpression : BinaryOpFactory["or"](expression, binaryExpression);
            }
            return expression;
        }
        private static Expression IsNotIn(Expression lhs, Expression rhs)
        {
            Expression expression = null;
            foreach (var item in rhs.ToString().Replace("\"", "").Split("|"))
            {
                object value = item;
                NormalizeValue(ref value, "neq", lhs.Type);
                var binaryExpression = Expression.NotEqual(lhs, Expression.Constant(value, lhs.Type));
                expression = expression == null ? binaryExpression : BinaryOpFactory["and"](expression, binaryExpression);
            }
            return expression;
        }
        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> source, string filters)
        {
            if (!string.IsNullOrEmpty(filters))
            {
                var selector = Expression.Parameter(typeof(TEntity), "selector");
                Expression conditions = null;
                foreach (var filter in filters.Split(','))
                {
                    var filterPart = filter.Split(' ');

                    if (filterPart.Length < 3) continue;

                    var logic = filterPart[0];
                    var field = filterPart[1];
                    var opr = filterPart[2];

                    // object value = filterPart.Length > 3 ? filterPart[3] : null;
                    object value = filterPart.Length > 3 ? String.Join(" ", filterPart.Skip(3).Take(filter.Length)) : null;
                    value = String.IsNullOrWhiteSpace(value.ToString()) || value.ToString().Trim().ToLower().Equals("null") ? null : value;
                    if (value == null && !(opr == "isnotempty" || opr == "isempty" || opr == "isnotnull" || opr == "isnull")) continue;

                    var memberValue = field.Split('.').Aggregate((Expression)selector, Expression.PropertyOrField);
                    var memberType = memberValue.Type;
                    try
                    {
                        NormalizeValue(ref value, opr, memberType);
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                    if (memberType == typeof(String))
                    {
                        memberValue = Expression.Call(memberValue, "ToLower", null, null);
                        memberValue = Expression.Call(memberValue, "Trim", null, null);

                        value = value != null ? value.ToString().Trim().ToLower() : value;

                        opr = opr == FullTextSearchOprKey ? "contain" : opr;
                    }
                    else
                    {
                        opr = opr == FullTextSearchOprKey ? "eq" : opr;
                    }

                    var condition = BinaryOpFactory[opr.ToLower()](memberValue, Expression.Constant(value, opr == "in" || opr == "notin" ? typeof(object) : memberType));
                    conditions = conditions == null ? condition : BinaryOpFactory[logic.ToLower()](conditions, condition);
                }

                if (conditions != null)
                {

                    var predicate = Expression.Lambda<Func<TEntity, bool>>(conditions, selector);
                    source = source.Where(predicate);
                }
            }

            return source;
        }

        public static void NormalizeValue(ref object value, string opr, Type memberType)
        {
            if (value != null && value.GetType() != memberType && opr.ToLower() != "in" && opr.ToLower() != "notin")
            {
                if (memberType.IsEnum)
                {
                    value = Enum.Parse(memberType, value.ToString());
                }
                else if (memberType == typeof(Guid) || memberType == typeof(Guid?))
                {
                    value = new Guid(value.ToString());
                }
                else
                {
                    // for nullable type 
                    if (memberType.IsGenericType && memberType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        if (value != null)
                        {
                            memberType = Nullable.GetUnderlyingType(memberType);
                        }
                    }
                    value = Convert.ChangeType(value, memberType);
                    if ((memberType == typeof(DateTime) || memberType == typeof(DateTime?)))
                    {
                        value = (value as DateTime?).Value.ToUniversalTime();
                    }
                }
            }
        }

        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> source, RequestDTO input)
        {
            input.SearchedText = input.SearchedText != null ? input.SearchedText.Trim() : null;

            var FullTextSearchFilering = "";

            var newSource = source.Filter(input.Filtering);

            if (!String.IsNullOrWhiteSpace(input.SearchedFields))
            {
                foreach (var field in input.SearchedFields.Split(","))
                {
                    FullTextSearchFilering += $"{(string.IsNullOrWhiteSpace(FullTextSearchFilering) ? "" : ",")}OR {field} {FullTextSearchOprKey} {input.SearchedText}";
                }
            }

            return newSource.Filter(FullTextSearchFilering);
        }
        public static IQueryable<TEntity> Select<TEntity>(this IQueryable<TEntity> source, RequestDTO input)
        {
            if (string.IsNullOrWhiteSpace(input.Fields))
            {
                return source;
            }

            if (!("," + input.Fields + ",").ToLower().Contains(",id,"))
            {
                input.Fields = "id," + input.Fields;
            }
            // input parameter "o"
            var xParameter = Expression.Parameter(typeof(TEntity), "o");

            // new statement "new Data()"
            var xNew = Expression.New(typeof(TEntity));

            var flag = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;
            // create initializers
            var bindings = input.Fields.Split(',').Select(o => o.Trim())
                .Select(o =>
                {
                    var field = o.Contains('.') ? o.Split('.')[0] : o;
                    // property "Field1"
                    var mi = typeof(TEntity).GetProperty(field, flag);

                    // original value "o.Field1"
                    var xOriginal = Expression.Property(xParameter, mi);

                    // set value "Field1 = o.Field1"
                    return Expression.Bind(mi, xOriginal);
                }
            );

            // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(xNew, bindings);

            // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<TEntity, TEntity>>(xInit, xParameter);


            //var expression = lambda.Compile();

            return source.Select(lambda);
        }
        public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> source, string sorts)
        {

            if (!string.IsNullOrEmpty(sorts))
            {
                var expression = source.Expression;
                int count = 0;
                foreach (var orderBy in sorts.Split(','))
                {
                    var item = orderBy.Split(' ');
                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var selector = Expression.PropertyOrField(parameter, item[0]);
                    var method = item.Length > 1 && item[1].ToLower().Equals("desc") ?
                        (count == 0 ? "OrderByDescending" : "ThenByDescending") :
                        (count == 0 ? "OrderBy" : "ThenBy");
                    expression = Expression.Call(typeof(Queryable), method,
                        new Type[] { source.ElementType, selector.Type },
                        expression, Expression.Quote(Expression.Lambda(selector, parameter)));
                    count++;
                }

                source = count > 0 ? source.Provider.CreateQuery<TEntity>(expression) : source;
            }

            return source;
        }
        public static Task<ResultDTO<TEntityDto>> ToResultDTO<TEntity, TEntityDto>(this IQueryable<TEntity> source, RequestDTO input)
        {
            if (!String.IsNullOrWhiteSpace(input.Fields) && !("," + input.Fields + ",").ToLower().Contains(",id,"))
            {
                input.Fields = "id," + input.Fields;
            }

            source = source.Filter(input);
            var totalCount = source.Count();
            source = source.Select(input);
            source = source.Sort(input.Sorting);
            source = source.Skip(input.SkipCount);
            source = source.Take(input.MaxResultCount);

            var list = source.ToList();
            IMapper Mapper = LazyServiceProvider.LazyGetService<IMapper>();

            var response = Mapper.Map<List<TEntity>, List<TEntityDto>>(list);

            return Task.Run(() =>
            {
                return new ResultDTO<TEntityDto>
                {
                    Items = response,
                    TotalCount = totalCount
                };
            });
        }
        public static Task<ResultDTO<KeyValueDTO<TPrimaryKey>>> ToLookup<TEntityDto, TPrimaryKey>(this ResultDTO<TEntityDto> ResultDTO, RequestDTO input)
            where TEntityDto : IEntityDTO<TPrimaryKey>
        {
            if (string.IsNullOrWhiteSpace(input.Fields))
            {

                return Task.Run(() =>
                {
                    return new ResultDTO<KeyValueDTO<TPrimaryKey>>();
                });
            }

            var list = ResultDTO.Items;
            var totalCount = ResultDTO.TotalCount;

            var fileds = input.Fields.Replace("id,", "").Replace(",id", "").Split(",");
            if (string.IsNullOrWhiteSpace(input.LookupStringFormat))
            {
                for (int i = 0; i < fileds.Length; i++)
                {
                    input.LookupStringFormat += (i > 0 ? " " : "") + "{" + i + "}";
                }
            }

            var flag = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;
            var response = list.Select(it => new KeyValueDTO<TPrimaryKey>()
            {
                Id = it.Id,
                Name = String.Format(input.LookupStringFormat,
                    fileds.Length > 0 && it.GetType().GetProperty(fileds[0], flag) != null && it.GetType().GetProperty(fileds[0], flag).GetValue(it) != null ? it.GetType().GetProperty(fileds[0], flag).GetValue(it).ToString() : "",
                    fileds.Length > 1 && it.GetType().GetProperty(fileds[1], flag) != null && it.GetType().GetProperty(fileds[1], flag).GetValue(it) != null ? it.GetType().GetProperty(fileds[1], flag).GetValue(it).ToString() : "",
                    fileds.Length > 2 && it.GetType().GetProperty(fileds[2], flag) != null && it.GetType().GetProperty(fileds[2], flag).GetValue(it) != null ? it.GetType().GetProperty(fileds[2], flag).GetValue(it).ToString() : "",
                    fileds.Length > 3 && it.GetType().GetProperty(fileds[3], flag) != null && it.GetType().GetProperty(fileds[3], flag).GetValue(it) != null ? it.GetType().GetProperty(fileds[3], flag).GetValue(it).ToString() : "",
                    fileds.Length > 4 && it.GetType().GetProperty(fileds[4], flag) != null && it.GetType().GetProperty(fileds[4], flag).GetValue(it) != null ? it.GetType().GetProperty(fileds[4], flag).GetValue(it).ToString() : "").TrimStart().TrimEnd(),
                Code = it.GetType().GetProperty("Code", flag) != null && it.GetType().GetProperty("Code", flag).GetValue(it) != null ? it.GetType().GetProperty("Code", flag).GetValue(it).ToString() : "null"
            }).ToList();

            return Task.Run(() =>
             {
                 return new ResultDTO<KeyValueDTO<TPrimaryKey>>()
                 {
                     Items = response,
                     TotalCount = totalCount
                 };
             });
        }
    }
}