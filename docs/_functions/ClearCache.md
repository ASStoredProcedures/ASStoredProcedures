---
title: ClearCache
version: "1.1"
permissions: U
class: XmlaDiscover
functions:
    - ClearCache
---

**ClearCache(_cubeName_)**
This function clears the cache which is useful when doing performance tuning. If you call ClearCache without any parameters the entire cache for the current database is cleared. If the optional _cubeName_ parameter is specified then only the cache for that particular cube is cleared.

``` sql
-- Clears the cache for the current database
CALL ASSP.ClearCache();
-- Clears the cache for the Adventure Works cube in the current database 
CALL ASSP.ClearCache("Adventure Works");
```