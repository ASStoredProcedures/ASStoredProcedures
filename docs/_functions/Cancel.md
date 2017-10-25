---
title: Cancel
version: "1.1"
permissions: U
class: XmlaDiscover
functions:
    - CancelConnection
    - CancelSession
    - CancelSPID
---

The cancel functions allow for the cancelling of the specified object (similar to the kill command in SQL Server). You can use the various [discover](discover) functions to get a list of currently active sessions or connections.

**CancelConnection(connectionID)**
Cancels the specified Connection based on its connection id.

**CancelSPID(SPID)**
Cancels the specified SPID

**CancelSession(sessionGuid)**
Cancels the session for the specified session GUID
