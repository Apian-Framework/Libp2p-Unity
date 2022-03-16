
# Browserfied `js-libp2p`

In order to make use of libp2p in a Unity WEBGL (browser) build it is necessary to use the javascript `js-libp2p` implementation. Because `js-libp2p` is built on Node.js it is also necessary to create a "browserfied" version which contains the parts of Node used by the code.

<br/>

## To build the browser library:

  *Node.js must be installed on your computer in order to do this*

<br>

`browser-libp2p` is based in the ["libp2p-in-the-browser" example](https://github.com/libp2p/js-libp2p/tree/master/examples/libp2p-in-the-browser) in the `js-libp2p` repository, which makes use of the [Parcel Bundler](https://parceljs.org/) to do the bundling and browserification.

<br />

`npm install`
> Do this initially to install all of the node modules

<br >


`npx parcel build  ./browser-libp2p.js`
> just writes library and map - but doesn't mess with the name<br />

<br >

## To import in a browser:
<br >

Add  `<script src="browser-libp2p.js" defer></script>` to your HTML
and a magic `window.Libp2pObj` object will be available to you, defined as:
```
window.Libp2pObj = {
  Libp2p: Libp2p,
  Websockets: Websockets,
  WebRTCStar: WebRTCStar,
  NOISE: NOISE,
  Mplex: Mplex,
  Bootstrap: Bootstrap,
  GossipSub: GossipSub,
  FloodSub: FloodSub,
  fromString: fromString,
  toString: toString,
}
```

> **** NOTE: this is a bit of a crappy way to do this. It would be  better if browser-libp2p.js` was an ES module that had the object as a default export and the browser could import it like a modern browser does.

    My experiments with the parcel bundler, however, resulted in ES modules that had bizarre undefined symbol issues internal to the resulting module.

>... or it would still be better if `browser-libp2p.js` set a global var instead of magically adding a property
to `window`.

    Tried this, too - and was unable to get parcel to NOT mangle the resulting variable name.

<br />

## To use window.Libp2pObj from a js module:

<br />

```
const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,
    Bootstrap,  GossipSub,  FloodSub, fromString, toString }  = window.Libp2pObj;
```
 ... then just code as if you had imported the libs individually


## Some caveats (pretty much all about bundling and parcel):

<br />

- I tried webpack and ran into a super-verbose wall-of-error regarding the (current at the time) nodeJS version.

  Parcel is what is used in the `js-libp2p` github "libp2p in a browser" example.

- I tried to be very specific in telling parcel what to do so it didn't have to try to infer so much, but never had any real success.

  I only ever got it to work by setting it up the way it is - as if it were a completely minimal web app consisting of `index.html` and `browser-libp2p.js` and specifying almost nothing explicitly. Interestingly, set up this way it cannot build the web app. But it will build just the JS library if you type:
   >`npx parcel build ./browser-libp2p.js`

- Be very careful messing with the dummy `index.html` file. If you, for instance, modify the import to be a "module" import it'll completely blow up the JS lib generation. Really.

- Someday I'll try to fix some of this, but it's so painful I'm not going to be in a hurry to do it.

- I'm not POSITIVE it's important, but somehow I've come into the habit of deleting the .parcel-cache folder before every build.

- Oh: (_and this shows what a Node noob I am_) Be VERY careful about upgrading packages in package.json. I mean, it SHOULD happen, but... never mind.







