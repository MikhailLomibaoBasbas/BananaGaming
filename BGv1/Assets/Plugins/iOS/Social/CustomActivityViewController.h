//
//  CustomActivityViewController.h
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/4/13.
//
//

#import <UIKit/UIKit.h>
#import <QuartzCore/CAAnimation.h>

@class CustomActivityViewController;

@protocol CustomActivityViewControllerDelegate <NSObject>

@required
-(void) customActivityViewController: (CustomActivityViewController *)customActivityViewController tappedButtonAtIndex:(NSInteger)buttonIndex;

@optional
-(void) customActivityViewController: (CustomActivityViewController *) customActivityViewController didDismissWithButtonIndex: (NSInteger) buttonIndex;
@end

@interface CustomActivityViewController : UIViewController {
    id <CustomActivityViewControllerDelegate> delegate;
}

@property (nonatomic, retain) id <CustomActivityViewControllerDelegate> delegate;

-(id) initWithImageDictionary: (NSDictionary *) imageDictionary;

@end
