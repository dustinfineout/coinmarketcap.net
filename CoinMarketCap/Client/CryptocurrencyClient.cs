﻿using CoinMarketCap.DataContracts;
using CoinMarketCap.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CoinMarketCap.Client
{
    // ReSharper disable once UnusedMember.Global
    public class CryptocurrencyClient : ApiClientBase
    {
        public CryptocurrencyClient(string apiKey, bool sandbox = false)
            : base(apiKey, sandbox)
        { }

        #region Endpoint: /v1/cryptocurrency/map - CoinMarketCap ID map

        /// <summary>
        /// Returns a mapping of all cryptocurrencies to unique CoinMarketCap IDs.
        /// <remarks>See https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyMap </remarks>
        /// </summary>
        /// <param name="listingStatus">Default: <value>"active"</value>. Only active cryptocurrencies are returned by default. Pass <value>inactive</value> to get a list of cryptocurrencies that are no longer active. Pass <value>untracked</value> to get a list of cryptocurrencies that are listed but do not yet meet methodology requirements to have tracked markets available. You may pass one or more comma-separated values.</param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="sort"></param>
        /// <param name="symbol"></param>
        /// <param name="aux"></param>
        /// <returns></returns>
        public ApiResponseList<CryptocurrencyIdMapping> Map(string listingStatus = null, int? start = null,
            int? limit = null, eSortCryptocurrencyMap? sort = null, string symbol = null, string aux = null)
        {
            if (start.HasValue && start < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start.Value,
                    $"{nameof(start)} must be greater than or equal to 1.");
            }

            if (limit.HasValue && (limit < 1 || limit > 5000))
            {
                throw new ArgumentOutOfRangeException(nameof(limit), limit.Value,
                    $"{nameof(limit)} must be between 1 and 5000.");
            }

            return ApiRequest<ApiResponseList<CryptocurrencyIdMapping>>("cryptocurrency/map",
                new Dictionary<string, string>
                {
                    ["listing_status"] = listingStatus,
                    ["start"] = start?.ToString(),
                    ["limit"] = limit?.ToString(),
                    ["sort"] = sort?.GetDescription(),
                    ["symbol"] = symbol,
                    ["aux"] = aux
                });
        }

        #endregion Endpoint: /v1/cryptocurrency/map - CoinMarketCap ID map

        #region Endpoint: /v1/cryptocurrency/info - Metadata

        // ReSharper disable once UnusedMember.Global
        public ApiResponseMap<CryptocurrencyMetadata> MetadataById(
            string id, string aux = null)
        {
            return Metadata(id, null, null, aux);
        }
        // ReSharper disable once UnusedMember.Global
        public ApiResponseMap<CryptocurrencyMetadata> MetadataBySlug(
            string slug, string aux = null)
        {
            return Metadata(null, slug, null, aux);
        }
        // ReSharper disable once UnusedMember.Global
        public ApiResponseMap<CryptocurrencyMetadata> MetadataBySymbol(
            string symbol, string aux = null)
        {
            return Metadata(null, null, symbol, aux);
        }

        /// <summary>
        /// Returns all static metadata available for one or more cryptocurrencies. This information
        /// includes details like logo, description, official website URL, social links, and links to a
        /// cryptocurrency's technical documentation.
        /// <remarks>See https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyInfo </remarks>
        /// </summary>
        /// <param name="id">One or more comma-separated cryptocurrency CoinMarketCap IDs. Example: 1,2</param>
        /// <param name="slug"></param>
        /// <param name="symbol"></param>
        /// <param name="aux"></param>
        /// <returns>A map of cryptocurrency objects by ID, symbol, or slug (as used in query parameters).</returns>
        public ApiResponseMap<CryptocurrencyMetadata> Metadata(
            string id = null, string slug = null, string symbol = null, string aux = null)
        {
            if (string.IsNullOrWhiteSpace(id) &&
                string.IsNullOrWhiteSpace(slug) &&
                string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Must specify one of: {nameof(id)}, {nameof(slug)}, or {nameof(symbol)}",
                    nameof(id));
            }

            return ApiRequest<ApiResponseMap<CryptocurrencyMetadata>>("cryptocurrency/info",
                new Dictionary<string, string>
                {
                    ["id"] = id,
                    ["slug"] = slug,
                    ["symbol"] = symbol,
                    ["aux"] = aux
                });
        }

        #endregion Endpoint: /v1/cryptocurrency/info - Metadata

        #region Endpoint: /v1/cryptocurrency/listings/latest - Latest listings

        /// <summary>
        /// Returns a paginated list of all active cryptocurrencies with latest market data.
        ///
        /// <remarks>See https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyListingsLatest </remarks>
        /// </summary>
        /// <param name="start">
        /// Optionally offset the start (1-based index) of the paginated list of items to return.
        ///     Default: <value>1</value></param>
        ///     Valid values <value>&gt;= 1</value>
        /// <param name="limit">
        /// Optionally specify the number of results to return. Use this parameter and the
        /// <see cref="start"/> parameter to determine your own pagination size.
        ///     Default: <value>100</value>
        ///     Valid values <value>[ 1 .. 5000 ]</value>
        /// </param>
        /// <param name="priceMin">
        /// Optionally specify a threshold of minimum USD price to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="priceMax">
        /// Optionally specify a threshold of maximum USD price to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="marketCapMin">
        /// Optionally specify a threshold of minimum market cap to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="marketCapMax">
        /// Optionally specify a threshold of maximum market cap to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="volume24HMin">
        /// Optionally specify a threshold of minimum 24 hour USD volume to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="volume24HMax">
        /// Optionally specify a threshold of maximum 24 hour USD volume to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="circulatingSupplyMin">
        /// Optionally specify a threshold of minimum circulating supply to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="circulatingSupplyMax">
        /// Optionally specify a threshold of maximum circulating supply to filter results by.
        ///     Valid values <value>[ 0 .. 100000000000000000 ]</value>
        /// </param>
        /// <param name="percentChange24HMin">
        /// Optionally specify a threshold of minimum 24 hour percent change to filter results by.
        ///     Valid values <value>&gt;= -100</value>
        /// </param>
        /// <param name="percentChange24HMax">
        /// Optionally specify a threshold of maximum 24 hour percent change to filter results by.
        ///     Valid values <value>&gt;= -100</value>
        /// </param>
        /// <param name="convert">
        /// Optionally calculate market quotes in up to 120 currencies at once by passing a comma-separated
        /// list of cryptocurrency or fiat currency symbols. Each additional convert option beyond the first
        /// requires an additional call credit. A list of supported fiat options can be found at
        /// https://coinmarketcap.com/api/documentation/v1/#section/Standards-and-Conventions.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// </param>
        /// <param name="sort">
        /// What field to sort the list of cryptocurrencies by.
        ///     Default: <value>market_cap</value>
        ///     Valid values <value>name</value>, <value>symbol</value>,
        ///                      <value>date_added</value>, <value>market_cap</value>,
        ///                      <value>market_cap_strict</value>, <value>price</value>,
        ///                      <value>circulating_supply</value>, <value>total_supply</value>,
        ///                      <value>max_supply</value>, <value>num_market_pairs</value>,
        ///                      <value>volume_24h</value>, <value>percent_change_1h</value>,
        ///                      <value>percent_change_24h</value>, <value>percent_change_7d</value>,
        ///                      <value>market_cap_by_total_supply_strict</value>, <value>volume_7d</value>,
        ///                      <value>volume_30d</value>
        /// </param>
        /// <param name="sortDir">
        /// The direction in which to order cryptocurrencies against the specified sort.
        ///     Valid values: <value>asc</value>, <value>desc</value>
        /// </param>
        /// <param name="cryptocurrencyType">
        /// The type of cryptocurrency to include.
        ///     Default: <value>all</value>
        ///     Valid values:  <value>all</value>, <value>coins</value>, <value>tokens</value>
        /// </param>
        /// <param name="aux">
        /// Optionally specify a comma-separated list of supplemental data fields to return. Pass
        /// <value>num_market_pairs,cmc_rank,date_added,tags,platform,max_supply,circulating_supply,
        /// total_supply,market_cap_by_total_supply,volume_24h_reported,volume_7d,volume_7d_reported,
        /// volume_30d,volume_30d_reported</value> to include all auxiliary fields.
        ///     Default: <value>num_market_pairs,cmc_rank,date_added,tags,platform,max_supply,circulating_supply,total_supply</value>
        /// </param>
        /// <returns>Each conversion is returned in its own <see cref="Quote"/> object.</returns>
        public ApiResponseList<Cryptocurrency> ListingsLatest(
            int? start = 1, int? limit = 5000,
            long? priceMin = null, long? priceMax = null,
            long? marketCapMin = null, long? marketCapMax = null,
            long? volume24HMin = null, long? volume24HMax = null,
            long? circulatingSupplyMin = null, long? circulatingSupplyMax = null,
            double? percentChange24HMin = null, double? percentChange24HMax = null,
            string convert = null, string convertId = null,
            eSortCryptocurrencyListingsLatest? sort = null, eSortDir? sortDir = null,
            eCryptocurrencyType? cryptocurrencyType = null, string aux = null)
        {
            return ApiRequest<ApiResponseList<Cryptocurrency>>("cryptocurrency/listings/latest",
                new Dictionary<string, string>
                {
                    ["start"] = start?.ToString(),
                    ["limit"] = limit?.ToString(),
                    ["price_min"] = priceMin?.ToString(),
                    ["price_max"] = priceMax?.ToString(),
                    ["market_cap_min"] = marketCapMin?.ToString(),
                    ["market_cap_max"] = marketCapMax?.ToString(),
                    ["volume_24h_min"] = volume24HMin?.ToString(),
                    ["volume_24h_max"] = volume24HMax?.ToString(),
                    ["circulating_supply_min"] = circulatingSupplyMin?.ToString(),
                    ["circulating_supply_max"] = circulatingSupplyMax?.ToString(),
                    ["percent_change_24h_min"] = percentChange24HMin?.ToString(CultureInfo.InvariantCulture),
                    ["percent_change_24h_max"] = percentChange24HMax?.ToString(CultureInfo.InvariantCulture),
                    ["convert"] = convert,
                    ["convert_id"] = convertId,
                    ["sort"] = sort?.GetDescription(),
                    ["sort_dir"] = sortDir?.GetDescription(),
                    ["cryptocurrency_type"] = cryptocurrencyType?.GetDescription(),
                    ["aux"] = aux
                });
        }

        #endregion Endpoint: /v1/cryptocurrency/listings/latest - Latest listings

        #region Endpoint: /v1/cryptocurrency/listings/historical - Historical listings

        /// <summary>
        /// Returns a ranked and sorted list of all cryptocurrencies for a historical UTC date.
        /// <remarks>See https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyListingsHistorical </remarks>
        /// </summary>
        /// <param name="date">Date to reference day of snapshot.</param>
        /// <param name="start">
        /// Optionally offset the start (1-based index) of the paginated list of items to return.
        ///     Default: <value>1</value></param>
        ///     Valid values <value>&gt;= 1</value>
        /// <param name="limit">
        /// Optionally specify the number of results to return. Use this parameter and the
        /// <see cref="start"/> parameter to determine your own pagination size.
        ///     Default: <value>100</value>
        ///     Valid values <value>[ 1 .. 5000 ]</value>
        /// </param>        
        /// <param name="convert">
        /// Optionally calculate market quotes in up to 120 currencies at once by passing a comma-separated
        /// list of cryptocurrency or fiat currency symbols. Each additional convert option beyond the first
        /// requires an additional call credit. A list of supported fiat options can be found at
        /// https://coinmarketcap.com/api/documentation/v1/#section/Standards-and-Conventions.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// </param>
        /// <param name="sort">
        /// What field to sort the list of cryptocurrencies by.
        ///     Default: <value>cmc_rank</value>
        ///     Valid values: <value>cmc_rank</value>, <value>name</value>, <value>symbol</value>,
        ///                   <value>date_added</value>, <value>market_cap</value>,
        ///                   <value>price</value>,
        ///                   <value>circulating_supply</value>, <value>total_supply</value>,
        ///                   <value>max_supply</value>, 
        ///                   <value>volume_24h</value>, <value>percent_change_1h</value>,
        ///                   <value>percent_change_24h</value>, <value>percent_change_7d</value>,
        ///                   <value>market_cap_by_total_supply_strict</value>, <value>volume_7d</value>
        /// </param>
        /// <param name="sortDir">
        /// The direction in which to order cryptocurrencies against the specified sort.
        ///     Valid values: <value>asc</value>, <value>desc</value>
        /// </param>
        /// <param name="cryptocurrencyType">
        /// The type of cryptocurrency to include.
        ///     Default: <value>all</value>
        ///     Valid values:  <value>all</value>, <value>coins</value>, <value>tokens</value>
        /// </param>
        /// <param name="aux">
        /// Optionally specify a comma-separated list of supplemental data fields to return. Pass
        /// <value>platform,tags,date_added,circulating_supply,total_supply,max_supply,cmc_rank</value> to
        /// include all auxiliary fields.
        ///     Default: <value>platform,tags,date_added,circulating_supply,total_supply,max_supply,cmc_rank</value>
        /// </param>
        /// <returns>Each conversion is returned in its own <see cref="Quote"/> object.</returns>
        /// <returns></returns>
        public ApiResponseList<Cryptocurrency> ListingsHistorical(
            DateTime date, int? start = null, int? limit = null,
            string convert = null, string convertId = null,
            eSortCryptocurrencyListingsHistorical? sort = null, eSortDir? sortDir = null,
            eCryptocurrencyType? cryptocurrencyType = null, string aux = null)
        {
            return ApiRequest<ApiResponseList<Cryptocurrency>>("cryptocurrency/listings/historical",
                new Dictionary<string, string>
                {
                    ["date"] = date.ToString("yyyy-MM-dd"),
                    ["start"] = start?.ToString(),
                    ["limit"] = limit?.ToString(),
                    ["convert"] = convert,
                    ["convert_id"] = convertId,
                    ["sort"] = sort?.GetDescription(),
                    ["sort_dir"] = sortDir?.GetDescription(),
                    ["cryptocurrency_type"] = cryptocurrencyType?.GetDescription(),
                    ["aux"] = aux
                });
        }


        #endregion Endpoint: /v1/cryptocurrency/listings/historical - Historical listings

        #region Endpoint: /v1/cryptocurrency/quotes/latest - Latest quotes

        /// <summary>
        /// <see cref="QuotesLatest"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="convert"></param>
        /// <param name="convertId"></param>
        /// <param name="aux"></param>
        /// <param name="skipInvalid"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public ApiResponseMap<Cryptocurrency> QuotesLatestById(
            string id, string convert = null, string convertId = null,
            string aux = null, bool skipInvalid = false)
        {
            return QuotesLatest(id, null, null, convert, convertId, aux, skipInvalid);
        }

        /// <summary>
        /// <see cref="QuotesLatest"/>
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="convert"></param>
        /// <param name="convertId"></param>
        /// <param name="aux"></param>
        /// <param name="skipInvalid"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public ApiResponseMap<Cryptocurrency> QuotesLatestBySlug(
            string slug, string convert = null, string convertId = null,
            string aux = null, bool skipInvalid = false)
        {
            return QuotesLatest(null, slug, null, convert, convertId, aux, skipInvalid);
        }

        /// <summary>
        /// <see cref="QuotesLatest"/>
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="convert"></param>
        /// <param name="convertId"></param>
        /// <param name="aux"></param>
        /// <param name="skipInvalid"></param>
        /// <returns></returns>
        public ApiResponseMap<Cryptocurrency> QuotesLatestBySymbol(
            string symbol, string convert = null, string convertId = null,
            string aux = null, bool skipInvalid = false)
        {
            return QuotesLatest(null, null, symbol, convert, convertId, aux, skipInvalid);
        }

        /// <summary>
        /// Returns the latest market quote for 1 or more cryptocurrencies. Use the "convert" option to
        /// return market values in multiple fiat and cryptocurrency conversions in the same call.
        /// <remarks>See https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyQuotesLatest </remarks>
        /// </summary>
        /// <param name="id">One or more comma-separated cryptocurrency CoinMarketCap IDs. Example: 1,2</param>
        /// <param name="slug"></param>
        /// <param name="symbol"></param>
        /// <param name="convert"></param>
        /// <param name="convertId"></param>
        /// <param name="aux"></param>
        /// <param name="skipInvalid"></param>
        /// <returns>A map of cryptocurrency objects by ID, symbol, or slug (as used in query parameters).</returns>
        public ApiResponseMap<Cryptocurrency> QuotesLatest(
            string id = null, string slug = null, string symbol = null,
            string convert = null, string convertId = null, string aux = null,
            bool skipInvalid = false)
        {
            if (string.IsNullOrWhiteSpace(id) &&
                string.IsNullOrWhiteSpace(slug) &&
                string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Must specify one of: {nameof(id)}, {nameof(slug)}, or {nameof(symbol)}",
                    nameof(id));
            }

            return ApiRequest<ApiResponseMap<Cryptocurrency>>("cryptocurrency/quotes/latest",
                new Dictionary<string, string>
                {
                    ["id"] = id,
                    ["slug"] = slug,
                    ["symbol"] = symbol,
                    ["convert"] = convert,
                    ["convert_id"] = convertId,
                    ["aux"] = aux,
                    ["skip_invalid"] = skipInvalid ? "true" : string.Empty
                });
        }

        #endregion Endpoint: /v1/cryptocurrency/quotes/latest - Latest quotes

        #region Endpoint: /v1/cryptopcurrency/quotes/historical - Historical Quotes

        /// <summary>
        /// <see cref="QuotesHistorical"/>
        /// </summary>
        /// <param name="id">One or more comma-separated CoinMarketCap cryptocurrency IDs. Example: "1,2"</param>
        /// <param name="timeStart">Timestamp (Unix or ISO 8601) to start returning quotes for. Optional, if not passed, we'll return quotes calculated in reverse from "time_end".</param>
        /// <param name="timeEnd">Timestamp (Unix or ISO 8601) to stop returning quotes for (inclusive). Optional, if not passed, we'll default to the current time. If no "time_start" is passed, we return quotes in reverse order starting from this time.</param>
        /// <param name="count">The number of interval periods to return results for. Optional, required if both "time_start" and "time_end" aren't supplied. The default is 10 items.</param>
        /// <param name="interval">Interval of time to return data points for. If null, defaults to "5m".</param>
        /// <param name="convert">By default market quotes are returned in USD. Optionally calculate market quotes in up to 3 other fiat currencies or cryptocurrencies.</param>
        /// <param name="convertId">Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is identical to convert outside of ID format. Ex: convert_id=1,2781 would replace convert=BTC,USD in your query. This parameter cannot be used when convert is used.</param>
        /// <param name="aux">Specify a comma-separated list of supplemental data fields to return. Pass <value>price, volume, market_cap, quote_timestamp, search_interval</value> to include all auxiliary fields.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public ApiResponse<CryptocurrencyHistoricalData> QuotesHistoricalById(
            string id, DateTime? timeStart = null,
            DateTime? timeEnd = null, int? count = null, eInterval? interval = null, string convert = null,
            string convertId = null, string aux = null)
        {
            return QuotesHistorical(id, null, timeStart, timeEnd, count, interval, convert, convertId, aux);
        }

        /// <summary>
        /// <see cref="QuotesHistorical"/>
        /// </summary>
        /// <param name="symbol">Alternatively pass one or more comma-separated cryptocurrency symbols. Example: "BTC,ETH". At least one "id" or "symbol" is required for this request.</param>
        /// <param name="timeStart">Timestamp (Unix or ISO 8601) to start returning quotes for. Optional, if not passed, we'll return quotes calculated in reverse from "time_end".</param>
        /// <param name="timeEnd">Timestamp (Unix or ISO 8601) to stop returning quotes for (inclusive). Optional, if not passed, we'll default to the current time. If no "time_start" is passed, we return quotes in reverse order starting from this time.</param>
        /// <param name="count">The number of interval periods to return results for. Optional, required if both "time_start" and "time_end" aren't supplied. The default is 10 items.</param>
        /// <param name="interval">Interval of time to return data points for. If null, defaults to "5m".</param>
        /// <param name="convert">By default market quotes are returned in USD. Optionally calculate market quotes in up to 3 other fiat currencies or cryptocurrencies.</param>
        /// <param name="convertId">Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is identical to convert outside of ID format. Ex: convert_id=1,2781 would replace convert=BTC,USD in your query. This parameter cannot be used when convert is used.</param>
        /// <param name="aux">Specify a comma-separated list of supplemental data fields to return. Pass <value>price, volume, market_cap, quote_timestamp, search_interval</value> to include all auxiliary fields.</param>
        /// <returns></returns>
        public ApiResponse<CryptocurrencyHistoricalData> QuotesHistoricalBySymbol(
           string symbol, DateTime? timeStart = null, DateTime? timeEnd = null,
           int? count = null, eInterval? interval = null, string convert = null,
           string convertId = null, string aux = null)
        {
            return QuotesHistorical(null, symbol, timeStart, timeEnd, count, interval, convert, convertId, aux);
        }

        /// <summary>
        /// Returns an interval of historic market quotes for any cryptocurrency based on time and interval parameters.
        /// <remarks>See https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyQuotesHistorical </remarks>
        /// </summary>
        /// <param name="id">One or more comma-separated cryptocurrency CoinMarketCap IDs. Example: 1,2</param>
        /// <param name="symbol"></param>
        /// <param name="timeStart">Timestamp(Unix or ISO 8601) to start returning quotes for. Optional, if not passed, returns quotes calculated in reverse from "time_end".</param>
        /// <param name="timeEnd">Timestamp (Unix or ISO 8601) to stop returning quotes for (inclusive). Optional, if not passed, defaults to the current time. If no "time_start" is passed, we return quotes in reverse order starting from this time.</param>
        /// <param name="count">The number of interval periods to return results for. Optional, required if both "time_start" and "time_end" aren't supplied. The default is 10 items.</param>
        /// <param name="interval">Interval of time to return data points for. If null, defaults to "5m".</param>
        /// <param name="convert">By default market quotes are returned in USD. Optionally calculate market quotes in up to 3 other fiat currencies or cryptocurrencies.</param>
        /// <param name="convertId">Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is identical to convert outside of ID format. Ex: convert_id=1,2781 would replace convert=BTC,USD in your query. This parameter cannot be used when convert is used.</param>
        /// <param name="aux">Specify a comma-separated list of supplemental data fields to return. Pass <value>price, volume, market_cap, quote_timestamp, search_interval</value> to include all auxiliary fields.</param>
        /// <returns>Results of your query returned as an object map.</returns>
        public ApiResponse<CryptocurrencyHistoricalData> QuotesHistorical(
            string id = null, string symbol = null, DateTime? timeStart = null,
            DateTime? timeEnd = null, int? count = null, eInterval? interval = null,
            string convert = null, string convertId = null, string aux = null)
        {
            if (string.IsNullOrWhiteSpace(id) &&
                string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Must specify one of: {nameof(id)}, or {nameof(symbol)}",
                    nameof(id));
            }

            return ApiRequest<ApiResponse<CryptocurrencyHistoricalData>>("cryptocurrency/quotes/historical",
                new Dictionary<string, string>
                {
                    ["id"] = id,
                    ["symbol"] = symbol,
                    ["time_start"] = timeStart?.ToString("yyyy-MM-dd"),
                    ["time_end"] = timeEnd?.ToString("yyyy-MM-dd"),
                    ["count"] = count?.ToString(),
                    ["interval"] = interval?.GetDescription(),
                    ["convert"] = convert,
                    ["convert_id"] = convertId,
                    ["aux"] = aux
                });
        }

        #endregion Endpoint: /v1/cryptopcurrency/quotes/historical - Historical Quotes

        #region Endpoint: /v1/cryptocurrency/market-pairs/latest - Market Pairs Latest

        /// <summary>
        /// <see cref="MarketPairsLatest"/>
        /// </summary>
        /// <param name="id">A cryptocurrency or fiat currency by CoinMarketCap ID to list market pairs for. 
        /// Example: <value>1</value></param>
        /// <param name="start">Optionally offset the start (1-based index) of the paginated list of items to return.
        ///     Default: <value>1</value>
        ///     Valid values <value>&gt;= 1</value></param>
        /// <param name="limit">Optionally specify the number of results to return. Use this parameter and the 
        /// <see cref="start"/> parameter to determine your own pagination size.
        ///     Default: <value>100</value>
        ///     Valid values <value>[ 1 .. 5000 ]</value>
        /// </param>
        /// <param name="sortDir">Optionally specify the sort direction of markets returned. 
        ///     Default is <value>desc</value>.
        /// </param>
        /// <param name="sort">Optionally specify the sort order of markets returned. By default we return a strict sort on
        /// 24 hour reported volume. Pass <value>cmc_rank</value> to return a CMC methodology based sort where markets 
        /// with excluded volumes are returned last.
        ///     Default: <value>volume_24h_strict</value>
        ///     Valid values: <value>volume_24h_strict</value>, <value>cmc_rank</value>
        /// </param>
        /// <param name="aux">Optionally specify a comma-separated list of supplemental data fields to return. 
        /// Pass <value>num_market_pairs, category, fee_type, market_url, currency_name, currency_slug, price_quote, 
        /// notice</value> to include all auxiliary fields.
        ///     Default: <value>num_market_pairs</value>, <value>category</value>, <value>fee_type</value>
        ///</param>
        /// <param name="matchedId">Optionally include one or more fiat or cryptocurrency IDs to filter market pairs by. 
        /// For example: <value>?id=1&amp;matched_id=2781</value> would only return BTC markets that matched: 
        /// <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedSymbol"/> 
        /// is used.</param>
        /// <param name="matchedSymbol">Optionally include one or more fiat or cryptocurrency symbols to filter market pairs by. 
        /// For example: <value>?symbol=BTC&amp;matched_symbol=USD</value> would only return BTC markets that matched: 
        /// <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedId"/> is used.
        /// </param>
        /// <param name="convert">Optionally calculate market quotes in up to 120 currencies at once by passing a comma-separated 
        /// list of cryptocurrency or fiat currency symbols. Each additional convert option beyond the first requires an additional 
        /// call credit. A list of supported fiat options can be found at 
        /// https://sandbox.coinmarketcap.com/api/v1/#section/Standards-and-Conventions. 
        /// Each conversion is returned in its own <value>quote</value> object.</param>
        /// <param name="convertId">Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is 
        /// identical to <see cref="convert"/> outside of ID format. Ex: convert_id=1,2781 would replace convert=BTC,USD in your 
        /// query. This parameter cannot be used when <see cref="convert"/> is used.</param>
        /// <returns></returns>
        public ApiResponse<CryptocurrencyMarketPairs> MarketPairsLatestById(
            string id, int? start = 1, int? limit = null,
            eSortDir? sortDir = null, eSortCryptocurrencyMarketPairsLatest? sort = null, string aux = null,
            string matchedId = null, string matchedSymbol = null,
            string convert = null, string convertId = null)
        {
            return MarketPairsLatest(id, null, null, start, limit, sortDir, sort, aux, matchedId, matchedSymbol, convert, convertId);
        }

        /// <summary>
        /// <see cref="MarketPairsLatest"/>
        /// </summary>
        /// <param name="slug">Alternatively pass a cryptocurrency by slug. Example: <value>bitcoin</value></param>
        /// <param name="start">Optionally offset the start (1-based index) of the paginated list of items to return.
        ///     Default: <value>1</value>
        ///     Valid values <value>&gt;= 1</value></param>
        /// <param name="limit">Optionally specify the number of results to return. Use this parameter and the 
        /// <see cref="start"/> parameter to determine your own pagination size.
        ///     Default: <value>100</value>
        ///     Valid values <value>[ 1 .. 5000 ]</value>
        /// </param>
        /// <param name="sortDir">Optionally specify the sort direction of markets returned. 
        ///     Default is <value>desc</value>.
        /// </param>
        /// <param name="sort">Optionally specify the sort order of markets returned. By default we return a strict sort on
        /// 24 hour reported volume. Pass <value>cmc_rank</value> to return a CMC methodology based sort where markets 
        /// with excluded volumes are returned last.
        ///     Default: <value>volume_24h_strict</value>
        ///     Valid values: <value>volume_24h_strict</value>, <value>cmc_rank</value>
        /// </param>
        /// <param name="aux">Optionally specify a comma-separated list of supplemental data fields to return. 
        /// Pass <value>num_market_pairs, category, fee_type, market_url, currency_name, currency_slug, price_quote, 
        /// notice</value> to include all auxiliary fields.
        ///     Default: <value>num_market_pairs</value>, <value>category</value>, <value>fee_type</value>
        ///</param>
        /// <param name="matchedId">Optionally include one or more fiat or cryptocurrency IDs to filter market pairs by. 
        /// For example: <value>?id=1&amp;matched_id=2781</value> would only return BTC markets that matched: 
        /// <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedSymbol"/> 
        /// is used.</param>
        /// <param name="matchedSymbol">Optionally include one or more fiat or cryptocurrency symbols to filter market pairs by. 
        /// For example: <value>?symbol=BTC&amp;matched_symbol=USD</value> would only return BTC markets that matched: 
        /// <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedId"/> is used.
        /// </param>
        /// <param name="convert">Optionally calculate market quotes in up to 120 currencies at once by passing a comma-separated 
        /// list of cryptocurrency or fiat currency symbols. Each additional convert option beyond the first requires an additional 
        /// call credit. A list of supported fiat options can be found at 
        /// https://sandbox.coinmarketcap.com/api/v1/#section/Standards-and-Conventions. 
        /// Each conversion is returned in its own <value>quote</value> object.</param>
        /// <param name="convertId">Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is 
        /// identical to <see cref="convert"/> outside of ID format. Ex: convert_id=1,2781 would replace convert=BTC,USD in your 
        /// query. This parameter cannot be used when <see cref="convert"/> is used.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public ApiResponse<CryptocurrencyMarketPairs> MarketPairsLatestBySlug(
            string slug, int? start = 1, int? limit = null,
            eSortDir? sortDir = null, eSortCryptocurrencyMarketPairsLatest? sort = null, string aux = null,
            string matchedId = null, string matchedSymbol = null,
            string convert = null, string convertId = null)
        {
            return MarketPairsLatest(null, slug, null, start, limit, sortDir, sort, aux, matchedId, matchedSymbol, convert, convertId);
        }

        /// <summary>
        /// <see cref="MarketPairsLatest"/>
        /// </summary>
        /// <param name="symbol">Alternatively pass a cryptocurrency by symbol. Fiat currencies are not supported by this field.
        /// Example: <value>BTC</value>. A single cryptocurrency <value>id</value>, <value>slug</value>, or <value>symbol</value> 
        /// is required.</param>
        /// <param name="start">Optionally offset the start (1-based index) of the paginated list of items to return.
        ///     Default: <value>1</value>
        ///     Valid values <value>&gt;= 1</value></param>
        /// <param name="limit">Optionally specify the number of results to return. Use this parameter and the 
        /// <see cref="start"/> parameter to determine your own pagination size.
        ///     Default: <value>100</value>
        ///     Valid values <value>[ 1 .. 5000 ]</value>
        /// </param>
        /// <param name="sortDir">Optionally specify the sort direction of markets returned. 
        ///     Default is <value>desc</value>.
        /// </param>
        /// <param name="sort">Optionally specify the sort order of markets returned. By default we return a strict sort on
        /// 24 hour reported volume. Pass <value>cmc_rank</value> to return a CMC methodology based sort where markets 
        /// with excluded volumes are returned last.
        ///     Default: <value>volume_24h_strict</value>
        ///     Valid values: <value>volume_24h_strict</value>, <value>cmc_rank</value>
        /// </param>
        /// <param name="aux">Optionally specify a comma-separated list of supplemental data fields to return. 
        /// Pass <value>num_market_pairs, category, fee_type, market_url, currency_name, currency_slug, price_quote, 
        /// notice</value> to include all auxiliary fields.
        ///     Default: <value>num_market_pairs</value>, <value>category</value>, <value>fee_type</value>
        ///</param>
        /// <param name="matchedId">Optionally include one or more fiat or cryptocurrency IDs to filter market pairs by. 
        /// For example: <value>?id=1&amp;matched_id=2781</value> would only return BTC markets that matched: 
        /// <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedSymbol"/> 
        /// is used.</param>
        /// <param name="matchedSymbol">Optionally include one or more fiat or cryptocurrency symbols to filter market pairs by. 
        /// For example: <value>?symbol=BTC&amp;matched_symbol=USD</value> would only return BTC markets that matched: 
        /// <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedId"/> is used.
        /// </param>
        /// <param name="convert">Optionally calculate market quotes in up to 120 currencies at once by passing a comma-separated 
        /// list of cryptocurrency or fiat currency symbols. Each additional convert option beyond the first requires an additional 
        /// call credit. A list of supported fiat options can be found at 
        /// https://sandbox.coinmarketcap.com/api/v1/#section/Standards-and-Conventions. 
        /// Each conversion is returned in its own <value>quote</value> object.</param>
        /// <param name="convertId">Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is 
        /// identical to <see cref="convert"/> outside of ID format. Ex: convert_id=1,2781 would replace convert=BTC,USD in your 
        /// query. This parameter cannot be used when <see cref="convert"/> is used.</param>
        /// <returns></returns>
        public ApiResponse<CryptocurrencyMarketPairs> MarketPairsLatestBySymbol(
            string symbol, int? start = 1, int? limit = null,
            eSortDir? sortDir = null, eSortCryptocurrencyMarketPairsLatest? sort = null, string aux = null,
            string matchedId = null, string matchedSymbol = null,
            string convert = null, string convertId = null)
        {
            return MarketPairsLatest(null, null, symbol, start, limit, sortDir, sort, aux, matchedId, matchedSymbol, convert, convertId);
        }

        ///  <summary>
        ///  Lists all active market pairs that CoinMarketCap tracks for a given cryptocurrency or fiat currency. All markets with this
        ///  currency as the pair base or pair quote will be returned. The latest price and volume information is returned for each
        ///  market. Use the <see cref="convert"/> option to return market values in multiple fiat and cryptocurrency conversions in the
        ///  same call.
        ///  <remarks>See https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyMarketpairsLatest </remarks>
        ///  </summary>
        ///  <param name="id">A cryptocurrency or fiat currency by CoinMarketCap ID to list market pairs for. 
        ///  Example: <value>1</value></param>
        ///  <param name="symbol"></param>
        ///  <param name="start">Optionally offset the start (1-based index) of the paginated list of items to return.
        ///      Default: <value>1</value>
        ///      Valid values <value>&gt;= 1</value></param>
        ///  <param name="limit">Optionally specify the number of results to return. Use this parameter and the 
        ///  <see cref="start"/> parameter to determine your own pagination size.
        ///      Default: <value>100</value>
        ///      Valid values <value>[ 1 .. 5000 ]</value>
        ///  </param>
        ///  <param name="sortDir">Optionally specify the sort direction of markets returned. 
        ///      Default is <value>desc</value>.
        ///  </param>
        ///  <param name="sort">Optionally specify the sort order of markets returned. By default we return a strict sort on
        ///  24 hour reported volume. Pass <value>cmc_rank</value> to return a CMC methodology based sort where markets 
        ///  with excluded volumes are returned last.
        ///      Default: <value>volume_24h_strict</value>
        ///      Valid values: <value>volume_24h_strict</value>, <value>cmc_rank</value>
        ///  </param>
        ///  <param name="aux">Optionally specify a comma-separated list of supplemental data fields to return. 
        ///  Pass <value>num_market_pairs, category, fee_type, market_url, currency_name, currency_slug, price_quote, 
        ///  notice</value> to include all auxiliary fields.
        ///      Default: <value>num_market_pairs</value>, <value>category</value>, <value>fee_type</value>
        /// </param>
        ///  <param name="matchedId">Optionally include one or more fiat or cryptocurrency IDs to filter market pairs by. 
        ///  For example: <value>?id=1&amp;matched_id=2781</value> would only return BTC markets that matched: 
        ///  <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedSymbol"/> 
        ///  is used.</param>
        ///  <param name="matchedSymbol">Optionally include one or more fiat or cryptocurrency symbols to filter market pairs by. 
        ///  For example: <value>?symbol=BTC&amp;matched_symbol=USD</value> would only return BTC markets that matched: 
        ///  <value>BTC/USD</value> or <value>USD/BTC</value>. This parameter cannot be used when <see cref="matchedId"/> is used.
        ///  </param>
        ///  <param name="convert">Optionally calculate market quotes in up to 120 currencies at once by passing a comma-separated 
        ///  list of cryptocurrency or fiat currency symbols. Each additional convert option beyond the first requires an additional 
        ///  call credit. A list of supported fiat options can be found at 
        ///  https://sandbox.coinmarketcap.com/api/v1/#section/Standards-and-Conventions. 
        ///  Each conversion is returned in its own <value>quote</value> object.</param>
        ///  <param name="convertId">Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is 
        ///  identical to <see cref="convert"/> outside of ID format. Ex: convert_id=1,2781 would replace convert=BTC,USD in your 
        ///  query. This parameter cannot be used when <see cref="convert"/> is used.</param>
        ///  <returns>Results of your query returned as an object.</returns>
        public ApiResponse<CryptocurrencyMarketPairs> MarketPairsLatest(
            string id = null, string slug = null, string symbol = null,
            int? start = 1, int? limit = null, eSortDir? sortDir = null,
            eSortCryptocurrencyMarketPairsLatest? sort = null, string aux = null, string matchedId = null,
            string matchedSymbol = null, string convert = null,
            string convertId = null)
        {
            if (string.IsNullOrWhiteSpace(id) &&
                string.IsNullOrWhiteSpace(slug) &&
                string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Must specify one of: {nameof(id)}, {nameof(slug)}, or {nameof(symbol)}",
                    nameof(id));
            }

            return ApiRequest<ApiResponse<CryptocurrencyMarketPairs>>("cryptocurrency/market-pairs/latest",
                new Dictionary<string, string>
                {
                    ["id"] = id,
                    ["slug"] = slug,
                    ["symbol"] = symbol,
                    ["start"] = start?.ToString(),
                    ["limit"] = limit?.ToString(),
                    ["sort_dir"] = sortDir?.GetDescription(),
                    ["sort"] = sort?.GetDescription(),
                    ["aux"] = aux,
                    ["matched_id"] = matchedId,
                    ["matched_symbol"] = matchedSymbol,
                    ["convert"] = convert,
                    ["convert_id"] = convertId
                });
        }

        #endregion Endpoint: /v1/cryptocurrency/market-pairs/latest - Market Pairs Latest

        #region Endpoint: /v1/cryptocurrency/ohlcv/latest - Latest OHLCV

        public ApiResponseMap<CryptocurrencyOhlcvQuotes> OhlcvLatestById(
            string id, string convert = null, string convertId = null, bool? skipInvalid = null)
        {
            return OhlcvLatest(id, null, convert, convertId, skipInvalid);
        }

        // ReSharper disable once UnusedMember.Global
        public ApiResponseMap<CryptocurrencyOhlcvQuotes> OhlcvLatestBySymbol(
            string symbol, string convert = null, string convertId = null, bool? skipInvalid = null)
        {
            return OhlcvLatest(null, symbol, convert, convertId, skipInvalid);
        }

        /// <summary>
        /// Returns the latest OHLCV (Open, High, Low, Close, Volume) market values for one or more
        /// cryptocurrencies for the current UTC day. Since the current UTC day is still active these values
        /// are updated frequently. You can find the final calculated OHLCV values for the last completed UTC
        /// day along with all historic days using <see cref="OhlcvHistorical"/>.
        /// </summary>
        /// <param name="id">
        /// One or more comma-separated cryptocurrency CoinMarketCap IDs.
        /// Example: <value>a,2</value>
        /// </param>
        /// <param name="symbol">
        /// Alternatively pass one or more comma-separated cryptocurrency symbols. Example: <value>BTC,ETH</value>.
        /// At least one of <see cref="id"/> or <see cref="symbol"/> is required
        /// </param>
        /// <param name="convert">
        /// Optionally calculate market quotes in up to 120 currencies at once by passing a comma-separated
        /// list of cryptocurrency or fiat currency symbols. Each additional convert option beyond the first
        /// requires an additional call credit. A list of supported fiat options can be found at
        /// https://coinmarketcap.com/api/documentation/v1/#section/Standards-and-Conventions
        /// Each conversion is returned in its own <see cref="OhlcvQuote"/> object.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/>outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// </param>
        /// <param name="skipInvalid">
        /// Pass <value>true</value> to relax request validation rules. When requesting records on multiple
        /// cryptocurrencies an error is returned if any invalid cryptocurrencies are requested or a
        /// cryptocurrency does not have matching records in the requested timeframe. If set to true, invalid
        /// lookups will be skipped allowing valid cryptocurrencies to still be returned.
        /// </param>
        /// <returns>Latest OHLCV (Open, High, Low, Close, Volume) market values for the specified cryptocurrencies.</returns>
        public ApiResponseMap<CryptocurrencyOhlcvQuotes> OhlcvLatest(
            string id = null, string symbol = null, string convert = null, string convertId = null,
            bool? skipInvalid = null)
        {
            if (string.IsNullOrWhiteSpace(id) &&
                string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Must specify one of: {nameof(id)}, or {nameof(symbol)}",
                    nameof(id));
            }

            return ApiRequest<ApiResponseMap<CryptocurrencyOhlcvQuotes>>("cryptocurrency/ohlcv/latest",
                new Dictionary<string, string>
                {
                    ["id"] = id,
                    ["symbol"] = symbol,
                    ["convert"] = convert,
                    ["convert_id"] = convertId,
                    ["skip_invalid"] = skipInvalid.HasValue && skipInvalid.Value ? "true" : string.Empty
                });
        }

        #endregion Endpoint: /v1/cryptocurrency/ohlcv/latest - Latest OHLCV

        #region Endpoint: /v1/cryptocurrency/ohlcv/historical - Historical OHLCV


        /// <summary>
        /// <see cref="OhlcvHistorical"/>
        /// </summary>
        /// <param name="id">
        /// One or more comma-separated cryptocurrency CoinMarketCap IDs.
        /// Example: <value>a,2</value>
        /// </param>
        /// <param name="timePeriod">
        /// Time period to return OHLCV data for.
        ///     Default: <value>daily</value>
        ///     Valid values: <see cref="eTimePeriodOhlcvHistorical"/>
        ///     See https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical for complete details.
        /// </param>
        /// <param name="timeStart">
        /// Timestamp (Unix or ISO 8601) to start returning OHLCV time periods for. Only the date portion of the timestamp is
        /// used for daily OHLCV so it's recommended to send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="timeEnd">
        /// Timestamp (Unix or ISO 8601) to stop returning OHLCV time periods for (inclusive). Optional, if not passed we'll
        /// default to the current time. Only the date portion of the timestamp is used for daily OHLCV so it's recommended to
        /// send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="count">
        /// Optionally limit the number of time periods to return results for.
        ///     Default: <value>10</value>
        ///     Valid values: <value>[1 .. 10000]</value>
        /// </param>
        /// <param name="interval">
        /// Optionally adjust the interval that <see cref="time_period"/> is sampled. <see cref="eIntervalOhlcvHistorical"/> for
        /// available options.
        /// </param>
        /// <param name="convert">
        /// By default market quotes are returned in <value>USD</value>. Optionally calculate market quotes in up to 3 fiat
        /// currencies or cryptocurrencies.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// </param>
        /// <param name="skipInvalid">
        /// Pass <value>true</value> to relax request validation rules. When requesting records on multiple
        /// cryptocurrencies an error is returned if any invalid cryptocurrencies are requested or a
        /// cryptocurrency does not have matching records in the requested timeframe. If set to true, invalid
        /// lookups will be skipped allowing valid cryptocurrencies to still be returned. Note: This parameter is only used in the
        /// https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical version.
        /// </param>
        /// <returns></returns>
        public ApiResponse<CryptocurrencyOhlcvHistorical> OhlcvHistoricalById(
            string id, eTimePeriodOhlcvHistorical? timePeriod = null,
            DateTime? timeStart = null, DateTime? timeEnd = null,
            int? count = null, eIntervalOhlcvHistorical? interval = null,
            string convert = null, string convertId = null, bool? skipInvalid = null)
        {
            return OhlcvHistorical(id, null, null, timePeriod, timeStart, timeEnd, count, interval, convert, convertId, skipInvalid);
        }

        /// <summary>
        /// <see cref="OhlcvHistorical"/>
        /// </summary>
        /// <param name="slug">
        /// Alternatively pass a comma-separated list of cryptocurrency slugs. Example: <value>bitcoin</value>, 
        /// <value>ethereum</value>
        /// </param>
        /// <param name="timePeriod">
        /// Time period to return OHLCV data for.
        ///     Default: <value>daily</value>
        ///     Valid values: <see cref="eTimePeriodOhlcvHistorical"/>
        ///     See https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical for complete details.
        /// </param>
        /// <param name="timeStart">
        /// Timestamp (Unix or ISO 8601) to start returning OHLCV time periods for. Only the date portion of the timestamp is
        /// used for daily OHLCV so it's recommended to send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="timeEnd">
        /// Timestamp (Unix or ISO 8601) to stop returning OHLCV time periods for (inclusive). Optional, if not passed we'll
        /// default to the current time. Only the date portion of the timestamp is used for daily OHLCV so it's recommended to
        /// send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="count">
        /// Optionally limit the number of time periods to return results for.
        ///     Default: <value>10</value>
        ///     Valid values: <value>[1 .. 10000]</value>
        /// </param>
        /// <param name="interval">
        /// Optionally adjust the interval that <see cref="time_period"/> is sampled. <see cref="eIntervalOhlcvHistorical"/> for
        /// available options.
        /// </param>
        /// <param name="convert">
        /// By default market quotes are returned in <value>USD</value>. Optionally calculate market quotes in up to 3 fiat
        /// currencies or cryptocurrencies.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// </param>
        /// <param name="skipInvalid">
        /// Pass <value>true</value> to relax request validation rules. When requesting records on multiple
        /// cryptocurrencies an error is returned if any invalid cryptocurrencies are requested or a
        /// cryptocurrency does not have matching records in the requested timeframe. If set to true, invalid
        /// lookups will be skipped allowing valid cryptocurrencies to still be returned. Note: This parameter is only used in the
        /// https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical version.
        /// </param>
        /// <returns></returns>
        public ApiResponse<CryptocurrencyOhlcvHistorical> OhlcvHistoricalBySlug(
        string slug, eTimePeriodOhlcvHistorical? timePeriod = null,
        DateTime? timeStart = null, DateTime? timeEnd = null,
        int? count = null, eIntervalOhlcvHistorical? interval = null,
        string convert = null, string convertId = null, bool? skipInvalid = null)
        {
            return OhlcvHistorical(null, slug, null, timePeriod, timeStart, timeEnd, count, interval, convert, convertId, skipInvalid);
        }

        /// <summary>
        /// <see cref="OhlcvHistorical"/>
        /// </summary>
        /// <param name="symbol">
        /// Alternatively pass one or more comma-separated cryptocurrency symbols. Example: <value>BTC,ETH</value>.
        /// At least one of <see cref="id"/>, <see cref="slug"/>, or <see cref="symbol"/> is required.
        /// </param>
        /// <param name="timePeriod">
        /// Time period to return OHLCV data for.
        ///     Default: <value>daily</value>
        ///     Valid values: <see cref="eTimePeriodOhlcvHistorical"/>
        ///     See https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical for complete details.
        /// </param>
        /// <param name="timeStart">
        /// Timestamp (Unix or ISO 8601) to start returning OHLCV time periods for. Only the date portion of the timestamp is
        /// used for daily OHLCV so it's recommended to send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="timeEnd">
        /// Timestamp (Unix or ISO 8601) to stop returning OHLCV time periods for (inclusive). Optional, if not passed we'll
        /// default to the current time. Only the date portion of the timestamp is used for daily OHLCV so it's recommended to
        /// send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="count">
        /// Optionally limit the number of time periods to return results for.
        ///     Default: <value>10</value>
        ///     Valid values: <value>[1 .. 10000]</value>
        /// </param>
        /// <param name="interval">
        /// Optionally adjust the interval that <see cref="time_period"/> is sampled. <see cref="eIntervalOhlcvHistorical"/> for
        /// available options.
        /// </param>
        /// <param name="convert">
        /// By default market quotes are returned in <value>USD</value>. Optionally calculate market quotes in up to 3 fiat
        /// currencies or cryptocurrencies.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// </param>
        /// <param name="skipInvalid">
        /// Pass <value>true</value> to relax request validation rules. When requesting records on multiple
        /// cryptocurrencies an error is returned if any invalid cryptocurrencies are requested or a
        /// cryptocurrency does not have matching records in the requested timeframe. If set to true, invalid
        /// lookups will be skipped allowing valid cryptocurrencies to still be returned. Note: This parameter is only used in the
        /// https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical version.
        /// </param>
        /// <returns></returns>
        public ApiResponse<CryptocurrencyOhlcvHistorical> OhlcvHistoricalBySymbol(
        string symbol, eTimePeriodOhlcvHistorical? timePeriod = null,
        DateTime? timeStart = null, DateTime? timeEnd = null,
        int? count = null, eIntervalOhlcvHistorical? interval = null,
        string convert = null, string convertId = null, bool? skipInvalid = null)
        {
            return OhlcvHistorical(null, null, symbol, timePeriod, timeStart, timeEnd, count, interval, convert, convertId, skipInvalid);
        }

        /// <summary>
        /// Returns historical OHLCV (Open, High, Low, Close, Volume) data along with market cap for any cryptocurrency using
        /// time interval parameters. Currently daily and hourly OHLCV periods are supported. Volume is only supported with
        /// daily periods at this time.
        /// </summary>
        /// <param name="id">
        /// One or more comma-separated cryptocurrency CoinMarketCap IDs.
        /// Example: <value>a,2</value>
        /// </param>
        /// <param name="slug">
        /// Alternatively pass a comma-separated list of cryptocurrency slugs. Example: <value>bitcoin</value>, 
        /// <value>ethereum</value>
        /// </param>
        /// <param name="symbol">
        /// Alternatively pass one or more comma-separated cryptocurrency symbols. Example: <value>BTC,ETH</value>.
        /// At least one of <see cref="id"/>, <see cref="slug"/>, or <see cref="symbol"/> is required.
        /// </param>
        /// <param name="timePeriod">
        /// Time period to return OHLCV data for.
        ///     Default: <value>daily</value>
        ///     Valid values: <see cref="eTimePeriodOhlcvHistorical"/>
        ///     See https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical for complete details.
        /// </param>
        /// <param name="timeStart">
        /// Timestamp (Unix or ISO 8601) to start returning OHLCV time periods for. Only the date portion of the timestamp is
        /// used for daily OHLCV so it's recommended to send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="timeEnd">
        /// Timestamp (Unix or ISO 8601) to stop returning OHLCV time periods for (inclusive). Optional, if not passed we'll
        /// default to the current time. Only the date portion of the timestamp is used for daily OHLCV so it's recommended to
        /// send an ISO date format like <value>2018-09-19</value> without time.
        /// </param>
        /// <param name="count">
        /// Optionally limit the number of time periods to return results for.
        ///     Default: <value>10</value>
        ///     Valid values: <value>[1 .. 10000]</value>
        /// </param>
        /// <param name="interval">
        /// Optionally adjust the interval that <see cref="time_period"/> is sampled. <see cref="eIntervalOhlcvHistorical"/> for
        /// available options.
        /// </param>
        /// <param name="convert">
        /// By default market quotes are returned in <value>USD</value>. Optionally calculate market quotes in up to 3 fiat
        /// currencies or cryptocurrencies.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// </param>
        /// <param name="skipInvalid">
        /// Pass <value>true</value> to relax request validation rules. When requesting records on multiple
        /// cryptocurrencies an error is returned if any invalid cryptocurrencies are requested or a
        /// cryptocurrency does not have matching records in the requested timeframe. If set to true, invalid
        /// lookups will be skipped allowing valid cryptocurrencies to still be returned. Note: This parameter is only used in the
        /// https://coinmarketcap.com/api/v1/#operation/getV1CryptocurrencyOhlcvHistorical version.
        /// </param>
        /// <returns>Results of your query returned as an object.</returns>
        public ApiResponse<CryptocurrencyOhlcvHistorical> OhlcvHistorical(
        string id = null, string slug = null,
        string symbol = null, eTimePeriodOhlcvHistorical? timePeriod = null,
        DateTime? timeStart = null, DateTime? timeEnd = null,
        int? count = null, eIntervalOhlcvHistorical? interval = null,
        string convert = null, string convertId = null, bool? skipInvalid = null)
        {
            if (string.IsNullOrWhiteSpace(id) &&
               string.IsNullOrWhiteSpace(slug) &&
               string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Must specify one of: {nameof(id)}, {nameof(slug)}, or {nameof(symbol)}",
                    nameof(id));
            }

            return ApiRequest<ApiResponse<CryptocurrencyOhlcvHistorical>>("cryptocurrency/ohlcv/historical",
               new Dictionary<string, string>
               {
                   ["id"] = id,
                   ["slug"] = slug,
                   ["symbol"] = symbol,
                   ["time_period"] = timePeriod?.GetDescription(),
                   ["time_start"] = timeStart?.ToString("yyyy-MM-dd"),
                   ["time_end"] = timeEnd?.ToString("yyyy-MM-dd"),
                   ["count"] = count?.ToString(),
                   ["interval"] = interval?.GetDescription(),
                   ["convert"] = convert,
                   ["convert_id"] = convertId,
                   ["skip_invalid"] = skipInvalid.HasValue && skipInvalid.Value ? "true" : string.Empty
               });
        }

        #endregion Endpoint: /v1/cryptocurrency/ohlcv/historical - Historical OHLCV

        #region Endpoint: /v1/cryptocurrency/price-performance-stats/latest - Price Performance Stats

        /// <summary>
        /// <see cref="PricePerformanceStats"/>
        /// </summary>
        /// <param name="id">
        /// One or more comma-separated cryptocurrency CoinMarketCap IDs.
        /// Example: <value>a,2</value>
        /// </param>
        /// <param name="timePeriod">
        /// Specify one or more comma-delimited time periods to return stats for. 
        ///     Default: <value>all_time</value>
        ///     Pass all <see cref="eTimePeriodPricePerformanceStats"/> to return all supported time periods.
        /// All rolling periods have a rolling close time of the current request time. For example <value>24h</value> would have a
        /// close time of now and an open time of 24 hours before now. Please note: <value>yesterday</value> is a UTC period and
        /// currently does not currently support <value>high</value> and <value>low</value> timestamps.
        /// </param>
        /// <param name="convert">
        /// Optionally calculate quotes in up to 120 currencies at once by passing a comma-separated list of cryptocurrency or fiat
        /// currency symbols. Each additional convert option beyond the first requires an additional call credit. A list of
        /// supported fiat options can be found at https://coinmarketcap.com/api/documentation/v1/#section/Standards-and-Conventions
        /// Each conversion is returned in its own <value>quote</value> object.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// <returns></returns>
        public ApiResponseMap<CryptocurrencyPricePerformanceStats> PricePerformanceStatsById(
            string id, eTimePeriodPricePerformanceStats? timePeriod = null,
            string convert = null, string convertId = null)
        {
            return PricePerformanceStats(id, null, null, timePeriod, convert, convertId);
        }

        /// <summary>
        /// <see cref="PricePerformanceStats"/>
        /// </summary>
        /// <param name="slug">
        /// Alternatively pass a comma-separated list of cryptocurrency slugs. Example: <value>bitcoin</value>, 
        /// <value>ethereum</value>
        /// </param>
        /// <param name="timePeriod">
        /// Specify one or more comma-delimited time periods to return stats for. 
        ///     Default: <value>all_time</value>
        ///     Pass all <see cref="eTimePeriodPricePerformanceStats"/> to return all supported time periods.
        /// All rolling periods have a rolling close time of the current request time. For example <value>24h</value> would have a
        /// close time of now and an open time of 24 hours before now. Please note: <value>yesterday</value> is a UTC period and
        /// currently does not currently support <value>high</value> and <value>low</value> timestamps.
        /// </param>
        /// <param name="convert">
        /// Optionally calculate quotes in up to 120 currencies at once by passing a comma-separated list of cryptocurrency or fiat
        /// currency symbols. Each additional convert option beyond the first requires an additional call credit. A list of
        /// supported fiat options can be found at https://coinmarketcap.com/api/documentation/v1/#section/Standards-and-Conventions
        /// Each conversion is returned in its own <value>quote</value> object.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// <returns></returns>
        public ApiResponseMap<CryptocurrencyPricePerformanceStats> PricePerformanceStatsBySlug(
            string slug, eTimePeriodPricePerformanceStats? timePeriod = null,
            string convert = null, string convertId = null)
        {
            return PricePerformanceStats(null, slug, null, timePeriod, convert, convertId);
        }

        /// <summary>
        /// <see cref="PricePerformanceStats"/>
        /// </summary>
        /// <param name="symbol">
        /// Alternatively pass one or more comma-separated cryptocurrency symbols. Example: <value>BTC,ETH</value>.
        /// At least one of <see cref="id"/>, <see cref="slug"/>, or <see cref="symbol"/> is required.
        /// </param>
        /// <param name="timePeriod">
        /// Specify one or more comma-delimited time periods to return stats for. 
        ///     Default: <value>all_time</value>
        ///     Pass all <see cref="eTimePeriodPricePerformanceStats"/> to return all supported time periods.
        /// All rolling periods have a rolling close time of the current request time. For example <value>24h</value> would have a
        /// close time of now and an open time of 24 hours before now. Please note: <value>yesterday</value> is a UTC period and
        /// currently does not currently support <value>high</value> and <value>low</value> timestamps.
        /// </param>
        /// <param name="convert">
        /// Optionally calculate quotes in up to 120 currencies at once by passing a comma-separated list of cryptocurrency or fiat
        /// currency symbols. Each additional convert option beyond the first requires an additional call credit. A list of
        /// supported fiat options can be found at https://coinmarketcap.com/api/documentation/v1/#section/Standards-and-Conventions
        /// Each conversion is returned in its own <value>quote</value> object.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// <returns></returns>
        public ApiResponseMap<CryptocurrencyPricePerformanceStats> PricePerformanceStatsBySymbol(
            string symbol, eTimePeriodPricePerformanceStats? timePeriod = null,
            string convert = null, string convertId = null)
        {
            return PricePerformanceStats(null, null, symbol, timePeriod, convert, convertId);
        }

        /// <summary>
        /// Returns price performance statistics for one or more cryptocurrencies including launch price ROI and 
        /// all-time high / all-time low. Stats are returned for an <value>all_time</value> period by default. 
        /// UTC <value>yesterday</value> and a number of rolling time periods may be requested using the <value>time_period</value>
        /// parameter. Utilize the <value>convert</value> parameter to translate values into multiple fiats or cryptocurrencies
        /// using historical rates.
        /// </summary>
        /// <param name="id">
        /// One or more comma-separated cryptocurrency CoinMarketCap IDs.
        /// Example: <value>a,2</value>
        /// </param>
        /// <param name="slug">
        /// Alternatively pass a comma-separated list of cryptocurrency slugs. Example: <value>bitcoin</value>, 
        /// <value>ethereum</value>
        /// </param>
        /// <param name="symbol">
        /// Alternatively pass one or more comma-separated cryptocurrency symbols. Example: <value>BTC,ETH</value>.
        /// At least one of <see cref="id"/>, <see cref="slug"/>, or <see cref="symbol"/> is required.
        /// </param>
        /// <param name="timePeriod">
        /// Specify one or more comma-delimited time periods to return stats for. 
        ///     Default: <value>all_time</value>
        ///     Pass all <see cref="eTimePeriodPricePerformanceStats"/> to return all supported time periods.
        /// All rolling periods have a rolling close time of the current request time. For example <value>24h</value> would have a
        /// close time of now and an open time of 24 hours before now. Please note: <value>yesterday</value> is a UTC period and
        /// currently does not currently support <value>high</value> and <value>low</value> timestamps.
        /// </param>
        /// <param name="convert">
        /// Optionally calculate quotes in up to 120 currencies at once by passing a comma-separated list of cryptocurrency or fiat
        /// currency symbols. Each additional convert option beyond the first requires an additional call credit. A list of
        /// supported fiat options can be found at https://coinmarketcap.com/api/documentation/v1/#section/Standards-and-Conventions
        /// Each conversion is returned in its own <value>quote</value> object.
        /// </param>
        /// <param name="convertId">
        /// Optionally calculate market quotes by CoinMarketCap ID instead of symbol. This option is
        /// identical to <see cref="convert"/> outside of ID format. Ex: <value>convert_id=1,2781</value>
        /// would replace <value>convert=BTC,USD</value> in your query. This parameter cannot be used when
        /// <see cref="convert"/> is used.
        /// <returns>An object map of cryptocurrency objects by ID, slug, or symbol (as used in query parameters).</returns>
        public ApiResponseMap<CryptocurrencyPricePerformanceStats> PricePerformanceStats(
             string id = null, string slug = null,
             string symbol = null, eTimePeriodPricePerformanceStats? timePeriod = null,
             string convert = null, string convertId = null)
        {
            if (string.IsNullOrWhiteSpace(id) &&
               string.IsNullOrWhiteSpace(slug) &&
               string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Must specify one of: {nameof(id)}, {nameof(slug)}, or {nameof(symbol)}",
                    nameof(id));
            }

            return ApiRequest<ApiResponseMap<CryptocurrencyPricePerformanceStats>>("cryptocurrency/price-performance-stats/latest", 
                new Dictionary<string, string>
                {
                    ["id"] = id,
                    ["slug"] = slug,
                    ["symbol"] = symbol,
                    ["time_period"] = timePeriod?.GetDescription(),
                    ["convert"] = convert,
                    ["convert_id"] = convertId
                });
        }

        #endregion Endpoint: /v1/cryptocurrency/price-performance-stats/latest - Price Performance Stats
    }
}
