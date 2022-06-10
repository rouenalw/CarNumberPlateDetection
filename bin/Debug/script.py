# import the necessary packages
from pytesseract import Output
import pytesseract
import cv2

n = 0
PText = ""
done = False
Text = ""

pytesseract.pytesseract.tesseract_cmd = 'C:/Users/waqas/AppData/Local/Tesseract-OCR/tesseract'
TESSDATA_PREFIX = 'C:/Users/waqas/AppData/Local/Tesseract-OCR/tessdata'

# change number to 0 if using default camera
vs = cv2.VideoCapture(0)
vs.set(10, 50)

while done != True:
        Text = ""
        ret, image = vs.read()
        
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        
        results = pytesseract.image_to_data(gray, output_type=Output.DICT, lang="eng")


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
                if conf > 50 and text != "":
                        # display the confidence and text to our terminal
                        # strip out non-ASCII text so we can draw the text on the image
                        # using OpenCV, then draw a bounding box around the text along
                        # with the text itself
                        text = "".join([c if ord(c) < 128 else "" for c in text]).strip()
                        cv2.putText(image, text, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX,
                                1.2, (0, 0, 255), 3)
                        if (text.isalnum()):
                                Text = Text + " " + text

        if(PText == Text and len(Text.strip()) > 5):
                n = n + 1
        elif(len(Text) > 5):
                n = 0

        if(n > 0):
                done = True
                
        if (len(Text) > 5):
                PText = Text

        # show the output image
        cv2.imshow("Image", image)
        cv2.waitKey(1)
print(Text.strip())
vs.release()
cv2.destroyAllWindows()
