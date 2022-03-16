
- To build the browser library:

    npx parcel build  ./browser-libp2p.js

        just writes library and map - but doesn't mess with the name
        
- To import in a browser:

  <script src="browser-libp2p.js" defer></script>
  
  ...and a magic `window.Libp2pObj` object will be available to you, defined as:
  
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
 
**** NOTE: this is a bit of a crappy way to do this. It would be  better if `browser-libp2p.js` was an ES module that 
had the object as a default export and the browser could import it like a modern browser does. 

    My experiments with the parcel bundler, however, resulted in ES modules that had bizarre undefined symbol issues 
    internal to the resulting module.

... or it would still be better if `browser-libp2p.js` set a global var instead of magically adding a property 
to `window`.

    Tried this, too - and was unable to get parcel to NOT mangle the resulting var name.
 
  
- To use window.Libp2pObj from a js module:

const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,
    Bootstrap,  GossipSub,  FloodSub, fromString, toString }  = window.Libp2pObj;

 ... then just code as if you had imported the lib individually/    
 
- Some caveats (pretty much all about bundling and parcel):

    - I tried webpack and ran into a super-verbose wall-of-error regarding the (current at the time) nodeJS version.
        
        Parcel is what is used in the `js-libp2p` github "libp2p in a browser" example.
        
    - I tried to be very specific in telling parcel what to do so it didn't have to try to infer so much, but never had
      any real success. 
    
         I only ever got it to work by setting it up the way it is - as if it were a completely minimal web app
         consisting of `index.html` and `browser-libp2p.js` and specifying almost nothing explicitly. Interestingly,
         set up this way it cannot build the web app. But it will build just the JS library if you type:
            $ npx parcel build ./browser-libp2p.js
            
    - Be very careful messing with the dummy `index.html` file. If you, for instance, modify the import to be a 
        "module" import it'll completely blow up the JS lib generation. Really.
        
    - Someday I'll try to fix some of this, but it's so painful I'm not going to be in a hurry to do it.
    
    - I'm not POSITIVE it's important, but somehow I've come into the habit of deleting the .parcel-cache folder 
      before every build.
    
    - OH: Be VERY careful about upgrading packages in package.json. I mean, it SHOULD happen, but... never mind.
    
    
 
            
         
 