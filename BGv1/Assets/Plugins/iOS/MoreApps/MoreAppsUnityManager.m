//
//  MoreAppsUnityManager.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/15/13.
//
//

#import "MoreAppsUnityManager.h"
#import "MoreAppsWebViewController.h"
#import "AppController.h"
#import "ViewControllerWrapper.h"


@interface SubUINavigationController : UINavigationController

@end

@implementation SubUINavigationController

#ifdef __IPHONE_6_0
-(BOOL)shouldAutorotate{
    return YES;
}
-(NSUInteger) supportedInterfaceOrientations {
    return UIInterfaceOrientationMaskLandscapeRight | UIInterfaceOrientationMaskLandscapeLeft;
}
-(UIInterfaceOrientation) preferredInterfaceOrientationForPresentation {
    return UIInterfaceOrientationLandscapeRight;
}
#else
-(BOOL) shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation {
    return (toInterfaceOrientation == UIInterfaceOrientationLandscapeRigh) || (toInterfaceOrientation == UIInterfaceOrientationLandscapeLeft);
}
#endif


@end
@interface MoreAppsUnityManager () <UIWebViewDelegate, MoreAppsWebViewControllerDelegate>
@property (nonatomic, retain) UIViewController *viewControllerWrapper;

@end

@implementation MoreAppsUnityManager
@synthesize viewControllerWrapper;

+ (MoreAppsUnityManager *) sharedManager {
    static MoreAppsUnityManager * sharedInstance;
    if(sharedInstance == NULL)
        sharedInstance = [[MoreAppsUnityManager alloc] init];
    return sharedInstance;
}

-(void) presentViewController:(UIViewController *) viewControllerToPresent animated:(BOOL)flag modalTransitionStyle:(UIModalTransitionStyle) transitionStyle completion:(void (^)(void))completion {
    if(self.viewControllerWrapper != nil) {
        [self.viewControllerWrapper release];
        self.viewControllerWrapper = nil;
    }
    [AppController UnityPause:true];
    self.viewControllerWrapper = [[ViewControllerWrapper alloc] init];
    [[[UIApplication sharedApplication] keyWindow] addSubview:self.viewControllerWrapper.view];
    self.viewControllerWrapper.view.frame = CGRectMake(0, 0, [UIScreen mainScreen].bounds.size.width, [UIScreen mainScreen].bounds.size.height);
    [self.viewControllerWrapper setModalPresentationStyle:transitionStyle];
    [self.viewControllerWrapper presentViewController:viewControllerToPresent animated:flag completion:completion];
}

-(void) openLinkWithString:(NSString *) urlString {
    NSLog(@"%s",__func__);
    NSLog(@"%@", urlString);
    MoreAppsWebViewController *webViewController = [[MoreAppsWebViewController alloc] initWithLink:urlString];
    webViewController.delegate = self;
    SubUINavigationController *navigationController = [[SubUINavigationController alloc] initWithRootViewController:webViewController];
    navigationController.view.autoresizingMask = (UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleHeight | UIViewAutoresizingFlexibleWidth);
    [navigationController setTitle:@"More Applications"];
    [webViewController setTitle:@"More Applications"];
    [navigationController.navigationBar setBarStyle:UIBarStyleBlack];
    UIBarButtonItem *cancelButton = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemCancel target:webViewController action:@selector(closeAction:)];
    UIBarButtonItem *refreshButton = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemRefresh target:webViewController action:@selector(refreshAction:)];
    [webViewController release];
    [webViewController.navigationItem setLeftBarButtonItem:cancelButton animated:NO];
    [webViewController.navigationItem setRightBarButtonItem:refreshButton animated:NO];
    UIModalTransitionStyle transitionStyle = UIModalTransitionStyleFlipHorizontal;
    [self presentViewController:navigationController animated:YES modalTransitionStyle: transitionStyle completion:nil];


}

-(void) MoreAppsWebViewControllerDidFinished:(MoreAppsWebViewController *)webviewController {

    [self.viewControllerWrapper.view removeFromSuperview];
    [self.viewControllerWrapper release];
    self.viewControllerWrapper = nil;
    [AppController UnityPause:false];
}

#pragma mark - ApplicationDidBecomeActive Methods

-(bool) isViewControllerWrapperPresent {
    return (self.viewControllerWrapper != nil);
}

@end
