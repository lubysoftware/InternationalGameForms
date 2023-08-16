using System.Collections.Generic;
using System.Globalization;

namespace API
{
    /// <summary>
    /// Builder responsible for creating queries (strings) for consuming routes made using the package
    /// <a href="https://github.com/nestjsx/crud">NestjsX CRUD</a>.
    /// <br />
    /// For more information about the meaning of each command see
    /// <a href="https://github.com/nestjsx/crud/wiki/Requests#frontend-usage">NestjsX CRUD Request</a>. 
    /// </summary>
    public class RequestQueryBuilder
    {
        /// <summary>
        /// Defines which page will be queried.
        /// </summary>
        /// <param name="page">The page value.</param>
        /// <returns>The builder instance</returns>
        public RequestQueryBuilder SetPage(uint page)
        {
            _queryList.Add($"page={page}");
            return this;
        }

        /// <summary>
        /// Defines the page size.
        /// </summary>
        /// <param name="limit">The pave size value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetLimit(uint limit)
        {
            _queryList.Add($"limit={limit}");
            return this;
        }

        /// <summary>
        /// Defines the elements offset.
        ///
        /// The offset is defined by a value that says where the counting process will begin.
        /// </summary>
        /// <param name="offset">The page offset value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOffset(uint offset)
        {
            _queryList.Add($"offset={offset}");
            return this;
        }

        /// <summary>
        /// Defines which fields will be selected from the table.
        /// </summary>
        /// <param name="fields">The array of fields that will be selected.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFields(params string[] fields)
        {
            _queryList.Add($"fields={string.Join(',', fields)}");
            return this;
        }

        /// <summary>
        /// Defines which table will be joined.
        /// </summary>
        /// <param name="relation">The table name.</param>
        /// <param name="fields">The array of fields present in the table</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetJoin(string relation, params string[] fields)
        {
            _queryList.Add(fields.Length != 0
                ? $"join={relation}||{string.Join(',', fields)}"
                : $"join={relation}");

            return this;
        }

        /// <summary>
        /// Defines which field will be used for sorting the response.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="order">The response data order.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetSort(string field, Order order = Order.Asc)
        {
            _queryList.Add($"sort={field},{order.ToFriendlyString()}");
            return this;
        }

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The Filter condition.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFilter(string field, Condition condition)
        {
            _queryList.Add($"filter={field}||{condition.ToFriendlyString()}");
            return this;
        }

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFilter(string field, Condition condition, bool value) =>
            SetFilter(field, condition, value.ToString().ToLower());

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFilter(string field, Condition condition, int value) =>
            SetFilter(field, condition, value.ToString());

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFilter(string field, Condition condition, float value) =>
            SetFilter(field, condition, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFilter(string field, Condition condition, double value) =>
            SetFilter(field, condition, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition values.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFilter<T>(string field, Condition condition, IEnumerable<T> value) =>
            SetFilter(field, condition, string.Join(',', value));

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetFilter(string field, Condition condition, string value)
        {
            _queryList.Add($"filter={field}||{condition.ToFriendlyString()}||{value}");
            return this;
        }

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOr(string field, Condition condition)
        {
            _queryList.Add($"or={field}||{condition.ToFriendlyString()}");
            return this;
        }

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOr(string field, Condition condition, bool value) =>
            SetOr(field, condition, value.ToString().ToLower());

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOr(string field, Condition condition, int value) =>
            SetOr(field, condition, value.ToString());

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOr(string field, Condition condition, float value) =>
            SetOr(field, condition, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOr(string field, Condition condition, double value) =>
            SetOr(field, condition, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition values.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOr<T>(string field, Condition condition, IEnumerable<T> value) =>
            SetOr(field, condition, string.Join(',', value));

        /// <summary>
        /// Defines how the response will be filtered.
        /// </summary>
        /// <param name="field">The field name.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The condition value.</param>
        /// <returns>The builder instance.</returns>
        public RequestQueryBuilder SetOr(string field, Condition condition, string value)
        {
            _queryList.Add($"or={field}||{condition.ToFriendlyString()}||{value}");
            return this;
        }

        /// <summary>
        /// Builds a query string.
        /// </summary>
        /// <returns></returns>
        public string Build() => "?" + string.Join('&', _queryList.ToArray());

        /// <summary>
        /// Clears the current query.
        /// </summary>
        public void Reset() => _queryList.Clear();

        private readonly List<string> _queryList = new();
    }
}