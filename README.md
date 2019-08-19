# Azure Cognitive Services Keyword Spotting for UWP
This is a workaround to make the Microsoft Azure Cognitive-Services Keyword Spotting work with the UWP application.

Microsoft's Cognitive Serivces do not support keyword spotting on UWP applications:
> Wake Word (Keyword Spotter/KWS) functionality might work with any microphone type, official KWS support, however, is currently limited to the microphone arrays found in the Azure Kinect DK hardware or the Speech Devices SDK.
There is no official support for KWS in UWP: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/scenario-availability.  Where UWP is supported, it is explicitly called out.
we will take it as a feature request into our planning, but for the next SDK update (sometime in September probably).

This is a quote from the Microsoft developers.

In summary, **KWS is not *currenlty* supported in UWP**, so this is a **workaounrd** for now.

## What it does?
A solution I found is to use a background console that listens and does the KWS part.

On startup, the UWP application starts a console process (Listener). The Listener opens a connection with the UWP, and listens for the KWS. Once the Listener recognizes the keyword, it's listening for utterances. Then it sends a request to the UWP with the recognized utterance as a string.

## Media
### Gifs
Keyword spotting:
![Recordit GIF](http://g.recordit.co/nyJbAWWxC1.gif)

KWS toggling:
![Recordit GIF](http://g.recordit.co/h7VOKkLbBm.gif)

Microphone button:
![Recordit GIF](http://g.recordit.co/oG8MV7Wb68.gif)


### Screenshots
UWP view:
<img src="https://i.imgur.com/clQBZe4.png" title="Kws UWP" alt="Kws UWP">

Console (Listener) view:
<img src="https://i.imgur.com/wIkvcKY.png" title="Kws Console" alt="Kws Console">

### Videos
Coming soon

## Setting up

Coming soon.


### Using
Once up and running, use the keyword to activate the listening mode, or use the microphone button. Say something, and the UWP will show the result after it's recognized.
You can use the *Toggle KWS* to toggle on/off the keyword spotting function.

## Credits
Huge thanks to [StefanWickDev] and his work on [UWP-FullTrust]. Check out his blog for more information about UWP FullTrust and communication with other apps.

## License

[![License](http://img.shields.io/:license-mit-blue.svg?style=flat-square)](http://badges.mit-license.org)

[MIT license](http://opensource.org/licenses/mit-license.php), Copyright 2019 © [Tom Milman]

[UWP-FullTrust]: <https://github.com/StefanWickDev/UWP-FullTrust>
[StefanWickDev]: <https://github.com/StefanWickDev>
[Tom Milman]: <https://github.com/tom1milman>
