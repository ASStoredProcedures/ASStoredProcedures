---
title: Don't Deadlock Yourself
---

Consider this situation...

* An MDX query is executed which calls an ASSP sproc.
* Cube processing finishes and asks for a commit lock. The lock cannot be granted until the MDX query completes.
* The ASSP sproc then opens a connection to Analysis Services with AMO or AdomdClient. This new connection is blocked by #2 which is blocked by #1. A deadlock is reached.
* Even when ForceCommitTimeout expires (the duration allowed for cube processing commits to be blocked), Analysis Services is not (currently) able to cancel the MDX query unless the sproc is specifically coded to handle this situation.

The following diagram illustrates this deadlock:

![](/img/Don't-Deadlock-Yourself/Don't Deadlock Yourself_deadlock.png)


**Where The Problem Can Occur**

It appears that any MDX query which calls a sproc, or any CALL statement, will take a lock on the database. Thus, any processing of any object in that database will be blocked until the MDX query or CALL statement completes. Thus, any sproc which opens a connection to that database may cause a deadlock. However, if the sproc opens a connection with the * connection string which opens a connection on the same session as the parent query (see the [PartitionHealthCheck.DiscoverPartitionSlices](https://github.com/ASStoredProcedures/ASStoredProcedures/blob/master/ASSP/PartitionHealthCheck.cs) for an example), there will be no deadlock issue as everything is running under one connection which SSAS is able to cancel after ForceCommitTimeout expires.


**Working Around The Problem**

Until this SSAS bug is fixed and SSAS is able to [kill a sproc after ForceCommitTimeout expires](https://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=510567), we'll have to be content with working around the problem.

Inside the stored procedure, the key to handling the above situation is calling Context.CheckCancelled() frequently _during_ the execution of any APIs or queries which may get hung. Therefore, multi-threading is needed so that one thread can be running the APIs or queries, and another thread can be running Context.CheckCancelled(). The Context.CheckCancelled() thread must be the foreground thread in which the sproc was called, not a background thread.

In the ASSP code, there is a [TimeoutUtility](https://github.com/ASStoredProcedures/ASStoredProcedures/blob/master/ASSP/Utilities/TimeoutUtility.cs) class which can help in many situations. For example, when executing a drillthrough command using AdomdClient, the TimeoutUtility.ConnectAdomdClient and TimeoutUtility.FillAdomdDataAdapter methods encapsulate all the logic around threading and calling Context.CheckCancelled() frequently. For instance, look at the [Drillthrough.ExecuteDrillthroughAndFixColumns](https://github.com/ASStoredProcedures/ASStoredProcedures/blob/master/ASSP/Drillthrough.cs) sproc.

For sprocs called from an MDX query, if you want to use AMO, you cannot use the * connection string (as the * connection string can only be used from a CALL sproc). Since every function call of AMO code could get hung due to the deadlock, you will have to run AMO code (in the situation described in this paragraph) in a background thread. See [CubeInfo](https://github.com/ASStoredProcedures/ASStoredProcedures/blob/master/ASSP/CubeInfo.cs) as an example, and notice the delegate function usage.


**ASSP and This Problem**

Prior to the 1.3.1 release, this problem could be seen in the CubeInfo, Drillthrough, Parallel, Discover, and DMV functions. With the (forthcoming) 1.3.1 release, we believe these problems are fixed. We're not aware of any other ASSP sprocs that will cause this problem, except some administrative sprocs like Partition.CreatePartitions. Let us know on the Issue Tracker tab if you run into the above situation with any other sprocs.