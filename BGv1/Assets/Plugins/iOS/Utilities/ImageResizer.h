//
//  ImageResizer.h
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/12/13.
//
//

#import <Foundation/Foundation.h>

@interface ImageResizer : NSObject
+ (ImageResizer *) sharedResizer;
- (UIImage *)resizeImage:(UIImage*)image newScale:(float) scale;
-(UIImage *) stretchableImage:(UIImage *) image insetMakeTop:(CGFloat) top left:(CGFloat) left bottom:(CGFloat) bottom right:(CGFloat) right;

@end
