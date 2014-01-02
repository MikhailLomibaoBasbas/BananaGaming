//
//  SocialUnityManager.h
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 2/28/13.
//
//

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

#define UNITY_SOCIALLISTENER_GAMEOBJECT_NAME            "ShareSocialListener"
#define UNITY_SOCIALLISTENER_DIDFINISHSHARING         "OnSocialDidFinishSharing"

@interface SocialUnityManager : NSObject <UIAlertViewDelegate>

+ (SocialUnityManager *) sharedManager;

- (void) openSocialView:(NSMutableDictionary *) params imagePath:(NSString *) imagePath;

- (BOOL) isActivityViewController;

-(bool) isViewControllerWrapperPresent;

@end