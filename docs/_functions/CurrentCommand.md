---
title: CurrentCommand
version: "1.3.7"
permissions: U
class: CurrentCommand
functions: 
    - CurrentCommandIsDiscover
    - GetCurrentCommand
---
The following will be part of release 1.3.7...

**bool CurrentCommandIsDiscover()**
This function returns true if the current command executing which triggered this sproc is a DISCOVER or a MDSCHEMA command. For example, if you define a dynamic set in the MDX script, it will be evaluated when the user connects in Excel so that Excel can determine which dimension folder to display the set under. Changing the dynamic set definition as follows will allow the set to communicate that it's part of the Product dimension without incurring the expense of evaluating the TopCount function until actually used in a real query. (This is usually unnecessary, but if the TopCount expression is already optimized with aggregations and it is causes unacceptable slowness when the user connects Excel, this is a workaround.)

```raw
Create Dynamic Set CurrentCube.[Top 25 Selling Products] As 
IIf(
 ASSP.CurrentCommandIsDiscover(),
 Head([Product].[Product].[Product].Members,0),
 TopCount
    (
       [Product].[Product].[Product].Members,
       25,
       [Measures].[Sales Amount]
    ) 
),
Display_Folder = 'Sets';   
```

Teo Lachev describes this scenario and usage of this sproc further in a [blog post](http://prologika.com/CS/blogs/blog/archive/2014/02/18/when-dynamic-sets-are-not-dynamic.aspx).


**string GetCurrentCommand()**
This sproc also provides a function call which returns the string which is the currently executing command.

> Note: These sprocs are not available in Analysis Services 2005 because Context.CurrentConnection.SessionID isn't available in sprocs in AS2005. It is available in later versions of SSAS.