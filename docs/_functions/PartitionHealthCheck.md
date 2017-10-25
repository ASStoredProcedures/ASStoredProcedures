---
title: PartitionHealthCheck
version: "1.2"
permissions: U
class: PartitionHealthCheck
functions:
    - DiscoverPartitionSlices
---

The functions in this class are designed to allow cube administrators to check whether the dataid ranges of the partitions in a measure group overlap.

**DiscoverPartitionSlices(_string cubeName_, _string measureGroupName_)**

This function calls the Discover function in the XMLADiscover class to retrieve the {{DISCOVER_PARTITION_DIMENSION_STAT}} schema rowset for every partition in a measure group, merges the datatables returned into one, and adds an extra column called Overlap which lists the names of the partitions which have dataid ranges which overlap with the current partition for the current dimension.

The following statement returns the partition slice information for the Internet Sales measure group of the Adventure Works cube:
call `assp.DiscoverPartitionSlices("Adventure Works", "Internet Sales")`

More information on using this function is available in a [blog post](http://cwebbbi.spaces.live.com/Blog/cns!7B84B0F2C239489A!1478.entry) by Chris Webb.