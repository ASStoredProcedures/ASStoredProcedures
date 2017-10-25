---
title: EfficientToDate
version: "1.0"
permissions: S
class: EfficientToDate
function:
    - GetEfficientPeriodsToDateSet
    - GetEfficientToDateSet
    - GetEfficientYearToDateSet
    - GetEfficientQuarterToDateSet
    - GetEfficientMonthToDateSet
    - GetMostGranularHierarchyCurrentMember
---

The EfficientToDate class contains functions which can be used to optimize "to date" style sets. For example, the following _Ytd_ function call returns a set which does not take full advantage of the aggregations above the fiscal month level:

```raw
YTD([Date].[Fiscal].[October 2006])
```

This Ytd expression results in a set of 10 members (January 2006 through October 2006). An equivalent and more efficient set would require only three members (H1 2006, Q3 2006, October 2006).

The functions in this class are:

**GetEfficientPeriodsToDateSet(_LevelName_, _MemberExpression_)**

This function has a very similar signature to the _PeriodsToDate_ MDX function. The first parameter (_LevelName_) is a string specifying the level name.  The second parameter (_MemberExpression_) is a valid MDX member expression.

The following example returns a set which is the most efficient way of expressing Year-To-Date October 2002:

```raw
GetEfficientPeriodsToDateSet("Calendar Year",[Date].[Calendar].[Month].[October 2002])
```

The following example returns a set which is the most efficient way of expressing all of time through October 2003:

```raw
GetEfficientPeriodsToDateSet("(All)",[Date].[Calendar].[Month].[October 2003])
```

Compared to the expense of calling _PeriodsToDate_, the _GetEfficientPeriodsToDateSet_ is generally in the same order of magnitude. All of the other functions listed below are much more expensive to call -- so expensive to call, in fact, that calling those functions more than a handful of times per MDX query is prohibitively expensive. Therefore, the _GetEfficientPeriodsToDateSet_ should be used whenever possible.



**GetEfficientToDateSet()**

This function returns the most efficient set representing all of time through the current member in the first Time dimension. If the first Time dimension contains more than one user-defined hierarchy, the hierarchy with the current member which is the most granular is chosen. (See _GetMostGranularHierarchyCurrentMember_ below for more details.)

This function is more flexible than _GetEfficientPeriodsToDateSet_ because you do not have to specify a member. This flexibility could be handy in a situation where the Time dimension has more than one user-defined hierarchy (such as Calendar and Fiscal) by which an end user could slice the cube. Since it is not known in advance which hierarchy the end user will pick, this function could be used to correctly handle any slice they choose.


**GetEfficientToDateSet(_DimensionName_)**

This signature of the _GetEfficientToDateSet_ function takes in a string parameter specifying the dimension name for the Time dimension. This parameter makes this signature slightly less expensive because the system does not have to search for the first Time dimension. Otherwise, the function acts the same as _GetEfficientToDateSet()_.


**GetEfficientToDateSet(_MemberExpression_)**

This signature of the _GetEfficientToDateSet_ function takes in a valid MDX member expression parameter specifying the member for which the "to date" set should be calculated. This parameter makes this signature slightly less expensive because the system does not have to search for the Time dimension and the system does not have to search for the most granular current member. Otherwise, the function acts the same as _GetEfficientToDateSet()_, though it is not as flexible in the situation of a dimension having multiple user-defined hierarchies unless the member expression conditionally evaluates to a member in the appropriate hierarchy.


**GetEfficientYearToDateSet(_MemberExpression_)**

This function works similarly to _GetEfficientToDateSet(MemberExpression)_ except that it returns the Year-To-Date set instead of the (all time) to date set. This function is less efficient than _GetEfficientPeriodsToDateSet_ because it has to search for the year level in the hierarchy.


**GetEfficientQuarterToDateSet(_MemberExpression_)**

This function works similarly to _GetEfficientYearToDateSet_ except that it returns the Quarter-To-Date set. This function is less efficient than _GetEfficientPeriodsToDateSet_ because it has to search for the quarter level in the hierarchy.


**GetEfficientMonthToDateSet(_MemberExpression_)**

This function works similarly to _GetEfficientYearToDateSet_ except that it returns the Month-To-Date set. This function is less efficient than _GetEfficientPeriodsToDateSet_ because it has to search for the month level in the hierarchy.


**GetMostGranularHierarchyCurrentMember(_DimensionName_)**

This function takes a string parameter specifying a dimension name. It then searches all the user-defined hierarchies in that dimension for the most granular current member. For instance, a Date dimension may have a Calendar hierarchy with levels Year, Month, and Day. That same Date dimension may also have a Fiscal hierarchy with levels Year, Month, and Week. If you slice by January 1 using the Date attribute hierarchy, (depending upon the attribute relationships and setup of your UDM) the current member of the Fiscal hierarchy may be Week 26 and the current member of the Calendar hierarchy will be January 1. This function uses the metadata in the Date dimension to determine that the current member in the Calendar hierarchy is the more granular. If, however, the user slices by Fiscal 2006, the current member of the Calendar hierarchy will probably be the All member. In that case, this function realizes that the current member of the Fiscal hierarchy is the more granular.


**General Note:** The performance benefit gained by using the efficient sets returned from these functions are often outweighed by the performance cost of calculating the more efficient sets. It is suggested that you performance test your MDX with and without these functions. It is also suggested that, if possible, you use a named set in your query so that the efficient set can be reused many times per query without having to be recalculated many times. An example of such a query utilizing a named set would be:

```raw

with

set ToDateSet as ASSP.GetEfficientPeriodsToDateSet("Calendar Year",[Date].[Calendar].[Date].[November 3, 2003])

member [Measures](Measures)(Measures).[Test] as Sum({ToDateSet},[Measures].[Internet Sales Amount])

SELECT {[Measures].[Test]} ON 0,

NON EMPTY {

 [Product].[Product].[Product].Members

 *[Customer].[Country].[Country].Members

} ON 1

FROM [Adventure Works]

```