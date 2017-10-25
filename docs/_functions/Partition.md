---
title: Partition
version: "1.1"
permissions: U
class: Partition
functions:
    - CreatePartitions
    - CreateDistinctCountPartitions
    - CreateStringDistinctCountPartitions
---

The CreatePartitions function allows an Analysis Services administrator to quickly prototype various partitioning schemes. For example, you could quickly build one partition per year by running the following:

```raw
call ASSP.CreatePartitions("Adventure Works", "Internet Sales", "[Date].[Calendar Year].[Calendar Year].Members")
```

Or you could do more complex partitioning schemes. If you wanted to partition by years through 2003 then months after that, you could run the following:

```raw
call ASSP.CreatePartitions("Adventure Works", "Internet Sales", "{null:[Date].[Calendar].[CY 2003]} + {[Date].[Calendar].[January 2004]:null}")
```

Or you could partition on year and product category:

```raw
call ASSP.CreatePartitions("Adventure Works", "Internet Sales", "[Date].[Calendar Year].[Calendar Year].Members * [Product].[Category].[Category].Members")
```

Or you could partition in a more ad-hoc manner by creating your own groupings. For instance, if you wanted to partition by the first character of the product name, you could run:

```raw
call ASSP.CreatePartitions("Adventure Works", "Internet Sales", "[Product].[Product].[Product].Members", "Left([Product].[Product].CurrentMember.Name,1)")
```

Finally, if you want to go back to a single partition, you can run the following:

```raw
call ASSP.CreatePartitions("Adventure Works", "Internet Sales", "[Date].[Calendar Year].[All Periods]")
```

The signature of this function is:

**CreatePartitions(_CubeName_, _MeasureGroupName_, _SetString_, _\[PartitionGrouperExpressionString]_)**

Run this function from the database containing the cube you are trying to modify. The first parameter (_CubeName_) is a string specifying which cube to modify. The second parameter (_MeasureGroupName_) is a string specifying the measure group to modify. The third parameter (_SetString_) is a string which is a valid MDX set expression. For each tuple in that set, one partition will be created (unless a fourth parameter is specified). The optional fourth parameter (_PartitionGrouperExpressionString_) is a string which is a valid MDX string expression. If specified, the tuples will be grouped according to the value of this expression such that one partition will be built per unique grouper value.


#### Distinct Count Partitioning

ASSP also allows you to partition distinct count measure groups in a manner that will achieve [optimal performance](http://www.microsoft.com/downloads/details.aspx?FamilyID=65DF6EBF-9D1C-405F-84B1-08F492AF52DD&displaylang=en) by creating partitions which have non-overlapping ranges of distinct count values. This segmentation can be done on top of any partitioning mentioned above.

You could partition by year then further partition into 4 sub-partitions automatically based on the range of data currently in the dimension that corresponds to the distinct count measure (i.e. the Customer dimension in this Adventure Works example).

```raw
call ASSP.CreateDistinctCountPartitions("Adventure Works", "Internet Customers", "[Date].[Calendar Year].[Calendar Year].Members", "", 4)
```
1. where CustomerKey <= 15620
1. where CustomerKey between 15621 and 20240
1. where CustomerKey between 20241 and 24860
1. where CustomerKey >= 24861

Or you could specify the range of int/bigint values and then subdivide that range into 3 equal segments:

```raw
call ASSP.CreateDistinctCountPartitions("Adventure Works", "Internet Customers", "[Date].[Calendar Year].[Calendar Year].Members", "", 3, 10000, 30000)
```
1. where CustomerKey <= 16666
1. where CustomerKey between 16667 and 23332
1. where CustomerKey >= 23333

The signature of this function is:

**CreateDistinctCountPartitions(_CubeName_, _MeasureGroupName_, _SetString_, _PartitionGrouperExpressionString_, _NumSubPartitions_, _\[MinDistinctValue]_, _\[MaxDistinctValue]_)**

Run this function from the database containing the cube you are trying to modify. The first parameter (_CubeName_) is a string specifying which cube to modify. The second parameter (_MeasureGroupName_) is a string specifying the measure group to modify. The third parameter (_SetString_) is a string which is a valid MDX set expression. For each tuple in that set, one partition will be created (unless a fourth parameter is specified). If only wanting to partition on the distinct count value, simply specify an All member. The fourth parameter (_PartitionGrouperExpressionString_) is a string which is a valid MDX string expression. If specified (i.e. if not an empty string), the tuples will be grouped according to the value of this expression such that one partition will be built per unique grouper value. The fifth parameter (_NumSubPartitions_) specifies how many further sub-partitions to divide each previous partition into based on non-overlapping ranges of distinct count values. The optional sixth and seventh parameters (_MinDistinctValue_ and _MaxDistinctValue_) let you specify the range of distinct count values which will be used instead of having ASSP scan the dimension that corresponds to the distinct count measure to determine the range.

> **Restriction:** To use CreateDistinctCountPartitions, your distinct count measure must be an int or bigint (or the unsigned equivalent) data type. If you do not specify the MinDistinctValue and MaxDistinctValue parameters, your measure group must have a dimension tied to the column that the distinct count measure is based upon. ASSP will scan this dimension to determine the min and max distinct count value. (Actually, to ensure good performance, it only scans the first and last million members looking for the min and max value, respectively.)


You can also do distinct count partitioning based on strings by specifying the boundary values. This example partitions by year, then further partitions into 3 sub-partitions:
1. where SalesOrderNumber < 'SO49410'
1. where SalesOrderNumber >= 'SO49410' and SalesOrderNumber < 'SO69410'
1. where SalesOrderNumber >= 'SO69410'

```raw
call ASSP.CreateStringDistinctCountPartitions("Adventure Works", "Internet Orders", "[Date].[Calendar Year].[Calendar Year].Members", "", "SO49410", "SO69410")
```

The signature of this function is:

**CreateStringDistinctCountPartitions(_CubeName_, _MeasureGroupName_, _SetString_, _PartitionGrouperExpressionString_, _BoundaryValue 1 - 9_)**

Run this function from the database containing the cube you are trying to modify. The first parameter (_CubeName_) is a string specifying which cube to modify. The second parameter (_MeasureGroupName_) is a string specifying the measure group to modify. The third parameter (_SetString_) is a string which is a valid MDX set expression. For each tuple in that set, one partition will be created (unless a fourth parameter is specified). If only wanting to partition on the distinct count value, simply specify an All member. The fourth parameter (_PartitionGrouperExpressionString_) is a string which is a valid MDX string expression. If specified (i.e. if not an empty string), the tuples will be grouped according to the value of this expression such that one partition will be built per unique grouper value. The subsequent parameters provide the boundary values for sub-partitioning on the distinct count column. You may specify one to nine boundary values meaning you can have up to ten distinct count sub-partitions.



#### Notes

**General Note:** The ASSP assembly must be registered with unrestricted permissions for the CreatePartitions/CreateDistinctCountPartitions/CreateStringDistinctCountPartitions function to succeed.

**Important Note:** You may want to backup your cube before running this code as it deletes all the existing partitions in the measure group you are partitioning.

**Best Practices:**

* This function is not intended to be used for more than prototyping and quickly testing various partitioning schemes.
* This function deletes existing partitions and creates new ones matching the partitioning scheme you have specified. After the function completes, you need to right click on the measure group in Management Studio and do a Full Process to process all these new partitions. It should probably not be used in a production environment because running this function will cause you to have to reprocess all partitions in the measure group.
* Be sure that the set you specify as the third parameter exactly covers 100% of the fact table. No tuples in that set should contain duplicate data. Nor should there be any fact rows which do not appear in one of those tuples.
* This function does not provide any mechanisms for planning for members which will be added in the future. For instance, when you arrive at a new year and add a new Year member to your Date dimension, if you have partitioned by year using this function, no partition for that new year will already exist.
* The cube you are partitioning must be processed before you run the CreatePartitions/CreateDistinctCountPartitions/CreateStringDistinctCountPartitions function. At a minimum, run a Process Structure command on the cube to get it in the "processed" state.