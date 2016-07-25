'''
Identify people in video.

published July 2016 under MIT Open License

===============================
Histogram of Oriented Gradients
===============================

The `Histogram of Oriented Gradient
<http://en.wikipedia.org/wiki/Histogram_of_oriented_gradients>`__ (HOG) feature
descriptor [1]_ is popular for object detection.

In the following example, we compute the HOG descriptor and display
a visualisation.

Algorithm overview
------------------

Compute a Histogram of Oriented Gradients (HOG) by

1. (optional) global image normalisation
2. computing the gradient image in x and y
3. computing gradient histograms
4. normalising across blocks
5. flattening into a feature vector

The first stage applies an optional global image normalisation
equalisation that is designed to reduce the influence of illumination
effects. In practice we use gamma (power law) compression, either
computing the square root or the log of each colour channel.
Image texture strength is typically proportional to the local surface
illumination so this compression helps to reduce the effects of local
shadowing and illumination variations.

The second stage computes first order image gradients. These capture
contour, silhouette and some texture information, while providing
further resistance to illumination variations. The locally dominant
colour channel is used, which provides colour invariance to a large
extent. Variant methods may also include second order image derivatives,
which act as primitive bar detectors - a useful feature for capturing,
e.g. bar like structures in bicycles and limbs in humans.

The third stage aims to produce an encoding that is sensitive to
local image content while remaining resistant to small changes in
pose or appearance. The adopted method pools gradient orientation
information locally in the same way as the SIFT [2]_
feature. The image window is divided into small spatial regions,
called "cells". For each cell we accumulate a local 1-D histogram
of gradient or edge orientations over all the pixels in the
cell. This combined cell-level 1-D histogram forms the basic
"orientation histogram" representation. Each orientation histogram
divides the gradient angle range into a fixed number of
predetermined bins. The gradient magnitudes of the pixels in the
cell are used to vote into the orientation histogram.

The fourth stage computes normalisation, which takes local groups of
cells and contrast normalises their overall responses before passing
to next stage. Normalisation introduces better invariance to illumination,
shadowing, and edge contrast. It is performed by accumulating a measure
of local histogram "energy" over local groups of cells that we call
"blocks". The result is used to normalise each cell in the block.
Typically each individual cell is shared between several blocks, but
its normalisations are block dependent and thus different. The cell
thus appears several times in the final output vector with different
normalisations. This may seem redundant but it improves the performance.
We refer to the normalised block descriptors as Histogram of Oriented
Gradient (HOG) descriptors.

The final step collects the HOG descriptors from all blocks of a dense
overlapping grid of blocks covering the detection window into a combined
feature vector for use in the window classifier.

References
----------

.. [1] Dalal, N. and Triggs, B., "Histograms of Oriented Gradients for
       Human Detection," IEEE Computer Society Conference on Computer
       Vision and Pattern Recognition, 2005, San Diego, CA, USA.

.. [2] David G. Lowe, "Distinctive image features from scale-invariant
       keypoints," International Journal of Computer Vision, 60, 2 (2004),
       pp. 91-110.
'''

import matplotlib.pyplot as plt
from skimage.feature import hog
from skimage import color, exposure, io


def hog_detect(skimg):
    print "Running histogram oriented detection..."
    #image = color.rgb2gray(data.astronaut())
    image = color.rgb2gray(skimg)
    fd, hog_image = hog(image, orientations=8, pixels_per_cell=(16, 16),
                        cells_per_block=(1, 1), visualise=True)
    
    fig, (ax1, ax2) = plt.subplots(1, 2, figsize=(32, 16), sharex=True, sharey=True)
    
    ax1.axis('off')
    ax1.imshow(image, cmap=plt.cm.gray)
    ax1.set_title('Input image')
    ax1.set_adjustable('box-forced')
    
    # Rescale histogram for better display
    hog_image_rescaled = exposure.rescale_intensity(hog_image, in_range=(0, 0.02))
    
    ax2.axis('off')
    ax2.imshow(hog_image_rescaled, cmap=plt.cm.gray)
    ax2.set_title('Histogram of Oriented Gradients')
    ax1.set_adjustable('box-forced')
    plt.show()
    return hog_image

def sklearnHOG():
    empty_imgfile = "sam_yoga_nopeople.png"
    people_imgfile = "sam_yoga_people.png"
    
    print("Calculating the difference between" + empty_imgfile + " and " + people_imgfile)
    
    try:
        ppl = io.imread(people_imgfile)
        emp = io.imread(empty_imgfile)
    except Exception as e:
        print "Can't load file. Bummer."
        print str(e)
        return 
    
    #differenced = calc_differences(ppl, emp)
    print("Starting detection on image " + people_imgfile)   
    
    emp_hog = hog_detect(emp)
    ppl_hog = hog_detect(ppl) 
    return emp_hog, ppl_hog

    
'''
Runs HOG (Histogram Oriented Gradient) to generate features from the image.
Uses OpenCV's baked-in SVM (support vector machine) to classify whether or not 
a person is in a given image.
Finds multiple people in the image by taking various-sized slices, detecting 
person, and then aggregating them together.
'''

def openCvHog(imagePath): 
    from imutils.object_detection import non_max_suppression
    import numpy as np
    import cv2
        
    # initialize the HOG descriptor/person detector
    hog = cv2.HOGDescriptor()
    hog.setSVMDetector(cv2.HOGDescriptor_getDefaultPeopleDetector())
    
    
    # load the image and resize it to (1) reduce detection time
    # and (2) improve detection accuracy
    image = cv2.imread(imagePath)
    #image = imutils.resize(image, width=min(400, image.shape[1]))
    orig = image.copy()
    print("Loaded image from file: " + imagePath)
    
    # detect people in the image
    (rects, weights) = hog.detectMultiScale(image, winStride=(4, 4),
    	padding=(8, 8), scale=1.05)
    
    # draw the original bounding boxes
    for (x, y, w, h) in rects:
    	cv2.rectangle(orig, (x, y), (x + w, y + h), (0, 0, 255), 2)
     
    # apply non-maxima suppression to the bounding boxes using a
    # fairly large overlap threshold to try to maintain overlapping
    # boxes that are still people
    rects = np.array([[x, y, x + w, y + h] for (x, y, w, h) in rects])
    pick = non_max_suppression(rects, probs=None, overlapThresh=0.65)
     
    # draw the final bounding boxes
    for (xA, yA, xB, yB) in pick:
    	cv2.rectangle(image, (xA, yA), (xB, yB), (0, 255, 0), 2)
    
    # show some information on the number of bounding boxes
    filename = imagePath[imagePath.rfind("/") + 1:]
    print("[INFO] {}: {} original boxes, {} after suppression".format(
    	filename, len(rects), len(pick)))
     
    # show the output images
    cv2.imshow("Before NMS", orig)
    cv2.imshow("After NMS", image)
    cv2.waitKey(0)

    
    
if __name__ == '__main__':
    #sklearnHOG()
    people_imgfile = "sam_dog.png"
    openCvHog(people_imgfile)
    