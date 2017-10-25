---
title: Discover
version: "1.1"
permissions: U
class: Discover
functions: 
    - Discover
    - DiscoverConnections
    - DiscoverSessions
    - DiscoverRowsets
---

**Discover(requestType _, restrictions, properties_ )**
This function allows you to execute most of the XMLA Discover requests and returns the results as a DataTable where possible. This function is only build to handle "flat" results, when hierarchical results are returned this function will only return the first level. However this still caters for nearly all of the discover requests. 

> Note: In the source code you will notice the \[SafeToPrepare()] attribute, setting this attribute allows for these functions to be called from Reporting Services. Without this attribute, calling a stored procedure using the CALL syntax will result in an error.

``` sql
-- This is an example of a simple discover call
CALL ASSP.Discover("DBSCHEMA_CATALOGS")

-- This is an example passing in a CUBE_NAME restriction
-- the available restrictions varies depending on the particular query.
CALL ASSP.Discover("MDSCHEMA_DIMENSIONS","<CUBE_NAME>Channel Sales</CUBE_NAME>")

-- This is an example passing in a CUBE_NAME restriction
-- and the CATALOG property
CALL ASSP.Discover("MDSCHEMA_DIMENSIONS","<CUBE_NAME>Channel Sales</CUBE_NAME>","<CATALOG>Adventure Works DW</CATALOG>")
```

There are a number of requests for which short cut functions have been created:

* DiscoverConnections
* DiscoverSessions
* DiscoverRowsets