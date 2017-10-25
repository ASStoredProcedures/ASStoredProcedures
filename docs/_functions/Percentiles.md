---
title: Percentiles
version: 1.3.7
permissions: S
class: Percentiles
functions:
  - ValueAtPercentile
  - RangePoint
---
The following will be part of release 1.3.7...

Courtesy of [Richard Mintz](http://richmintzbi.wordpress.com), the Percentiles class contains functions which can be used to return the value at a certain percentile within a set. The ASSP sproc is much easier to use than the complex MDX required to mimic this functionality, however, it may or may not perform as well as straight MDX.

The functions in this class are:

**ValueAtPercentile(_inputSet_, _sortExpression_, _percentileValue_, _sortAscending_, _calcMethod_)**

* The _inputSet_ parameter is the set over which the percentile will be calculated. You may want to do a NonEmpty on the set before passing in the set, depending on requirements.
* The _sortExpression_ parameter is an expression, usually a measure, which will be used for ASSP to sort the set.
* The _percentileValue_ parameter is a double value. For the 90th percentile, pass in .9
* The _sortAscending_ parameter is a boolean value. True is ascending and False is descending.
* The _calcMethod_ parameter says which algorithm to use. If you pass in "EXC" then the code will mimic Excel. If "INC" is passed in, it will not mimic Excel.

An example call to this sproc against Adventure Works looks like:

```raw
with
member [Measures].[Percentiles] AS
ASSP.ValueAtPercentile(
  NonEmpty([Product].[Product].[Product].Members, [Measures].[Sales Amount])
  , [Measures].[Sales Amount]
  , .90
  , true
  , "INC"
 )
select {[measures].[percentiles]} on 0
from [Adventure Works]
```

The equivalent MDX is rather long and unmaintainable, though in some situations it may perform better:

```raw
with MEMBER [Measures].[RangePoint] AS
((COUNT(NONEMPTY([Product].[Product].[Product].members
     ,[Measures].[sales amount])) -1) *.90)
 
MEMBER [Measures].[RangePoint_Int]
AS
INT([Measures].[RangePoint])
 
MEMBER [Measures].[Percentiles] AS
IIF(INT([Measures].[RangePoint]) = [Measures].[RangePoint], 
(ORDER(NONEMPTY(([Product].[Product].[Product].members
),[Measures].[Sales Amount]),[Measures].[sales amount], basc).item([Measures].[RangePoint]), 
[Measures].[Sales Amount])
,
(ORDER(NONEMPTY(([Product].[Product].[Product].members
),[Measures].[Sales Amount]),[Measures].[sales amount], basc).item([Measures].[RangePoint_int]), 
[Measures].[Sales Amount])
+ 
([Measures].[RangePoint] - INT([measures].[rangepoint]))
*
(
(ORDER(NONEMPTY(([Product].[Product].[Product].members
),[Measures].[Sales Amount]),[Measures].[sales amount], basc).item([Measures].[RangePoint_int]+1), 
[Measures].[Sales Amount])
-
(ORDER(NONEMPTY(([Product].[Product].[Product].members
),[Measures].[Sales Amount]),[Measures].[sales amount], basc).item([Measures].[RangePoint_int]), 
[Measures].[Sales Amount])) )   
 
select {[measures].[percentiles]} on 0
from [Adventure Works]
```

Richard blogged about this business need further [here](http://richmintzbi.wordpress.com/2012/06/07/percentiles-like-excel-to-the-4th-power-t-sql-sql-clr-mdx-assp/).

The RangePoint function is also helpful, mainly for troubleshooting which member of the set it being returned. The parameters have the same meaning as in the ValueAtPercentile function above.

**RangePoint(_inputSet_, _percentileValue_, _calcMethod_)**
