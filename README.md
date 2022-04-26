# Libp2p-Unity
libp2p (libp2p.io) networking for Unity C# projects

## Status

It's really, **really** rough. Well, _early_. I'm doing WEBGL targets (compiled to WASM/browser) first partly because browsers are so constrained network-wise: only able to use Websockets and/or WebRTC and not really able to listen for a connection at all. Mostly, though, it's because I already know how to have Unity C# code talk to browser-based Javascript and have already done so with a browserfied version of the Javascript version of libp2p.

## Purpose

Libp2p-Unity is a Unity 3D package allowing Unity apps written in C# to make use of the [`libp2p`](https://libp2p.io) peer-to-peer networking stack for app-level communication.

The impetus for doing this is to be able to use `libp2p` as one of the available carrier network layers for apps using the Apian Framework, which really means creating a libp2p implementation of Apian's [P2pNet](https://github.com/Apian-Framework/P2pNet) networking API. It also implies that the main focus will be to make use of the `libp2p.pubsub` facility in doing so.

At the same time, however, it seemed like the vast majority of the work was going to be to get things to where C# Unity apps were able to talk to `libp2p` at all, and that once that worked the `P2pNet` pubsub implementation would be pretty easy (that _is_ kinda the point to P2pNet.)

`Libp2p-Unity` is the result: It will be the basis for `P2pNet.libp2p`, but hopefully will also to be useful to folks who have no interest in Apian or P2pNet, and just want a way to use libp2p in their apps.

Note that I do not intend to make the entire set of libp2p capabilities available.

## Direction

Given that there are already 3 different versions of `libp2p` (Go, Javascript, Rust) being maintained, it might seem like the best way to do this would be just to write a C# port. For me that's just a non-starter. `libp2p` is huge, with a whole pile of moving parts, and still really in development. What I really want to do with it for Apian, on the other hand, is pretty specific and will use a limited set of those parts.

So instead, I am going to make use of of the Foreign Function Interface capabilities present in C# and the languages that already have libp2p ports. It is demonstrably possible to use a compiled and browserfied version of `js-libp2p` for this when building for the browser. I expect that It'll be possible to do something similar for native build using libraries built from the Go and/or Rust versions.


