---
title: FileSystemCache
version: "1.3"
permissions: U
class: FileSystemCache
functions:
    - ClearAllCaches
    - ClearFileSystemCache
---

Analysis Services uses the Windows File System Cache to improve performance of reading files off disk. In performance testing MDX, clearing the Windows File System Cache may allow for more accurate cold-cache tests.

To clear the active and [standby](http://www.artisconsulting.com/blogs/greggalloway/Lists/Posts/Post.aspx?ID=19) Windows File System Cache, run the following:

```raw
call ASSP.ClearFileSystemCache()
```

To clear just the active Windows File System Cache, run the following:

```raw
call ASSP.ClearFileSystemCache(false)
```

You can monitor the size of the active Windows File System Cache by looking at "Memory\Cache Bytes" (and Cache Bytes Peak) in perfmon. You can monitor the size of the standby Windows File System Cache by looking at the "Memory\Standby Cache Normal Priority Bytes" perfmon counter. Or you can run the following sproc which returns the current Cache Bytes (in bytes and in GB), Cache Bytes Peak (in bytes and GB), and Standby Cache (in bytes and GB):

```raw
call ASSP.GetFileSystemCacheBytes()
```

**Important Note:** Be sure to also use the [ClearCache](../ClearCache) function to clear the Analysis Services cache. This is separate from the Windows System File Cache. Better yet, run ClearAllCaches which clears the Analysis Services cache and the active and standby file system cache in one sproc call:

```raw
call ASSP.ClearAllCaches()
```


**Note:** To keep the active Windows System File Cache trimmed, set the LimitSystemFileCacheSizeMB setting in msmdsrv.ini instead of using this ASSP function.

**Note:** Starting with release 1.3.5, these stored procedures account for the standby file system cache properly.

**Note:** It is not recommended you use this ASSP function on a production system as SSAS will need to read the files off disk instead of from the Windows System File Cache.

**Note:** The account/impersonation used to run these stored procs needs adequate permissions ({"SE_INCREASE_QUOTA_NAME and SE_PROFILE_SINGLE_PROCESS_NAME"}, to be specific, but Administrator permissions to be more simplistic). A common mistake is running the sproc from Management Studio on the server without running Management Studio as administrator when you open it.