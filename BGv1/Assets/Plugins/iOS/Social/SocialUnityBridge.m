//
//  SocialUnityManager.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 2/28/13.
//
//

#import "SocialUnityManager.h"

void _socialOpen () {
    NSUserDefaults * defaults = [NSUserDefaults standardUserDefaults];
    NSString *name = [defaults stringForKey:@"social_share_name"];
    NSString *message = [defaults stringForKey:@"social_share_message"];
    NSString *link = [defaults stringForKey:@"social_share_link"];
    NSString *picture = [defaults stringForKey:@"social_share_picture"];
    NSString *caption = [defaults stringForKey:@"social_share_caption"];
    NSString *description = [defaults stringForKey:@"social_share_description"];
    NSString *imagePath = [defaults stringForKey:@"social_share_screenshot"];
    
    NSMutableDictionary *params = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                   message, @"message",
                                   link, @"link",
                                   name, @"name",
                                   picture, @"picture",
                                   caption, @"caption",
                                   description, @"description",
                                   nil];
    [[SocialUnityManager sharedManager] openSocialView:params imagePath:imagePath];
}

bool _isActivityViewController () {
    return [[SocialUnityManager sharedManager] isActivityViewController];
}
