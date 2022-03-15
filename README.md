# Libp2p-Unity
libp2p (libp2p.io) networking for Unity C# projects

## Purpose

Libp2p-Unity is a Unity 3D package allowing Unity apps written in C# to make use of the [`libp2p`](https://libp2p.io)  peer-to-peer networking stack.

The impetus for doing this is to be able to use `libp2p` as one of the available carrier network layers for apps using the Apian Framework, which implies creating a libp2p implementation of Apian's [P2pNet](https://github.com/Apian-Framework/P2pNet) networking API. It also implies that the main focus will be to makes use of the `libp2p.pubsub` facility in doing so.

At the same time, however, it seemed like the vast majority of the work was going to be to get things to where C# Unity apps were able to talk to `libp2p` at all, and that once that worked the `P2pNet` pubsub implmentation would be pretty easy (that _is_ kinda the point to P2pNet.)

`Libp2p-Unity` is the result: It will be the basis for `P2pNet.libp2p`, but hopefuly will also to be useful to folks who have no interest in Apian or P2pNet, and just want a way to use libp2p in their apps.

