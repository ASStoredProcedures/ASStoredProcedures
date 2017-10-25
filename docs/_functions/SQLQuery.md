---
title: ExecuteSQL
version: "1.3"
permissions: U
class: SQLQuery
functions: 
    - ExecuteSql
---
This function allows for the execution of an arbitrary SQL query, probably most useful for calling from a [discussion:rowset action](83292).

This routine would be a useful starting point for developing a custom rowset action.

** ExecuteSQL( _connection string , SQL _ ) **
The first parameter is the connection string to be used, any OLEDB provider can be used, the second parameter is the text of the SQL query.

A user can execute any SQL statement for that their SQL login would normally have the rights to execute.

> **NOTE:** _ This code has been placed in a separate assembly and it should be deployed with the default option of impersonating the current user. If you deploy with the option of impersonating the Service Account there is a risk that the SQL statements could be executed with elevated privileges depending on the rights that have been given to the service account. _

``` sql
call SQLQuery.ExecuteSql(
    "provider=sqlncli;server=localhost;database=AdventureWorksDW;trusted_connection=yes"
    ,"Create TABLE myTable (id int)");
GO
call SQLQuery.ExecuteSql(
    "provider=sqlncli;server=localhost;database=AdventureWorksDW;trusted_connection=yes"
    ,"INSERT INTO myTable VALUES(1)");
GO
call SQLQuery.ExecuteSql(
    "provider=sqlncli;server=localhost;database=AdventureWorksDW;trusted_connection=yes"
    ,"SELECT * FROM myTable");
GO
call SQLQuery.ExecuteSql(
    "provider=sqlncli;server=localhost;database=AdventureWorksDW;trusted_connection=yes"
    ,"DROP TABLE myTable");
```