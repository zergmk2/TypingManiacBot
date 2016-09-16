# TypingManiacBot
Cheat application for Typing Maniac, in a nutshell you have to type the words that are falling down the screen before they reach the bottom, this application automates it.

The application takes a screenshot of a region in the browser control by request, using an image processor library to obtain the location of the word within the screenshot by using the blob detection mechanism, crop the screenshot so only the word is visible, load the cropped screenshot (directly from memory) into the ocr engine to extract the word in plain text, and finally send keystrokes to the browser control.

Major components used:
- AForge.NET, for detecting the blobs inside the browser-shot.
- Nuance OmniPage Capture SDK v20 x86, for integrating the OCR to process the blobs and extract the word from them.
