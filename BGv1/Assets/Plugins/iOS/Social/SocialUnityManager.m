//
//  SocialUnityManager.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 2/28/13.
//
//

#import "SocialUnityManager.h"
#import "CustomActivityViewController.h"
#import "FacebookUnityManager.h"
#import "TwitterUnityManager.h"
#import "ImageResizer.h"
#import "ViewControllerWrapper.h"
#import "UnityAppController.h"


@interface SocialUnityManager () <CustomActivityViewControllerDelegate> {
}
@property (nonatomic, retain) UIViewController *viewControllerWrapper;
@property (nonatomic, retain) NSMutableDictionary *param;
@property (nonatomic, strong) UIImage *image;
@property (nonatomic, retain) UIActivityIndicatorView *activityIndicatorView;
@end

@implementation SocialUnityManager
@synthesize viewControllerWrapper;
@synthesize param;
@synthesize activityIndicatorView;

+ (SocialUnityManager *) sharedManager {
    static SocialUnityManager * sharedInstance;
    if(sharedInstance == nil) {
        static dispatch_once_t pred;
        dispatch_once(&pred, ^{
            sharedInstance = [[SocialUnityManager alloc] init];
        });
    }
    return sharedInstance;
}

-(void) cleanViewControllerWrapper {
    if(self.viewControllerWrapper != nil) {
        if([self.viewControllerWrapper isViewLoaded] && [self.viewControllerWrapper.view window]) {
            [self.viewControllerWrapper.view removeFromSuperview];
        }
        [self.viewControllerWrapper release];
        self.viewControllerWrapper = nil;
    }
}

- (BOOL) isActivityViewController {
    return [UIActivityViewController class] ? YES : NO;
}

- (void) openSocialView: (NSMutableDictionary *) params imagePath:(NSString *) imagePath {
    NSLog(@"%s", __func__);
    self.param = params;
    self.image = [UIImage imageWithContentsOfFile:[NSBundle pathForResource:@"screenshot" ofType:@"png" inDirectory:imagePath]];
    NSLog(@"%@", self.image);
    [self showCustomActivityViewController];
}

#pragma mark - CustomActivityViewController and Delegate

- (void) showCustomActivityViewController {
    // Initialize viewControllerwrapper
    [self cleanViewControllerWrapper];
    [UnityAppController UnityPause:true];
    self.viewControllerWrapper = [[ViewControllerWrapper alloc] init];
	[[[UIApplication sharedApplication] keyWindow] addSubview:self.viewControllerWrapper.view];
    self.viewControllerWrapper.view.frame = CGRectMake(0, 0, [UIScreen mainScreen].bounds.size.width, [UIScreen mainScreen].bounds.size.height);
    self.viewControllerWrapper.view.autoresizingMask = ( UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight );
    
    NSDictionary *dictionary = [self setImageDictionary];
    
    CustomActivityViewController *cavc = [[CustomActivityViewController alloc] initWithImageDictionary:dictionary];
    cavc.delegate = self;
    [self.viewControllerWrapper presentViewController:cavc animated:YES completion:nil];
}

// This is where you add your customviewController buttons.
-(NSDictionary *) setImageDictionary {
    return [NSDictionary dictionaryWithObjectsAndKeys:
            [UIImage imageNamed:@"f_logo.png"], @"Facebook",
            [UIImage imageNamed:@"t_logo.png"], @"Twitter", nil];
}

-(void) customActivityViewController:(CustomActivityViewController *)customActivityViewController tappedButtonAtIndex:(NSInteger)buttonIndex {
    switch (buttonIndex) {
        case 0: // Facebook
            [self facebookAction];
            //[self facebookActionTest];
            break;
        case 1: // Twitter
            [self twitterAction];
            break;
        default:
            break;
    }
}

-(void) customActivityViewController:(CustomActivityViewController *)customActivityViewController didDismissWithButtonIndex:(NSInteger)buttonIndex {
    /*if([self.viewControllerWrapper isViewLoaded] && [self.viewControllerWrapper.view window]) {
     [self.viewControllerWrapper.view removeFromSuperview];
     [self.viewControllerWrapper release];
     self.viewControllerWrapper = nil;
     }*/
    [self cleanViewControllerWrapper];
    [UnityAppController UnityPause:false];
}

#pragma mark Social Button Actions

-(void) startActivityIndicator {
    [self stopActivityIndicator];
    activityIndicatorView =[[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge];
    [activityIndicatorView setCenter:CGPointMake(self.viewControllerWrapper.view.frame.size.height * 0.5f, self.viewControllerWrapper.view.frame.size.width * 0.5f)];
    activityIndicatorView.autoresizingMask = (UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
    [self.viewControllerWrapper.view addSubview:activityIndicatorView];
    [activityIndicatorView startAnimating];
}

-(void) stopActivityIndicator {
    if(activityIndicatorView != nil) {
        [activityIndicatorView removeFromSuperview];
        [activityIndicatorView release];
        activityIndicatorView = nil;
    }
}

-(void) facebookActionWithScreenshot {
    if(![FacebookUnityManager sharedManager].hasOpenSession) {
        NSString *fbAppId = [[NSUserDefaults standardUserDefaults] stringForKey:@"social_share_appID"];
        [[FacebookUnityManager sharedManager] openSession:fbAppId addCompletionStatements:^(BOOL result){
            if(result) {
                [self startActivityIndicator];
                [self startShareRequestAndAddCompletion:^{
                    [self stopActivityIndicator];
                }];
            } else {
                [self stopActivityIndicator];
                [UnityAppController UnityPause:false];
            }
        }];
    } else {
        [self startActivityIndicator];
        [self startShareRequestAndAddCompletion:^{
            [self stopActivityIndicator];
        }];
    }
}
-(void) startShareRequestAndAddCompletion:(void (^)(void)) completion {
    [[FacebookUnityManager sharedManager] startShareRequestWithPhoto:self.image parameters:self.param completionBlock:^(BOOL isSuccess){
        if(isSuccess == YES) {
            [self AlertViewWithTitle:@"Post Successful" message:@"You have successfully shared to Facebook"];
        } else {
            [self AlertViewWithTitle:@"Post Failed" message:@"Your attempt to share to Facebook has failed"];
        }
        completion();
    }];
    
}

-(void) facebookAction {
    if(![FacebookUnityManager sharedManager].hasOpenSession) {
        NSString *fbAppId = [[NSUserDefaults standardUserDefaults] stringForKey:@"social_share_appID"];
        [[FacebookUnityManager sharedManager] openSession:fbAppId addCompletionStatements:^(BOOL result){
            if(result) {
                [self startActivityIndicator];
                [self startFacebookRequestAndAddCompletion:^{
                    [self stopActivityIndicator];
                }];
            } else {
                [self stopActivityIndicator];
                [UnityAppController UnityPause:false];
            }
        }];
    } else {
        [self startActivityIndicator];
        [self startFacebookRequestAndAddCompletion:^{
            [self stopActivityIndicator];
        }];
    }
    
}

-(void) startFacebookRequestAndAddCompletion:(void (^)(void)) completion {
    [[FacebookUnityManager sharedManager] initializeRequestConnection];
    [[FacebookUnityManager sharedManager] requestWithGraphPath:@"me/feed" parameters:self.param HTTPMethod:@"POST" completionBlock:^(BOOL success){
        if(success == YES) {
            [self AlertViewWithTitle:@"Post Successful" message:@"You have successfully shared to Facebook"];
        } else {
            [self AlertViewWithTitle:@"Post Failed" message:@"Your attempt to share to Facebook has failed"];
        }
        completion();
    }];
}

-(void) twitterAction {
    NSLog(@"%s", __func__);
    NSString *text = [self.param objectForKey:@"message"];
    NSURL *url = [NSURL URLWithString:[self.param objectForKey:@"link"]];
    UIImage *image = [UIImage imageWithContentsOfFile:[NSBundle pathForResource:@"screenshot" ofType:@"png" inDirectory:[[NSUserDefaults standardUserDefaults] stringForKey:@"social_share_screenshot"]]];
    image = [[ImageResizer sharedResizer] resizeImage:image newScale:0.2f];
    
    [[TwitterUnityManager sharedManager] tweetComposeWithViewController:self.viewControllerWrapper text:text image:image url:url completion:^(TwitterStatus result){
        NSString *alertTitle;
        NSString *alertMessage;
        switch (result) {
            case TweetSuccess: {
                alertTitle = @"Tweet Successful";
                alertMessage = @"You have successfully shared to Twitter";
                break;
            }
            case TweetCancelled: {
                [self cleanViewControllerWrapper];
                [UnityAppController UnityPause:false];
                break;
            }
            case TweetNoAccount: {
                alertTitle = @"Tweet Failed";
                alertMessage = @"No account is logged in, Please Log in first before Sharing";
            }
            default:
                break;
        }
        if(result != TweetCancelled) {
            [self AlertViewWithTitle:alertTitle message:alertMessage];
        }
    }];
}

-(void) AlertViewWithTitle:(NSString *) title message:(NSString *) message {
    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:title
                                                        message:message delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
    [alertView show];
    [alertView release];
}

#pragma mark - UIAlertView Delegate
-(void) alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex {
    NSLog(@"ViewControllerWrapper");
    [self cleanViewControllerWrapper];
    [UnityAppController UnityPause:false];
}

#pragma mark - ApplicationDidBecomeActive Methods

-(bool) isViewControllerWrapperPresent {
    return (self.viewControllerWrapper != nil);
}

@end