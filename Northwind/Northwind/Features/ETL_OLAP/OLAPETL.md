﻿# OLAP ETL

https://ravendb.net/docs/article-page/5.3/csharp/studio/database/tasks/ongoing-tasks/olap-etl-task
https://ravendb.net/docs/article-page/5.3/csharp/server/ongoing-tasks/etl/olap

OLAP ETL will generate Apache Parquet files.  
Apache Parquet is the most common “Big Data” storage format for analytics. In Parquet files, data is stored in a columnar-compressed binary format.   
Each Parquet file stores a single table. The table is partitioned into row groups, which each contain a subset of the rows of the table.   
Within a row group, the table data is stored in a columnar fashion.  

More about Parquet  
https://eng.uber.com/presto/

Parquet files can be consumed by 
- AWS Athena
- Azure Data Factory
- Snowflake
and many other systems 

One of handy ways to inspect generated files is  
DuckDB https://duckdb.org/  
https://duckdb.org/docs/data/parquet  
https://duckdb.org/docs/data/overview  

## Overview

OLAP ETL is an ongoing task. It will generate Parquet files in each etl batch-run, with the batch frequency 
being configured by the user. e.g. "every day at noon". Every time OLAP ETL task runs, it will process only
documents created/changed since the last run.

There are two flavors of OLAP ETL
- without partitions : hese transformations will transform and push whole collection(s) to a single Parquet file.
- with partitions : data to be exported is separated into subsets, and one Parquet file is generated per subset

Idea behind partitioning data is to introduce optimization early on, even before you start analyzing data
in OLAP database. For example, if you are doing retail analysis, you may partition sales statistics into partitions
for regions and for years. This way, you can load just "Australia 2018" sales data and perform analysis on this
subset.

## Transform without Partitions

Transformation script for Orders
```
loadToOrders(noPartition(), {
    Company: this.Company
});
```

Each batch run will create one Parquet file per such transformation script. All these files will be placed
in the same folder.

Content of this file can be inspected via

```
SELECT * FROM 'file.parquet';
```

or to show just first 10 orders

```
SELECT * FROM 'file.parquet' limit 10;
```

```
┌────────────────┬─────────────┬───────────────────┐
│    Company     │     _id     │ _lastModifiedTime │
├────────────────┼─────────────┼───────────────────┤
│ companies/85-A │ orders/1-A  │ 1532693513        │
│ companies/79-A │ orders/2-A  │ 1532693513        │
│ companies/34-A │ orders/3-A  │ 1532693513        │
│ companies/84-A │ orders/4-A  │ 1532693513        │
│ companies/76-A │ orders/5-A  │ 1532693513        │
│ companies/34-A │ orders/6-A  │ 1532693513        │
│ companies/14-A │ orders/7-A  │ 1532693513        │
│ companies/68-A │ orders/8-A  │ 1532693513        │
│ companies/88-A │ orders/9-A  │ 1532693513        │
│ companies/35-A │ orders/10-A │ 1532693513        │
└────────────────┴─────────────┴───────────────────┘
```

You can dereference CompanyID and push company names instead of their ID
```
loadToOrders(noPartition(), {
    Company: load(this.Company).Name
});
```

```
SELECT * FROM 'file.parquet' limit 10;
```

```
┌───────────────────────────┬─────────────┬───────────────────┐
│          Company          │     _id     │ _lastModifiedTime │
├───────────────────────────┼─────────────┼───────────────────┤
│ Vins et alcools Chevalier │ orders/1-A  │ 1532693513        │
│ Toms Spezialitäten        │ orders/2-A  │ 1532693513        │
│ Hanari Carnes             │ orders/3-A  │ 1532693513        │
│ Victuailles en stock      │ orders/4-A  │ 1532693513        │
│ Suprêmes délices          │ orders/5-A  │ 1532693513        │
│ Hanari Carnes             │ orders/6-A  │ 1532693513        │
│ Chop-suey Chinese         │ orders/7-A  │ 1532693513        │
│ Richter Supermarkt        │ orders/8-A  │ 1532693513        │
│ Wellington Importadora    │ orders/9-A  │ 1532693513        │
│ HILARION-Abastos          │ orders/10-A │ 1532693513        │
└───────────────────────────┴─────────────┴───────────────────┘
```

Whole document can be pushed as well, taking into account that complex fields will not be flattened

Transformation script for Products
```
loadToProducts(noPartition(), this);
```

```
SELECT * FROM 'file.parquet' limit 1;
```

```
┌────────────────┬──────────────┬──────┬───────────────────────┬────────────────────┬──────────────┬───────────────┬──────────────┬──────────────┬──────────────┬───────────────────┐
│    Category    │ Discontinued │ Name │     PricePerUnit      │  QuantityPerUnit   │ ReorderLevel │   Supplier    │ UnitsInStock │ UnitsOnOrder │     _id      │ _lastModifiedTime │
├────────────────┼──────────────┼──────┼───────────────────────┼────────────────────┼──────────────┼───────────────┼──────────────┼──────────────┼──────────────┼───────────────────┤
│ categories/1-A │ false        │ Chai │ 18.000000000000000000 │ 10 boxes x 20 bags │ 10           │ suppliers/1-A │ 1            │ 39           │ products/1-A │ 1647465276        │
└────────────────┴──────────────┴──────┴───────────────────────┴────────────────────┴──────────────┴───────────────┴──────────────┴──────────────┴──────────────┴───────────────────┘
```

Getting back to orders, now you can use OLAP database to 
find number of orders per company
```
SELECT Company, COUNT(*) FROM 'file.parquet' GROUP BY Company limit 5;
```

```
┌───────────────────────────┬──────────────┐
│          Company          │ count_star() │
├───────────────────────────┼──────────────┤
│ Vins et alcools Chevalier │ 5            │
│ Toms Spezialitäten        │ 6            │
│ Hanari Carnes             │ 14           │
│ Victuailles en stock      │ 10           │
│ Suprêmes délices          │ 12           │
└───────────────────────────┴──────────────┘
```

find companies with most orders
```
SELECT Company, COUNT(*) FROM 'file.parquet' GROUP BY Company ORDER BY COUNT(*) DESC limit 5;
```

```
┌────────────────────────────┬──────────────┐
│          Company           │ count_star() │
├────────────────────────────┼──────────────┤
│ Save-a-lot Markets         │ 31           │
│ Ernst Handel               │ 30           │
│ QUICK-Stop                 │ 28           │
│ Rattlesnake Canyon Grocery │ 19           │
│ Folk och fä HB             │ 19           │
└────────────────────────────┴──────────────┘
```

## Trasform with Partition

This transformation will create multiple parquets, with folder hierarchy determined by the partitioning scheme.
For example, if you are partitioning by year and month, foldrt "year=2022" will be created on top level, 
with subfolders "month=01", "month=02" etc.

Hence, these Parquet files contain a subset of the data, with the partition columns not being stored 
inside the parquet but instead they are used as folder names.

```
SELECT * FROM parquet_scan(['file1.parquet', 'file2.parquet', 'file3.parquet']);
```
such queries will be much more efficient and less demanding, since you will be processing
significantly less data

partition by date Order was created on
```
var key = new Date(this.OrderedAt);
loadToOrders(partitionBy(key), {
    // The partition that will be created will be: "_partition={key}"
    Company: load(this.Company).Name
});
```

partition by year and month
```
var orderDate = new Date(this.OrderedAt);
var year = orderDate.getFullYear();
var month = orderDate.getMonth() + 1;

loadToOrders(partitionBy(['year', year], ['month', month]), {
    // The order of params in the partitionBy method determines the parquet file path
    // The partition that will be created will be: "year={year}" on top level, with subfolders "month={month}"
    Company: load(this.Company).Name,
    ShipVia: this.ShipVia
});
```
