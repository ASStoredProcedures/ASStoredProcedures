---
title: TraceEvent
version: "1.3.6"
permissions: S
class: TraceEvent
functions:
    - FireTraceEventAndReturnValue
---
The function in the TraceEvent class is FireTraceEventAndReturnValue. It can be used to detect when Analysis Services is evaluating an expression since the function merely fires a trace event that appears in Profiler as a User Defined event, then returns the integer value you pass in. This function can be used in a scenario such as determining when the cell security expression is evaluated.

Here is an example MDX query displaying the usage of this function:

```raw
with member [Measures].[test] as ASSP.FireTraceEventAndReturnValue(99)
select [Measures].[test] on 0
from [Adventure Works]
```

The signature of the function is:
**_FireTraceEventAndReturnValue(int)_**
