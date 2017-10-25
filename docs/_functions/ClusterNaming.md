---
title: ClusterNaming
version: "1.1"
permissions: U
class: ClusterNaming
functions:
    - AutoNameClusters
    - DistinguishingCharacteristicsForClusters
---

When a clustering data mining model is processed, the resulting clusters are named Cluster 1, Cluster 2, etc. The ClusterNaming functions allow you to quickly determine the characteristics that distinguish each cluster from the whole population.

In the Customer Clusters mining model in Adventure Works, the distinguishing characteristics of Cluster 1 are _Home Owner = Yes; Commute Distance = 0-1 Miles; Education = Graduate Degree_. However, the distinguishing characteristics for Cluster 3 are _Marital Status = Single; Yearly Income = 10000 - 30000; Home Owner = No_.

The following DMX query, when run against the Adventure Works DW database, returns a recordset describing as succinctly as possible the distinguishing characteristics of each cluster:

```raw
CALL ASSP.DistinguishingCharacteristicsForClusters("Customer Clusters")
```

The following DMX query, when run against the Adventure Works DW database, renames each cluster so the name reflects that cluster's distinguishing characteristics:

```raw
CALL ASSP.AutoNameClusters("Customer Clusters")
```

**General Note:** The ASSP assembly must be registered with unrestricted permissions for the ClusterNaming functions to succeed.