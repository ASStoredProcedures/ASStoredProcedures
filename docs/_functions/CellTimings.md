---
title: CellTimings
version: "1.0"
permissions: S
class: CellTimings
functions: 
    - TimeToCalculate
---
## TimeToCalculate(Tuple tupleToEvaluate)



TimeToCalculate returns the time taken in ticks to return the value displayed in a cell in a cellset.

The following example MDX query shows how it can be used in a calculated member:


```raw
WITH MEMBER MEASURES.GETTIME AS ASSP.TimeToCalculate(Measures.[Internet Sales Amount])

MEMBER MEASURES.TIMEPERCENT AS MEASURES.GETTIME/SUM(AXIS(1), MEASURES.GETTIME), FORMAT_STRING='PERCENT'

MEMBER [Date].[Day of Week].VERYBIGCALC AS SUM([Date].[Date].[Date].MEMBERS * [Product].[Category].MEMBERS,Measures.[Internet Sales Amount] )

SELECT {MEASURES.GETTIME, MEASURES.TIMEPERCENT} ON 0,

[Date].[Day of Week].ALLMEMBERS

ON 1

from [Adventure Works]
```

you could also use it in an MDX Script assignment. This function could be useful when you have a poorly-performing query and you don't know what is causing the problem: cells which display relatively high values, which take most time to display their values, are likely to be the ones you need to investigate - in the example, the member VERYBIGCALC takes much longer to evaluate than any other member in the cellset. Note that this function will return slightly different values each time you run it, and you'll also see the effects of the caching that Analysis Services does between queries as well as the caching it does inside each query when retrieving cell values unaffected by any calculation (for example in the query above, you'll see that 

```raw
[Day].[Day of Week].&[1]
```

takes longer than any of its siblings to return).