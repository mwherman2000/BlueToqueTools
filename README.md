﻿# BlueToqueTools DID Toolkit
Trusted Digital Web (TDW) Project, Hyperonomy Digital Identity Lab, Parallelspace Corporation

BlueToqueTools is a collection of software utilities, prototype apps, proof-of-concept (PoC) apps, and demonstration apps used
to support, validate, and/or demonstrate key features of
the Blue Toque family of Fully Decentralized Object (FDO) Framework specifications.

![Blue Toque](images/bluetoquelogo2.jpg)

## BlueToqueTools Tool Kit Contents

- `didlang` Language Command Line Interpreter for DID Identifiers, DID Documents, DID Agents, and DID Objects version 0.4

## didlang Language Command Line Interpreter for DID Identifiers, DID Documents, DID Agents, and DID Objects version 0.4

`didlang` is a new interpreted, command line language for working with DID Identifiers, DID Documents, DID Agents, and DID Objects.

### CRUD Commands

#### Read Commands (Indirection) Operator)

- Enter `help` to redisplay this list of commands.
- Enter `!help` to see a list of command shortcuts.
- Enter `<did>` to verify a DID Identifier (no indirection).
- Enter `*<did>` to return the DID Document associated with the DID Identifier ("single indirection").
- Enter `**<did>` to return the Agent Scred (VC) associated with the DID Identifier ("double indirection").
- Enter `***<did>` to return the Object Scred (VC) associated with the DID Identifier ("triple indirection").

#### Create/Update Commands (Plus Operator)

- Enter `+did:<method name>` to register a new DID Method name - fails if the DID Method name already has been registered.
- Enter `+did:<method name>:<idstring>` to (re)register a new DID Document with a single (1) default serviceEndpoint 
(DID Agent) - configured with a default DID Agent implementation as well as pre-deleting the previous DID Document if it already exists.
- Enter `+did:<method name>:<idstring> type=clustered,roundrobin,BlueToque.Agent agents=<N>` to (re)register a new DID Document with multiple serviceEndpoints (DID Agents) - each preconfigured with a default DID Agent implementation as well as pre-deleting the previous DID Document if it already exists.
- Enter `++did:<method name>:<idstring>` to (re)create a new DID Agent (Structured Credential) describing the Agent's interfaces and the interfaces' methods.
- Enter `+++did:<method name>:<idstring>` to (re)create in DID Storage a new DID Object (Structure Credential) with no properties.
- Enter `++++did:<method name>:<idstring> Name1="Value1" ...` to add or update one or more named properties from a DID Object[1].

#### Delete Commands (Minus Operator)

- Enter `-did:<method name>` to deregister a new DID Method name - fails if the DID Method namespace contains existing
DID Documents, DID Agents, or DID Objects.
- Enter `-did:<method name>:<idstring>` to delete an existing DID Document and an associated DID Agent and DID Object (Structured Credentials), if they exist
- Enter `--did:<method name>:<idstring>` to delete an existing DID Agent (Structured Credential).
- Enter `---did:<method name>:<idstring>` to delete from DID Storage an existing DID Object (Structure Credential) - including all contained properties.
- Enter `----did:<method name>:<idstring> Name1 ...` to delete one or more named properties from an existing DID Object[1]. 

#### Advanced Commands (Coercion Operator)

- To override the default selectors for the Agent service endpoint and/or the Agent interface and interface method to be called (selector coercion), enter
```
*(serviceEndpointType^serviceEndpointId)*did
```
returns Agent Scred for an alternative Agent service endpoint in the DID Document.
```
**(serviceEndpointType^serviceEndpointId)*did
```
returns result of calling the default interface method on an alternative Agent service endpoint in the DID Document.
```
*(agentInterface:agentMethod)**did
```
returns the result of calling an alternative interface method on the default Agent service endpoint.
```
*(agentInterface^agentMethod)*(serviceEndpointType^serviceEndpointId)*did
```
returns the result of calling an alternative interface method on an alternative Agent service endpoint in the DID Document (double coercion).

### Command Shortcuts

```
- !0      = did:example:1234
- !r      = did:color:red
- !1      = *did:color:red
- !2      = **did:color:red
- !3      = ***did:color:red
- !red    = ***did:example:red
- !green  = ***did:example:green
- !blue   = ***did:example:blue
- !colors = display a list of the registered did:color DID Objects
- !help   = redisplay this list of command shortcuts
- help    = display a list of top-level commands
```

### Screenshot

![didlang screenshot](images/didlang-webcast-0.2.png)

### Build

- Build `BlueToqueTools.sln` using a recent version of Visual Studio 2022.
- Click `Debug -> Start New Instance` to run 
the `didlang Language` command line interpreter.

## Context

![Trusted Digital Web and the Decentralized OSI Model 0.7 – December 28, 2021](/images/TDW-DID%20Method%20Spaces%200.7.png)

## References

[1] In future versions of `didlang`, the syntax for Create/Update and Delete commands will evolve from:
```
++++did:<method name>:<idstring> Name1="Value1" ...
----did:<method name>:<idstring> Name1 ...
```
to also include:
```
+***did:<method name>:<idstring> Name1="Value1" ...
-***did:<method name>:<idstring> Name1 ...
```
That is, _indirection_ should first be used to Read the DID Document, DID Agent, or DID Object entity
and then `+` or `-` will then act appropriately on the returned entity.