# TypingManiacBot
Cheat application for Typing Maniac. In a nutshell, you have to type the words that are falling down the screen before they reach the bottom. This application automates it.

The application constantly takes a screenshot of the browser control, process the frame using an image processor library to obtain the location of the blob (word) within the screenshot by using the object detection mechanism, crop the screenshot so only the word is visible, load the cropped screenshot (directly from memory) into the ocr engine to extract the word in plain text, and finally send keystrokes to the browser control.

*Major components used*:
- **Accord.NET** - *for detecting the blobs inside the browser-shot*.
- **Nuance OmniPage Capture SDK v20 x86** - *for integrating the OCR to process the blobs and extract the word from them*.

**FAQ**

**Q**: Why did you choose to use Nuance's ocr engine out of all the publicly free engines out there?

**A**: Because it is the ONLY ocr engine that supports all blob colors without special preprocessing to the image.

![Preview](https://media.giphy.com/media/3kD720uJrRZXKCWLAf/giphy.gif)
