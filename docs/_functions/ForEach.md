---
title: ForEach
version: "1.3.7"
permissions: U
class: XmlaDiscover
functions:
    - ForEachPartition
    - ForEachMeasureGroup
---
The following will be part of release 1.3.7...

**ForEachPartition(command)**
This function loops through all the partitions in all the measure groups in all the cubes in the current database and runs the command passed in as a parameter and unions the results together. The command can be any valid command (such as MDX or DMV or ASSP sproc) which returns a recordset. It replaces the following parameters (case-sensitive) with the appropriate values for the current partition:

* @DATABASE_NAME
* @DATABASE_ID
* @CUBE_NAME
* @CUBE_ID
* @MEASUREGROUP_NAME
* @MEASUREGROUP_ID
* @PARTITION_NAME
* @PARTITION_ID

For example, this function allows you to run one ```"DISCOVER_PARTITION_STAT"``` command per partition, and union together the resultsets.

```raw
call ASSP.ForEachPartition("
 SELECT *
 FROM SystemRestrictSchema($system.Discover_Partition_Stat
 ,DATABASE_NAME = @DATABASE_NAME
 ,CUBE_NAME = @CUBE_NAME
 ,MEASURE_GROUP_NAME = @MEASUREGROUP_NAME
 ,PARTITION_NAME = @PARTITION_NAME)
")
```

If you wanted to add a new column to the resultset including the PARTITION_ID, could run the following:

```raw
call ASSP.ForEachPartition("
 SELECT *, @PARTITION_ID as PART_ID
 FROM SystemRestrictSchema($system.Discover_Partition_Stat
 ,DATABASE_NAME = @DATABASE_NAME
 ,CUBE_NAME = @CUBE_NAME
 ,MEASURE_GROUP_NAME = @MEASUREGROUP_NAME
 ,PARTITION_NAME = @PARTITION_NAME)
")
```


**ForEachMeasureGroup(command)**
This function loops through all the measure groups in all the cubes in the current database and runs the command passed in as a parameter and unions the results together. The command can be any valid command (such as MDX or DMV or ASSP sproc) which returns a recordset. It replaces the following parameters (case-sensitive) with the appropriate values for the current measure group:

* @DATABASE_NAME
* @DATABASE_ID
* @CUBE_NAME
* @CUBE_ID
* @MEASUREGROUP_NAME
* @MEASUREGROUP_ID

For example, this function allows you to run one ASSP.DiscoverPartitionSlices command per measure group, and union together the resultsets.

```raw
call ASSP.ForEachMeasureGroup("
 call ASSP.DiscoverPartitionSlices(@CUBE_NAME, @MEASUREGROUP_NAME)
")
```
