//
//  ImageResizer.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/12/13.
//
//

#import "ImageResizer.h"

@implementation ImageResizer

+ (ImageResizer *) sharedResizer {
    static ImageResizer * sharedInstance;
    if(sharedInstance == nil)
        sharedInstance = [[ImageResizer alloc] init];
    return sharedInstance;
}

- (UIImage *)resizeImage:(UIImage*)image newScale:(float) scale {
    NSLog(@"%s", __func__);
    CGSize newSize = CGSizeMake(image.size.width * scale, image.size.height * scale);
    CGRect newRect = CGRectIntegral(CGRectMake(0, 0, newSize.width, newSize.height));
    CGImageRef imageRef = image.CGImage;
    
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 0);
    CGContextRef context = UIGraphicsGetCurrentContext();
    
    // Set the quality level to use when rescaling
    CGContextSetInterpolationQuality(context, kCGInterpolationHigh);
    CGAffineTransform flipVertical = CGAffineTransformMake(1, 0, 0, -1, 0, newSize.height);
    
    CGContextConcatCTM(context, flipVertical);
    // Draw into the context; this scales the image
    CGContextDrawImage(context, newRect, imageRef);
    
    // Get the resized image from the context and a UIImage
    CGImageRef newImageRef = CGBitmapContextCreateImage(context);
    UIImage *newImage = [UIImage imageWithCGImage:newImageRef];
    
    CGImageRelease(newImageRef);
    UIGraphicsEndImageContext();
    
    return newImage;
}

-(UIImage *) stretchableImage:(UIImage *) image insetMakeTop:(CGFloat) top left:(CGFloat) left bottom:(CGFloat) bottom right:(CGFloat) right  {
    UIEdgeInsets insets = UIEdgeInsetsMake(top, left, bottom, right);
    UIImage *stretchableImage = [image resizableImageWithCapInsets:insets];
    return stretchableImage;
}

@end
