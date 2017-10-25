---
title: SetOperations
version: "1.1"
permissions: S
class: SetOperations
functions:
    - BottomCountWithTies
    - InverseHierarchility
    - TopCountWithTies
    - Order
    - RandomSample
    - ReverseSet
    - AsymmetricDrillDown
---
The functions in this class are:

**ReverseSet(_Set_)**

This function reverses the order of the set passed in as a parameter. For instance:

```raw
ASSP.ReverseSet([Date](Date).[Calendar](Calendar).[Calendar Year](Calendar-Year).members)
```

returns 2004, 2003, 2002, 2001.


**RandomSample(_Set_, _NumberToReturn_)**

This function takes in a set as the first parameter and an integer as the second parameter. It returns _NumberToReturn_ random tuples from the _Set_. For instance:

```raw
ASSP.RandomSample([Product].[Product].[Product].members, 10)
```

returns 10 random products.


**InverseHierarchility(_Set_)**

This function reverses the order of the members within each tuple in the set. The order of the set does not change. For instance:

```raw
ASSP.InverseHierarchility({([2004],[Bikes]),([2003],[Accessories]})
```

returns:

```raw
{([Bikes],[2004]),([Accessories],[2003])}
```


**AsymmetricDrillDown(_Set_, _TupleIndex_, _HierarchyIndex_)**

This function expands the set which is passed in as the first parameter. For the tuple at position _TupleIndex_ and for the member at _HierarchyIndex_, add tuples to the set by drilling down that member. Indexes are zero based.



**Order(_Set_, _Expression_, _\[SortDescending]_ )**

This function is very similar to the built-in Order function in MDX. In some cases, it may outperform the built-in Order function due to a [bug](http://sqljunkies.com/WebLog/mosha/archive/2007/04/19/stored_procs_best_practices.aspx). However, because it does the sorting in memory, it may also not scale as well as the built-in MDX Order function, so please test carefully. The parameters are as follows:
* **_Set_** - The first parameter is a set expression that you wish to sort.
* **_Expression_** - The second parameter is the expression you wish to sort on. This expression is evaluated for every tuple in the set you passed in as the first parameter. Then the evaluated value is cast to a double and sorted. (Do not use an expression that could return a string because it will either cause an error during casting to a double, or it will sort as a number, not as a string. This limitation exists because it is [not possible](https://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=254209) to detect the type of an evaluated expression.)
* **_SortDescending_** - The third parameter is an optional boolean parameter which specifies sort direction. Passing in false means to sort BASC. Passing in true means to sort BDESC. Sorting ASC or DESC are not supported by the ASSP Order function.
Note that the ordering may not match the order of the built-in Order function for those tuples that tie.

Note this function must be called with brackets so as not to conflict with the built-in Order function:

```raw
ASSP.[Order]([Product].[Category].[Category].MEMBERS, [Measures].[Internet Sales Amount], true)
```


**TopCountWithTies(_Set_, _Count_, _SortExpression_)**
**BottomCountWithTies(_Set_, _Count_, _SortExpression_)**

This function behaves exactly like the native MDX function TopCount and BottomCount. The only difference is the function will return the tied up value if the Nth tuple has the duplicated value. Thanks to Jean Zhang for the first prototype of this code.

```raw
ASSP.TopCountWithTies(
 [Product].[Product Categories].[Product].members
 ,62
 ,[Measures].[Internet Order Count]
)
```