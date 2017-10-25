---
title: DMV
version: "1.2"
permissions: U
class: XmlaDiscover
functions:
  - DMV
---

**DMV( SelectStatement , _\[Restrictions]_)**
This function supports the execution of DMV Select queries similar to those in SSAS 2008, but with the following differences.

* Supports the LIKE operator in the WHERE clause
* Supports multiple columns in the ORDER BY clause
* The $System schema is optional 
* Maps filter simple conditions from the WHERE clause to restrictions in the Discover call

This last point is particularly useful when querying schema rowsets which have mandatory restrictions, at the time of writting this was not possible with SSAS 2008. This facility only works with simple filters which are in the form of _<column> = <value>_ where multiple conditions are joined with an "AND". Any variation from this pattern (including surrounding conditions with parenthesis) will cause the restriction matching to fail. But if you do require the use of a more complicated filter with such a rowset, the DMV() function does provide for this with the optional restrictions clause, where the XML/A restrictions can be passed in to the function.

This example combines the LIKE operator with multiple columns in the ORDER BY clause.

```raw
CALL ASSP.DMV("
SELECT DIMENSION_ORDINAL
 , [DIMENSION_NAME]
 , DIMENSION_CARDINALITY
 , DEFAULT_HIERARCHY
  FROM $SYSTEM.MDSCHEMA_DIMENSIONS
 WHERE CUBE_NAME LIKE '%Sales'
ORDER BY DIMENSION_NAME, DIMENSION_ORDINAL");
```

This call demonstrates how ASSP.DMV can map WHERE filters to the restrictions of the XML/A discover call in order to support querying rowsets that have required restrictions.

```raw
CALL ASSP.DMV("
SELECT *
FROM $SYSTEM.DISCOVER_PARTITION_DIMENSION_STAT
WHERE DATABASE_NAME = 'Adventure Works DW'
  AND CUBE_NAME = 'Adventure Works'
  AND MEASURE_GROUP_NAME = 'Internet Sales'
  AND PARTITION_NAME = 'Internet_Sales_2003'
ORDER BY DIMENSION_NAME, ATTRIBUTE_COUNT_MAX desc
");
```

This is the same as the call above, just with an explicit restrictions clause rather than using an implied one from the WHERE conditions.

```raw
CALL ASSP.DMV("
SELECT *
FROM $SYSTEM.DISCOVER_PARTITION_DIMENSION_STAT
ORDER BY DIMENSION_NAME, ATTRIBUTE_COUNT_MAX desc"
,"<DATABASE_NAME>Adventure Works DW</DATABASE_NAME>
<CUBE_NAME>Adventure Works</CUBE_NAME>
<MEASURE_GROUP_NAME>Internet Sales</MEASURE_GROUP_NAME>
<PARTITION_NAME>Internet_Sales_2003</PARTITION_NAME>");}}
```

This is how the call would look using the Discover function  (ie. without the ORDER BY). You can see that, in this case, the output is very similar, but the DMV function does provide for more sophisticated filtering, ordering and the option of only returning a subset of columns.

```raw
CALL ASSP.Discover("DISCOVER_PARTITION_DIMENSION_STAT"
,"<DATABASE_NAME>Adventure Works DW</DATABASE_NAME>
<CUBE_NAME>Adventure Works</CUBE_NAME>
<MEASURE_GROUP_NAME>Internet Sales</MEASURE_GROUP_NAME>
<PARTITION_NAME>Internet_Sales_2003</PARTITION_NAME>");}}
```

There are more examples available in the "DMV Example MDX" file that is included with the source code.
