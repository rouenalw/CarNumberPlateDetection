# import the necessary packages
from pytesseract import Output
import pytesseract
import cv2
from imutils.video import VideoStream

n = 0
currentText = ""
pytesseract.pytesseract.tesseract_cmd = 'C:/Program Files/Tesseract-OCR/tesseract'
TESSDATA_PREFIX = 'C:/Program Files/Tesseract-OCR/tessdata'
vs = VideoStream(src=0).start()
while True:
        #load the input image, convert it from BGR to RGB channel ordering,
        # and use Tesseract to localize each area of text in the input image
        image = vs.read()
        rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        results = pytesseract.image_to_data(rgb, output_type=Output.DICT, lang="eng")


        # loop over each of the individual text localizations
        for i in range(0, len(results["text"])):
                # extract the bounding box coordinates of the text region from
                # the current result
                x = results["left"][i]
                y = results["top"][i]
                w = results["width"][i]
                h = results["height"][i]
                # extract the OCR text itself along with the confidence of the
                # text localization
                text = results["text"][i]
                conf = int(results["conf"][i])

                # filter out weak confidence text localizations
                if conf > 1:
                        # display the confidence and text to our terminal
                        print("Confidence: {}".format(conf))
                        print("Text: {}".format(text))
                        print("")
                        # strip out non-ASCII text so we can draw the text on the image
                        # using OpenCV, then draw a bounding box around the text along
                        # with the text itself
                        text = "".join([c if ord(c) < 128 else "" for c in text]).strip()
                        cv2.rectangle(image, (x, y), (x + w, y + h), (0, 255, 0), 2)
                        cv2.putText(image, text, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX,
                                1.2, (0, 0, 255), 3)
                        if(text == currentText and text != ""):
                                n = n + 1
        if (n > 3):
                print(text)
                break
        currentText = text
        # show the output image
        cv2.imshow("Image", image)
        cv2.waitKey(15)
cv2.destroyAllWindows()
