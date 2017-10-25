---
title: DiscoverSingleValue
version: "1.3"
permissions: U
class: XmlaDiscover
functions: 
    - DiscoverSingleValue
    - DiscoverXmlMetaDataSingleValue
    - DiscoverXmlMetaDataFullSingleData
    - DMVSingleValue
---
These functions wrap the Discover, DiscoverXmlMetadata, DiscoverXmlMetadataFull, and DMV functions returning the value of a single column. They can be used in an MDX expression that expects a scalar datatype like integer or string.

**DiscoverSingleValue(column, requestType _, restrictions, properties_ )**
The function wraps the [Discover](../Discover) function and returns the value of a particular column, specified by the first parameter, for the first row in the DataTable returned by the Discover function.

**DiscoverXmlMetaDataSingleValue(column, path, _restrictions_)**
The function wraps the [DiscoverXmlMetadata](../DiscoverXmlMetadata) function and returns the value of a particular column, specified by the first parameter, for the first row in the DataTable returned by the DiscoverXmlMetadata function.

**DiscoverXmlMetaDataFullSingleData(column, path, _restrictions_)**
The function wraps the [DiscoverXmlMetadataFull](../DiscoverXmlMetadata) function and returns the value of a particular column, specified by the first parameter, for the first row in the DataTable returned by the DiscoverXmlMetadataFull function.

**DMVSingleValue(column, SelectStatement, _restrictions_)**
The function wraps the [DMV](../DMV) function and returns the value of a particular column, specified by the first parameter, for the first row in the DataTable returned by the DMV function.

```raw
with member test as 
ASSP.DiscoverSingleValue("DIMENSION_CARDINALITY", "MDSCHEMA_DIMENSIONS","<CUBE_NAME>Channel Sales</CUBE_NAME><DIMENSION_NAME>Employee</DIMENSION_NAME>","<CATALOG>Adventure Works DW 2008</CATALOG>")
select test on 0
from [Adventure Works]

with member test as 
ASSP.discoverXmlMetaDataFullSingleValue("Version", "\Server|ID,Version")
select test on 0
from [Adventure Works]

with member test as 
ASSP.discoverXmlMetaDataSingleValue("EstimatedSize", "\Database")
select test on 0
from [Adventure Works]

with member test as 
ASSP.DMVSingleValue(
 "DIMENSION_CARDINALITY"
 ,"SELECT DIMENSION_ORDINAL
 , DIMENSION_NAME
 , DIMENSION_CARDINALITY
 , DEFAULT_HIERARCHY
  FROM $SYSTEM.MDSCHEMA_DIMENSIONS
 WHERE CUBE_NAME = 'Direct Sales'
 and DIMENSION_NAME = 'Sales Territory'
 ORDER BY DIMENSION_ORDINAL"
)
select test on 0
from [Adventure Works]
```

An example application of these functions is discussed in the [Self-Documenting Cubes In Excel PivotTables](http://www.artisconsulting.com/blogs/greggalloway/Lists/Posts/Post.aspx?ID=17) blog post by Greg Galloway.