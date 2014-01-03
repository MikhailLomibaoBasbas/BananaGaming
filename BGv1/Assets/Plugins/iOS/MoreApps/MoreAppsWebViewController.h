
//
//  WebViewController.h
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/21/13.
//
//

#import <UIKit/UIKit.h>

@class MoreAppsWebViewController;
@protocol MoreAppsWebViewControllerDelegate <NSObject>

-(void) MoreAppsWebViewControllerDidFinished:(MoreAppsWebViewController *) webviewController ;

@end

@interface MoreAppsWebViewController : UIViewController <UINavigationControllerDelegate>

-(MoreAppsWebViewController *) initWithLink:(NSString *) theLink;

-(void) refreshAction:(id) sender;
-(void) closeAction:(id) sender;

@property (nonatomic, copy) NSString *link;
@property (nonatomic, retain) id<MoreAppsWebViewControllerDelegate> delegate;

@end
