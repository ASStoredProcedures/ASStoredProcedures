---
title: Multiply
version: "1.0"
permissions: S
class: Multiplication
functions:
    - Multiply
---

Multiply() multiplies the value returned by an MDX expression for each member of a set. This is useful for calculations such as compound growths, and (limited) testing shows that using this function is much faster than implementing the same calculation in pure MDX using recursion or the approach using log functions descibed here [http://geekswithblogs.net/darrengosbell/archive/2006/07/18/85539.aspx](http://geekswithblogs.net/darrengosbell/archive/2006/07/18/85539.aspx)

```raw
-- An example query showing how the Multiply() function can be used to 
-- multiply a measure value for the members of a set.
WITH MEMBER MEASURES.MULTIPLYDEMO 
AS  
ASSP.Multiply(
  [Date].[Day Name].[Day Name].MEMBERS.ITEM(0):[Date].[Day Name].CURRENTMEMBER
  , [Measures].[Internet Sales Amount])
SELECT 
  {[Measures].[Internet Sales Amount],MEASURES.MULTIPLYDEMO} ON 0,
  [Date].[Day Name].[Day Name].MEMBERS ON 1
FROM [Adventure Works]
```
