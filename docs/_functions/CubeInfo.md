---
title: CubeInfo
version: "1.0"
permissions: U
class: CubeInfo
functions:
    - GetMeasureGroupLastProcessedDate
    - GetLastProcessedDateOverPartitions
---

The function in the CubeInfo class is GetCubeLastProcessedDate. This function will return the date when the current cube was last processed. The following is an example MDX query against the sample Adventure Works cube:

```raw
with member [Measures].[LastProcessed] as ASSP.GetCubeLastProcessedDate()
select [Measures].[LastProcessed] on 0
from [Adventure Works]
```

If this function is used, the ASSP assembly will have to be registered in Analysis Services with unrestricted permissions.

### A new stored proc in release 1.3 is GetMeasureGroupLastProcessedDate("MeasureGroupName"). 
If the different fact tables have a different refresh schedule, this stored proc can be helpful for reporting the last updated time for certain measure groups. This sproc only looks at the metadata from the measure group itself rather than looking at the last processed date on each individual partition. Processing one partition may not be reflected in the date returned by this sproc.

### A new sproc in release 1.3.7 called GetLastProcessedDateOverPartitions
Courtesy of Andrej Kuklin, this function will scan partitions you specify to find the most recent processed date. The following signatures of this function are available:

**GetLastProcessedDateOverPartitions()** - Reports the max processed date on any partition in the cube. If a measure group itself has a more recent processed date, that date is returned.

**GetLastProcessedDateOverPartitions(_IncludeMeasureGroupLastProcessed_)** - If true is passed in as the parameter, same as above. If false is passed in, then it does not check the measure group last processed date, just the partition last processed dates. Note that running ProcessUpdate on a dimension without ProcessAffectedObjects will update the measure group's last processed date, but not every partition's last processed date.

**GetLastProcessedDateOverPartitions(_MeasureGroupName_, _IncludeMeasureGroupLastProcessed_)** - Same as above except it only scans partitions in the specified measure group in the current cube.

**GetLastProcessedDateOverPartitions(_MeasureGroupName_, _PartitionName_, _IncludeMeasureGroupLastProcessed_)** - Same as above except it only scans the one partition in the one measure group specified in the current cube.

**GetLastProcessedDateOverPartitions(_CubeName_, _MeasureGroupName_, _PartitionName_, _IncludeMeasureGroupLastProcessed_)** - Same as above except this function signature allows you to scan partitions in another cube other than the current cube. For this function signature, you can specify empty string for the MeasureGroupName or PartitionName parameters if you want to scan multiple partitions in another cube.

Some example usages against Adventure Works are below:

```raw
with
member [Measures].[Cube Last Processed]
 as ASSP.GetCubeLastProcessedDate()
member [Measures].[Measure Group Last Processed]
 as ASSP.GetMeasureGroupLastProcessedDate("Internet Sales")
member [Measures].[Most Recent Partition Whole Cube With Measure Group]
 as ASSP.GetLastProcessedDateOverPartitions()
member [Measures].[Most Recent Partition Whole Cube]
 as ASSP.GetLastProcessedDateOverPartitions(false)
member [Measures].[Most Recent Partition In Measure Group]
 as ASSP.GetLastProcessedDateOverPartitions("Internet Sales", true)
member [Measures].[Partition Processed Date]
 as ASSP.GetLastProcessedDateOverPartitions("Internet Sales", "Internet_Sales_2008", true)
member [Measures].[Partition Processed Date Specific Cube]
 as ASSP.GetLastProcessedDateOverPartitions("Adventure Works", "Internet Sales", "Internet_Sales_2008", true)
select {
 [Measures].[Cube Last Processed]
,[Measures].[Measure Group Last Processed]
,[Measures].[Most Recent Partition Whole Cube With Measure Group]
,[Measures].[Most Recent Partition Whole Cube]
,[Measures].[Most Recent Partition In Measure Group]
,[Measures].[Partition Processed Date Specific Cube]
,[Measures].[Partition Processed Date]
}
on 0
from [Adventure Works]
```