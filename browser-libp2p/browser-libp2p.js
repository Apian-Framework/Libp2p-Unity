import 'babel-polyfill'
import Libp2p from 'libp2p'
import Websockets from 'libp2p-websockets'
import WebRTCStar from 'libp2p-webrtc-star'
import { NOISE } from '@chainsafe/libp2p-noise'
import Mplex from 'libp2p-mplex'
import Bootstrap from 'libp2p-bootstrap'
import GossipSub from 'libp2p-gossipsub'
import FloodSub from 'libp2p-floodsub'
import PubsubPeerDiscovery from 'libp2p-pubsub-peer-discovery'
import { PeerId, createFromB58String } from 'peer-id'

import {fromString as uint8arrayFromString} from 'uint8arrays/from-string'
import {toString as uint8arrayToString} from 'uint8arrays/to-string'

// Method #1: put directly into browser global scope
// window.Libp2p = Libp2p
// window.Websockets = Websockets
// window.WebRTCStar = WebRTCStar
// window.NOISE = NOISE
// window.Mplex = Mplex
// window.Bootstrap = Bootstrap
// window.GossipSub = GossipSub
// window.FloodSub = FloodSub
// window.fromString = fromString
// window.toString = toString

// Method #2: Put an object into window spoe, and destructure it where needed
// ie:
// const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,  Bootstrap,
//         GossipSub,  FloodSub, fromString, toString } = window.Libp2pObj;


window.Libp2pObj = {
  Libp2p: Libp2p,
  Websockets: Websockets,
  WebRTCStar: WebRTCStar,
  NOISE: NOISE,
  Mplex: Mplex,
  Bootstrap: Bootstrap,
  PubsubPeerDiscovery: PubsubPeerDiscovery,
  GossipSub: GossipSub,
  FloodSub: FloodSub,
  uint8arrayFromString: uint8arrayFromString,
  uint8arrayToString: uint8arrayToString,
  PeerId: PeerId,
  createFromB58String: createFromB58String
}


