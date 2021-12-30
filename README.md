# BlueToqueTools
Trusted Digital Web (TDW) Project, Hyperonomy Digital Identity Lab, Parallelspace Corporation

BlueToqueTools is a collection of software utilities, prototype apps, proof-of-concept (PoC) apps, and demonstration apps used
to support, validate, and/or demonstrate key features of
the Blue Toque family of Fully Decentralized Object (FDO) Framework specifications.

![Blue Toque](images/bluetoquelogo2.jpg)

## BlueToqueTools Tool Kit Contents

- `didlang` Language Command Line Interpreter for DID Identifiers, DID Documents, DID Agents, and DID Objects

## didlang Language Command Line Interpreter for DID Identifiers, DID Documents, DID Agents, and DID Objects

`didlang` is a new interpreted, command line language for working with DID Identifiers, DID Documents, DID Agents, and DID Objects.

### Commands

- Enter a `<did>` to verify a DID Identifier (no indirection).
- Enter `*<did>` to return the DID Document associated with the DID Identifier ("single indirection").
- Enter `**<did>` to return the Agent Scred (VC) associated with the DID Identifier ("double indirection").
- Enter `***<did>` to return the Object Scred (VC) associated with the DID Identifier ("triple indirection").
- Enter `help` to redisplay this list of commands.
- Enter `!help` to see a list of command shortcuts.

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

### Build

- Build `BlueToqueTools.sln` using a recent version of Visual Studio 2022.
- Click `Debug -> Start New Instance` to run 
the `didlang Language` command line interpreter.

## Context

![Trusted Digital Web and the Decentralized OSI Model 0.7 – December 28, 2021](/images/TDW-DID%20Method%20Spaces%200.7.png)
