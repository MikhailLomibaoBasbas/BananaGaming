//
//  MoreAppsUnityBridge.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/15/13.
//
//

#import "MoreAppsUnityManager.h"


void _moreAppsOpenLink (const char* url) {
    [[MoreAppsUnityManager sharedManager] openLinkWithString:[NSString stringWithUTF8String:url]];
}