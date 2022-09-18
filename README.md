# Flood Quest

Rescue people and make your way through the flood.

Gesture-based interactive flooding scenario for the HoloLens.

This repo is quite messy, the code not optimized and we need some sleep ;-)

https://drive.google.com/file/d/1Yg7lN5_277wIsYLTLsfb6DGP4erOIsvQ/view?usp=sharing


## Inspiration

“Once in a century floods” happen once every decade or so. If we look at the city of Dresden, Germany, a once in two-hundred years flood happened in 2002 and heavily damaged the city. Major flood events followed up in the city later again in 2006 and in 2013. Similarly, in 2021, major flooding happened in western Germany and across large parts of western Europe. These events are risky to the lives and livelihood of people and are only increasing over time. We need to be prepared for such events.

One of the ways to prepare yourself is to put yourself in the situation repetitively and in a safe setup. To achieve this, we unleash the potential of virtual reality. Further, to make people do something repetitively, it has to be somehow fun or fulfilling. Taking this into account, we propose a game that while being fun also prepares people for such risky situations.

What if there were to be a sudden flash flood in the Technopark right now? 😱

## What it does

We have created a virtual reality game that works on Hololens. The object of the game is to save as many objects as you can from the incoming floodwaters. The interface is gamified with a varied point system. With the VR experience, a good playing environment and gamification rules, we hope people use the game often and thus train themselves for emergency situations.
How we built it

We built a Mixed Reality game with Unity and ran it in Hololens. In the game there is gamification system, where users can collect points for saving objects from floods. More dangerous the object is, the more is the point. We built REST API in Django to fetch objects points and store user scores.

## Challenges we ran into

As we are building a game with Hololens, the first challenge we ran into is interfacing with the Hololens and controlling an app via gestures.

Furthermore, we tried to take an image of Technopark building and diffuse the image with a flood as if what it will look like if there was a flood in Technopark area. Building this image with proper API call and parameters, parsing the image and using the modified image in the Unity scene was a challenge which we overcame by building a Flask API.

## Accomplishments that we're proud of

We successfully integrated the Unity game with Hololens and could call REST APIs from it.
What we learned

From a design perspective, we learned how to do level design on a basic level. The biggest learning for all of us was to build app for Hololens for the first time. It was an excellent learning opportunity building for Hololens for Unity development, designing and consuming REST APIs.

## What's next for Flood Quest

We will be working in this game to make it more attractive to kids and for a larger global audience. People everywhere should be trained how to respond when a flash flood occurs. We are planning to gather more data from different regions and terrains and improve our game's experience by learning from those data models. We are also planning to make this game available for web and mobile app for those who do not have access to Hololens.

## Our CreatorSpace profile

https://app.creatorspace.dev/deepjyotipaul/projects/qnN8MqrC1JnyNjPm

## Youtube
https://www.youtube.com/watch?v=a7ctDzPqQrY
