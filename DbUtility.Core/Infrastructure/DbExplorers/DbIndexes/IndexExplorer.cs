using Dapper;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes
{
    public class IndexExplorer : IIndexExplorer
    {
        readonly IDbConnection _con;
        readonly ILogger<IndexExplorer> _logger;
        private readonly string UnusedIndexQuery = @"SELECT 
                                        '[' + s.name + '].[' + o.name + ']' TableName,
	                                    '[' + s.name + '].[' + i.name + ']' IndexName,
	                                    dm_ius.user_seeks UserSeek,
	                                    dm_ius.user_scans UserScans,
	                                    dm_ius.user_lookups UserLookups,
	                                    dm_ius.user_updates AS UserUpdates,
	                                    p.TableRows,
	                                    SUM(sz.[used_page_count]) * 8 IndexSize
                                    FROM sys.dm_db_index_usage_stats dm_ius
                                    INNER JOIN sys.indexes i ON i.index_id = dm_ius.index_id
	                                    AND dm_ius.OBJECT_ID = i.OBJECT_ID
                                    INNER JOIN sys.objects o ON dm_ius.OBJECT_ID = o.OBJECT_ID
                                    INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                                    INNER JOIN sys.dm_db_partition_stats sz ON sz.object_id = o.OBJECT_ID
                                    INNER JOIN (
	                                    SELECT SUM(p.rows) TableRows,
		                                    p.index_id,
		                                    p.OBJECT_ID
	                                    FROM sys.partitions p
	                                    GROUP BY p.index_id,
		                                    p.OBJECT_ID
	                                    ) p ON p.index_id = dm_ius.index_id
	                                    AND dm_ius.OBJECT_ID = p.OBJECT_ID
                                    WHERE OBJECTPROPERTY(dm_ius.OBJECT_ID, 'IsUserTable') = 1
	                                    AND dm_ius.database_id = DB_ID()
	                                    AND i.type_desc = 'nonclustered'
	                                    AND i.is_primary_key = 0
	                                    AND i.is_unique_constraint = 0
                                    GROUP BY s.name,
										'[' + s.name + '].[' + o.name + ']',
										'[' + s.name + '].[' + i.name + ']' ,
	                                    dm_ius.user_seeks,
	                                    dm_ius.user_scans,
	                                    dm_ius.user_lookups,
	                                    dm_ius.user_updates,
	                                    p.TableRows
                                    ORDER BY (dm_ius.user_seeks + dm_ius.user_scans + dm_ius.user_lookups) ASC";
        private readonly string DublicateIndexesQuery = @";WITH CTE_INDEX_DATA AS (
                                                            SELECT
                                                                    SCHEMA_DATA.name AS schema_name,
                                                                    TABLE_DATA.name AS table_name,
                                                                    INDEX_DATA.name AS index_name,
                                                                    STUFF((SELECT  ', ' + COLUMN_DATA_KEY_COLS.name + ' ' + CASE WHEN INDEX_COLUMN_DATA_KEY_COLS.is_descending_key = 1 THEN 'DESC' ELSE 'ASC' END -- Include column order (ASC / DESC)
                                                                                        FROM    sys.tables AS T
                                                                                                    INNER JOIN sys.indexes INDEX_DATA_KEY_COLS
                                                                                                    ON T.object_id = INDEX_DATA_KEY_COLS.object_id
                                                                                                    INNER JOIN sys.index_columns INDEX_COLUMN_DATA_KEY_COLS
                                                                                                    ON INDEX_DATA_KEY_COLS.object_id = INDEX_COLUMN_DATA_KEY_COLS.object_id
                                                                                                    AND INDEX_DATA_KEY_COLS.index_id = INDEX_COLUMN_DATA_KEY_COLS.index_id
                                                                                                    INNER JOIN sys.columns COLUMN_DATA_KEY_COLS
                                                                                                    ON T.object_id = COLUMN_DATA_KEY_COLS.object_id
                                                                                                    AND INDEX_COLUMN_DATA_KEY_COLS.column_id = COLUMN_DATA_KEY_COLS.column_id
                                                                                        WHERE   INDEX_DATA.object_id = INDEX_DATA_KEY_COLS.object_id
                                                                                                    AND INDEX_DATA.index_id = INDEX_DATA_KEY_COLS.index_id
                                                                                                    AND INDEX_COLUMN_DATA_KEY_COLS.is_included_column = 0
                                                                                                    AND INDEX_DATA.is_hypothetical = 0
                                                                                        ORDER BY INDEX_COLUMN_DATA_KEY_COLS.key_ordinal
                                                                                        FOR XML PATH('')), 1, 2, '') AS key_column_list ,
                                                                STUFF(( SELECT  ', ' + COLUMN_DATA_INC_COLS.name
                                                                                        FROM    sys.tables AS T
                                                                                                    INNER JOIN sys.indexes INDEX_DATA_INC_COLS
                                                                                                    ON T.object_id = INDEX_DATA_INC_COLS.object_id
                                                                                                    INNER JOIN sys.index_columns INDEX_COLUMN_DATA_INC_COLS
                                                                                                    ON INDEX_DATA_INC_COLS.object_id = INDEX_COLUMN_DATA_INC_COLS.object_id
                                                                                                    AND INDEX_DATA_INC_COLS.index_id = INDEX_COLUMN_DATA_INC_COLS.index_id
                                                                                                    INNER JOIN sys.columns COLUMN_DATA_INC_COLS
                                                                                                    ON T.object_id = COLUMN_DATA_INC_COLS.object_id
                                                                                                    AND INDEX_COLUMN_DATA_INC_COLS.column_id = COLUMN_DATA_INC_COLS.column_id
                                                                                        WHERE   INDEX_DATA.object_id = INDEX_DATA_INC_COLS.object_id
                                                                                                    AND INDEX_DATA.index_id = INDEX_DATA_INC_COLS.index_id
                                                                                                    AND INDEX_COLUMN_DATA_INC_COLS.is_included_column = 1
                                                                                                    AND INDEX_DATA.is_hypothetical = 0
                                                                                        ORDER BY INDEX_COLUMN_DATA_INC_COLS.key_ordinal
                                                                                        FOR XML PATH('')), 1, 2, '') AS include_column_list,
                                                            INDEX_DATA.is_disabled -- Check if index is disabled before determining which dupe to drop (if applicable)
                                                            FROM sys.indexes INDEX_DATA
                                                            INNER JOIN sys.tables TABLE_DATA ON TABLE_DATA.object_id = INDEX_DATA.object_id
                                                            INNER JOIN sys.schemas SCHEMA_DATA ON SCHEMA_DATA.schema_id = TABLE_DATA.schema_id	                                                
                                                            WHERE TABLE_DATA.is_ms_shipped = 0
                                                            AND INDEX_DATA.is_hypothetical = 0
                                                            AND INDEX_DATA.type_desc IN ('NONCLUSTERED', 'CLUSTERED')
                                                    )
                                                    SELECT
	                                                    '[' + schema_name + '].[' + table_name + ']' TableName,
	                                                    '[' + schema_name + '].[' + index_name + ']' IndexName,
	                                                    ISNULL(key_column_list,'') KeyColumnList,
	                                                    ISNULL(include_column_list,'') IncludeColumnList
                                                    FROM CTE_INDEX_DATA DUPE1
                                                    WHERE EXISTS
                                                    (SELECT * FROM CTE_INDEX_DATA DUPE2
                                                        WHERE DUPE1.schema_name = DUPE2.schema_name
                                                        AND DUPE1.table_name = DUPE2.table_name
                                                        AND DUPE1.key_column_list = DUPE2.key_column_list
                                                        AND ISNULL(DUPE1.include_column_list, '') = ISNULL(DUPE2.include_column_list, '')
                                                        AND DUPE1.index_name <> DUPE2.index_name)
                                                        GROUP BY '[' + schema_name + '].[' + table_name + ']',
	                                                    '[' + schema_name + '].[' + index_name + ']',
	                                                    key_column_list,
	                                                    include_column_list";

        public IndexExplorer(ILogger<IndexExplorer> logger, IDbConnection con)
        {
            _logger = logger;
            _con = con;
        }

        /// <summary>
        /// Get index execution statistic
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UnusedIndexStatistic>> GetUnusedIndexesAsync()
        {
            try
            {
                return await _con.QueryAsync<UnusedIndexStatistic>(UnusedIndexQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError($"IndexExplorer; GetUnusedIndexesAsync, Error= {ex.Message}");
                return new List<UnusedIndexStatistic>();
            }
        }

        /// <summary>
        /// Get index dublicates 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DbIndex>> GetDublicateIndexesAsync()
        {
            try
            {
                return await _con.QueryAsync<DbIndex>(DublicateIndexesQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError($"IndexExplorer; GetDublicateIndexesAsync, Error= {ex.Message}");
                return new List<DbIndex>();
            }
        }
    }
}
