
# TypingManiacBot
Cheat application for Typing Maniac. In a nutshell, you have to type the words that are falling down the screen before they reach the bottom. This application automates it.

The application constantly takes a screenshot of the browser control, process the frame using an image processor library to obtain the location of the blob (word) within the screenshot by using the object detection mechanism, crop the screenshot so only the word is visible, load the cropped screenshot (directly from memory) into the ocr engine to extract the word in plain text, and finally send keystrokes to the browser control.

# Dependencies
- [Accord.NET](http://accord-framework.net/) - *for detecting the blobs inside the browser-shot*.
- [Nuance OmniPage Capture SDK](https://www.nuance.com/print-capture-and-pdf-solutions/optical-character-recognition/omnipage/omnipage-capture-sdk-for-windows.html) - *for integrating the OCR to process the blobs and extract the word in plain text from them*.

# Demo Preview

![Preview](demo.gif)
