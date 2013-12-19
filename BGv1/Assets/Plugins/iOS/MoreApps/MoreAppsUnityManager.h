//
//  MoreAppsUnityManager.h
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/15/13.
//
//

#import <Foundation/Foundation.h>

#define UNITY_MALISTENER_GAMEOBJECT_NAME            "MoreAppsListener"
#define UNITY_MALISTENER_DIDFINISHBROWSING         "OnMoreAppsDidFinishBrowsing"

@interface MoreAppsUnityManager : NSObject


+(MoreAppsUnityManager *) sharedManager;

-(void) openLinkWithString:(NSString *) urlString;

-(bool) isViewControllerWrapperPresent;

@end
