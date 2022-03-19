# OLAP ETL

https://ravendb.net/docs/article-page/5.3/csharp/studio/database/tasks/ongoing-tasks/olap-etl-task

OLAP ETL will generate Apache Parquet files.
Apache Parquet is the most common “Big Data” storage format for analytics. In Parquet files, data is stored in a columnar-compressed binary format. 
Each Parquet file stores a single table. The table is partitioned into row groups, which each contain a subset of the rows of the table. 
Within a row group, the table data is stored in a columnar fashion.

More about Parquet
https://eng.uber.com/presto/

## Transform without Partitions

```
loadToOrders(noPartition(), {
    // Data will Not be partitioned
    Company: this.Company
});
```


You can use DuckDB to query these files https://duckdb.org/
Also, online tool https://www.parquet-viewer.com/


DuckDB from this point on
https://duckdb.org/docs/data/parquet

## working with sample orders

show content of parquet file
```
SELECT * FROM 'file.parquet' limit 20;
```

find number of orders per company
```
SELECT Company, COUNT(*) FROM 'file.parquet' GROUP BY Company limit 20;
```

find companies with most orders
```
SELECT Company, COUNT(*) FROM 'file.parquet' GROUP BY Company ORDER BY COUNT(*) DESC limit 10;
```